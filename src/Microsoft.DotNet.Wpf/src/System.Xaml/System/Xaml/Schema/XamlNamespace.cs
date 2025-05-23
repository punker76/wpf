﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Xaml.MS.Impl;
using MS.Internal.Xaml.Parser;

namespace System.Xaml.Schema
{
    internal class XamlNamespace
    {
        public readonly XamlSchemaContext SchemaContext;

        private AssemblyNamespacePair[] _assemblyNamespaces;
        private ConcurrentDictionary<string, XamlType> _typeCache;
        private ICollection<XamlType> _allPublicTypes;

        public bool IsClrNamespace { get; private set; }

        public XamlNamespace(XamlSchemaContext schemaContext)
        {
            SchemaContext = schemaContext;
        }

        // This ctor is used for "clr-namespace" uri's where there is only one pair
        public XamlNamespace(XamlSchemaContext schemaContext, string clrNs, string assemblyName)
        {
            SchemaContext = schemaContext;
            _assemblyNamespaces = GetClrNamespacePair(clrNs, assemblyName);
            // For now we just ignore failures, including swalloing assembly load exceptions.
            // Any types in this namespace will be treated as unknown. But it would be useful to
            // surface errors here through tracing or an event.
            if (_assemblyNamespaces is not null)
            {
                Initialize();
            }

            IsClrNamespace = true;
        }

        private void Initialize()
        {
            _typeCache = XamlSchemaContext.CreateDictionary<string, XamlType>();
        }

        public bool IsResolved => _assemblyNamespaces is not null;

        public ICollection<XamlType> GetAllXamlTypes() => _allPublicTypes ??= LookupAllTypes();

        public XamlType GetXamlType(string typeName, params XamlType[] typeArgs)
        {
            if (!IsResolved)
            {
                return null;
            }

            if (typeArgs is null || typeArgs.Length == 0)
            {
                return TryGetXamlType(typeName) ?? TryGetXamlType(GetTypeExtensionName(typeName));
            }

            Type[] clrTypeArgs = ConvertArrayOfXamlTypesToTypes(typeArgs);
            return TryGetXamlType(typeName, clrTypeArgs) ?? TryGetXamlType(GetTypeExtensionName(typeName), clrTypeArgs);
        }

        private XamlType TryGetXamlType(string typeName)
        {
            // Look up the type in our cache. If it's there, we're done.
            if (_typeCache.TryGetValue(typeName, out XamlType xamlType))
            {
                return xamlType;
            }

            // Otherwise, look up the type via reflection
            Type type = TryGetType(typeName);
            if (type is null)
            {
                return null;
            }

            // And save it in our cache
            xamlType = SchemaContext.GetXamlType(type);
            if (xamlType is null)
            {
                return null;
            }

            return XamlSchemaContext.TryAdd(_typeCache, typeName, xamlType);
        }

        private XamlType TryGetXamlType(string typeName, Type[] typeArgs)
        {
            Debug.Assert(typeArgs.Length > 0, "This method should only be called for generic types.");

            // It is not possible to get an array of open generic and then call
            // MakeGenericType on it so we need to process array subscripts.
            ReadOnlySpan<char> typeNameSpan = GenericTypeNameScanner.StripSubscript(typeName, out ReadOnlySpan<char> subscript);
            string mangledTypeName = MangleGenericTypeName(typeNameSpan, typeArgs.Length);

            // Get the open generic type.
            XamlType openXamlType = TryGetXamlType(mangledTypeName);
            Type openType = openXamlType?.UnderlyingType;
            if (openType is null)
            {
                return null;
            }

            // Close the open generic type.
            Type closedType = openType.MakeGenericType(typeArgs);
            if (!subscript.IsEmpty)
            {
                closedType = MakeArrayType(closedType, subscript);
                if (closedType is null)
                {
                    // Invalid array subscript.
                    return null;
                }
            }

            return SchemaContext.GetXamlType(closedType);
        }

        private static Type MakeArrayType(Type elementType, ReadOnlySpan<char> subscript)
        {
            Type type = elementType;
            int pos = 0;
            do
            {
                int rank = GenericTypeNameScanner.ParseSubscriptSegment(subscript, ref pos);
                if (rank == 0)
                {
                    // Invalid array subscript.
                    return null;
                }

                type = (rank == 1) ? type.MakeArrayType() : type.MakeArrayType(rank);
            }
            while (pos < subscript.Length);
            return type;
        }

