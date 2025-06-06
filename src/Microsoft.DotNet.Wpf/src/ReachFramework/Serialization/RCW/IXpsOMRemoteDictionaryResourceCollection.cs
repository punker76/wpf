// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Windows.Xps.Serialization.RCW
{
    /// <summary>
    /// RCW for xpsobjectmodel.idl found in Windows SDK
    /// This is generated code with minor manual edits. 
    /// i.  Generate TLB
    ///      MIDL /TLB xpsobjectmodel.tlb xpsobjectmodel.IDL //xpsobjectmodel.IDL found in Windows SDK
    /// ii. Generate RCW in a DLL
    ///      TLBIMP xpsobjectmodel.tlb // Generates xpsobjectmodel.dll
    /// iii.Decompile the DLL and copy out the RCW by hand.
    ///      ILDASM xpsobjectmodel.dll
    /// </summary>

    [Guid("5C38DB61-7FEC-464A-87BD-41E3BEF018BE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface IXpsOMRemoteDictionaryResourceCollection
    {
        void Append([In] IXpsOMRemoteDictionaryResource @object);

        IXpsOMRemoteDictionaryResource GetAt([In] uint index);

        IXpsOMRemoteDictionaryResource GetByPartName([In] IOpcPartUri partName);

        uint GetCount();

        void InsertAt([In] uint index, [In] IXpsOMRemoteDictionaryResource @object);

        void RemoveAt([In] uint index);

        void SetAt([In] uint index, [In] IXpsOMRemoteDictionaryResource @object);
    }
}
