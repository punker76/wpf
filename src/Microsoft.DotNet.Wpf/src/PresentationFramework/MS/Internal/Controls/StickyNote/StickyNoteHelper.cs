// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
// Description: Implementation of StickyNoteControl's internal helper classes.
//
//              See spec at http://tabletpc/longhorn/Specs/StickyNoteControlSpec.mht
//

namespace MS.Internal.Controls.StickyNote
{
    //-------------------------------------------------------------------------------
    //
    // Internal Classes
    //
    //-------------------------------------------------------------------------------

    #region Internal Classes


    // This is a helper class which is used as a lock which can lock or unlock a specified flag. The usage looks like:
    //
    // public class Test
    // {
    //      public void MethodA()
    //      {
    //          // A lock is needed in a scope which can block MethodB likes a event handler.
    //          using ( LockHelper.AutoLocker locker = new LockHelper.AutoLocker(InternalLocker, LockHelper.LockFlag.SliderValueChanged) )
    //          {
    //              // Do some work.
    //          }
    //      }
    //
    //      public void MethodB()
    //      {
    //          // We don't want to handle the below if the locker is locked.
    //          if ( !InternalLocker.IsLocked() )
    //          {
    //              // Do some work.
    //          }
    //      }
    // }
    internal class LockHelper
    {
        [Flags]
        public enum LockFlag
        {
            AnnotationChanged = 0x00000001,
            DataChanged = 0x00000002,
        }

        public class AutoLocker : IDisposable
        {
            public AutoLocker(LockHelper helper, LockFlag flag)
            {
                ArgumentNullException.ThrowIfNull(helper);

                Debug.Assert(!helper.IsLocked(flag));
                _helper = helper;
                _flag = flag;

                // Lock the locker at the object's creation time.
                _helper.Lock(_flag);
            }

            public void Dispose()
            {
                Debug.Assert(_helper.IsLocked(_flag));

                // Unlock the locker when it's being disposed.
                _helper.Unlock(_flag);
                GC.SuppressFinalize(this);
            }

            private AutoLocker() { }

            private LockHelper _helper;
            private LockFlag _flag;
        }

        public bool IsLocked(LockFlag flag)
        {
            return (_backingStore & flag) != 0;
        }

        private void Lock(LockFlag flag)
        {
            _backingStore |= flag;
        }

        private void Unlock(LockFlag flag)
        {
            _backingStore &= (~flag);
        }

        private LockFlag _backingStore;
    }

    // This class contains all constants which is used by StickyNoteControl component.
    internal static class SNBConstants
    {
        //-------------------------------------------------------------------------------
        //
        // Internal Fields
        //
        //-------------------------------------------------------------------------------

        #region Internal Fields
        // The style ID of StickyNote's children
        internal const string c_CloseButtonId = "PART_CloseButton";
        internal const string c_TitleThumbId = "PART_TitleThumb";
        internal const string c_BottomRightResizeThumbId = "PART_ResizeBottomRightThumb";
        internal const string c_ContentControlId = "PART_ContentControl";
        internal const string c_IconButtonId = "PART_IconButton";
        internal const string c_CopyMenuId = "PART_CopyMenuItem";
        internal const string c_PasteMenuId = "PART_PasteMenuItem";
        internal const string c_ClipboardSeparatorId = "PART_ClipboardSeparator";
        internal const string c_DeleteMenuId = "PART_DeleteMenuItem";
        internal const string c_InkMenuId = "PART_InkMenuItem";
        internal const string c_SelectMenuId = "PART_SelectMenuItem";
        internal const string c_EraseMenuId = "PART_EraseMenuItem";

        // CAF Resouce guids
        internal const string MetaResourceName = "Meta Data";        // Snc MetaData
        internal const string TextResourceName = "Text Data";        // Text
        internal const string InkResourceName = "Ink Data";          // Ink

        #endregion Internal Fields
    }

    #endregion Internal Classes
}







