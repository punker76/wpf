// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
// Description:
//      ValidationErrorCollection contains the list of ValidationErrors from
//      the various Bindings and MultiBindings on an Element.  ValidationErrorCollection
//      be set through the Validation.ErrorsProperty.
//
// See specs at Specs/Validation.mht
//


using System.Collections.ObjectModel;
using System.Windows.Controls;


namespace MS.Internal.Controls
{
    /// <summary>
    ///      ValidationErrorCollection contains the list of ValidationErrors from
    ///      the various Bindings and MultiBindings on an Element.  ValidationErrorCollection
    ///      be set through the Validation.ErrorsProperty.
    /// </summary>
    internal class ValidationErrorCollection : ObservableCollection<ValidationError>
    {
        /// <summary>
        /// Empty collection that serves as a default value for
        /// Validation.ErrorsProperty.
        /// </summary>
        public static readonly ReadOnlyObservableCollection<ValidationError> Empty =
                new ReadOnlyObservableCollection<ValidationError>(new ValidationErrorCollection());
    }
}