        private static string MangleGenericTypeName(ReadOnlySpan<char> typeName, int paramNum)
        {
            return $"{typeName}{KnownStrings.GraveQuote}{paramNum}";
        }

        private Type[] ConvertArrayOfXamlTypesToTypes(XamlType[] typeArgs)
        {
            var clrTypeArgs = new Type[typeArgs.Length];
            for (int n = 0; n < typeArgs.Length; n++)
            {
                // Checking for nulls and unknowns is done in public API layer before we ever get here
                Debug.Assert(typeArgs[n] is not null);
                Debug.Assert(typeArgs[n].UnderlyingType is not null);

                clrTypeArgs[n] = typeArgs[n].UnderlyingType;
            }

            return clrTypeArgs;
        }

        internal int RevisionNumber
        {
            // The only external mutation we allow is adding new namespaces. So the count of
            // namespaces also serves as a revision number.
            get => _assemblyNamespaces?.Length ?? 0;
        }

        private Type TryGetType(string typeName)
        {
            Type type = SearchAssembliesForShortName(typeName);
            if (type is null && IsClrNamespace)
            {
                Debug.Assert(_assemblyNamespaces.Length == 1);
                type = XamlLanguage.LookupClrNamespaceType(_assemblyNamespaces[0], typeName);
            }

            if (type is null)
            {
                return null;
            }

            // Discard private nested types
            Type currentType = type;
            while (currentType.IsNested)
            {
                if (currentType.IsNestedPrivate)
                {
                    return null;
                }

                currentType = currentType.DeclaringType;
            }

            return type;
        }

        private ICollection<XamlType> LookupAllTypes()
        {
            List<XamlType> xamlTypeList = new List<XamlType>();
            if (IsResolved)
            {
                foreach (AssemblyNamespacePair assemblyNamespacePair in _assemblyNamespaces)
                {
                    Assembly asm = assemblyNamespacePair.Assembly;
                    if (asm is null)
                    {
                        // This is a dynamic assembly that got unloaded; ignore it
                        continue;
                    }

                    string clrPrefix = assemblyNamespacePair.ClrNamespace;

                    Type[] types = asm.GetTypes();

                    foreach (Type t in types)
                    {
                        if (!KS.Eq(t.Namespace, clrPrefix))
                            continue;

                        XamlType xamlType = SchemaContext.GetXamlType(t);
                        xamlTypeList.Add(xamlType);
                    }
                }
            }

            return xamlTypeList.AsReadOnly();
        }

        private AssemblyNamespacePair[] GetClrNamespacePair(string clrNs, string assemblyName)
        {
            Assembly asm = SchemaContext.OnAssemblyResolve(assemblyName);
            if (asm is null)
                return null;

            return new AssemblyNamespacePair[1] { new AssemblyNamespacePair(asm, clrNs) };
        }

        private Type SearchAssembliesForShortName(string shortName)
        {
            foreach (AssemblyNamespacePair assemblyNamespacePair in _assemblyNamespaces)
            {
                Assembly asm = assemblyNamespacePair.Assembly;
                if (asm is null)
                {
                    // This is a dynamic assembly that got unloaded; ignore it
                    continue;
                }

                string longName = $"{assemblyNamespacePair.ClrNamespace}.{shortName}";

                Type type = asm.GetType(longName);
                if (type is not null)
                {
                    return type;
                }
            }

            return null;
        }

        // This method should only be called inside SchemaContext._syncExaminingAssemblies lock
        internal void AddAssemblyNamespacePair(AssemblyNamespacePair pair)
        {
            // To allow the array to be read concurrently by multiple threads, we create a new array, add the pair,
            // then assign it back to the original variable. Assignments are guaranteed to be atomic for word size.

            AssemblyNamespacePair[] assemblyNamespacesCopy;
            if (_assemblyNamespaces is null)
            {
                assemblyNamespacesCopy = new AssemblyNamespacePair[1];
                Initialize();
            }
            else
            {
                // Copy items over to the new collection
                assemblyNamespacesCopy = new AssemblyNamespacePair[_assemblyNamespaces.Length + 1];
                _assemblyNamespaces.CopyTo(assemblyNamespacesCopy, 0);
            }

            // Add new pair as the last one
            assemblyNamespacesCopy[^1] = pair;

            _assemblyNamespaces = assemblyNamespacesCopy;
        }

        private string GetTypeExtensionName(string typeName) => typeName + KnownStrings.Extension;
    }
}
