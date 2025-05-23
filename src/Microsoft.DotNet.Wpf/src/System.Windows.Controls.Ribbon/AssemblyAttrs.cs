﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// This file specifies various assembly level attributes.


using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(
    // Specifies the location of theme specific resources
    ResourceDictionaryLocation.SourceAssembly,
    // Specifies the location of non-theme specific resources:
    ResourceDictionaryLocation.SourceAssembly)]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls.Ribbon")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "System.Windows.Controls")]
