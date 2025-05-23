// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Collections.ObjectModel;

namespace System.Windows.Input
{
    /// <summary>
    /// Collection of the stylus devices that are available on the tablet.
    /// </summary>
    public class StylusDeviceCollection : ReadOnlyCollection<StylusDevice>
    {
        /// <summary>
        ///
        /// This was changed to IEnumerable since the collection is exposed to
        /// developers.  Internally we use the inheritance hierarchy but externally
        /// we use the wrapper classes, requiring us to build the list dynamically.
        /// </summary>
        /// <param name="styluses">The collection of stylus objects</param>
        internal StylusDeviceCollection(IEnumerable<StylusDeviceBase> styluses)
            : base(new List<StylusDevice>())
        {
            foreach (var stylusDevice in styluses)
            {
                Items.Add(stylusDevice.StylusDevice);
            }
        }

        internal void Dispose()
        {
            foreach (StylusDevice stylusDevice in this.Items)
            {
                stylusDevice.StylusDeviceImpl.Dispose();
            }
        }

        internal void AddStylusDevice(int index, StylusDeviceBase stylusDevice)
        {
            base.Items.Insert(index, stylusDevice.StylusDevice); // add it to our list.
        }
    }
}
