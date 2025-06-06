// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows
{
    /// <summary>
    ///     Attribute which specifies additional category strings which can be localized:
    ///     Accessibility, Content, Navigation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    internal sealed class CustomCategoryAttribute : CategoryAttribute
    {
        internal CustomCategoryAttribute(string name) : base(name)
        {
            Debug.Assert("Content".Equals(name, StringComparison.Ordinal) 
                      || "Accessibility".Equals(name, StringComparison.Ordinal) 
                      || "Navigation".Equals(name, StringComparison.Ordinal));
        }

        protected override string GetLocalizedString(string value)
        {
            // Return a localized version of the custom category
            if (string.Equals(value, "Content", StringComparison.Ordinal))
                return SR.DesignerMetadata_CustomCategory_Content;
            else if (string.Equals(value, "Accessibility", StringComparison.Ordinal))
                return SR.DesignerMetadata_CustomCategory_Accessibility;
            else // if (string.Equals(value, "Navigation", StringComparison.Ordinal))
                return SR.DesignerMetadata_CustomCategory_Navigation;
        }
    }
}
