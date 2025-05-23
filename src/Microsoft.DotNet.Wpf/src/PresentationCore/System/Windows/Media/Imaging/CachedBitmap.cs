﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using MS.Win32.PresentationCore;
using MS.Internal;

namespace System.Windows.Media.Imaging
{
    #region CachedBitmap

    /// <summary>
    /// CachedBitmap provides caching functionality for a BitmapSource.
    /// </summary>
    public sealed class CachedBitmap : System.Windows.Media.Imaging.BitmapSource
    {
        /// <summary>
        /// Construct a CachedBitmap
        /// </summary>
        /// <param name="source">BitmapSource to apply to the crop to</param>
        /// <param name="createOptions">CreateOptions for the new Bitmap</param>
        /// <param name="cacheOption">CacheOption for the new Bitmap</param>
        public CachedBitmap(BitmapSource source, BitmapCreateOptions createOptions, BitmapCacheOption cacheOption)
            : base(true) // Use base class virtuals
        {
            ArgumentNullException.ThrowIfNull(source);

            BeginInit();
            _source = source;
            RegisterDownloadEventSource(_source);
            _createOptions = createOptions;
            _cacheOption = cacheOption;
            _syncObject = source.SyncObject;

            EndInit();
        }

        /// <summary>
        /// </summary>
        internal unsafe CachedBitmap(
                    int pixelWidth,
                    int pixelHeight,
                    double dpiX,
                    double dpiY,
                    PixelFormat pixelFormat,
                    BitmapPalette palette,
                    IntPtr buffer,
                    int bufferSize,
                    int stride
                    )

            : base(true) // Use base class virtuals
        {
            InitFromMemoryPtr(pixelWidth, pixelHeight, dpiX, dpiY,
                              pixelFormat, palette,
                              buffer, bufferSize, stride);
        }

        /// <summary>
        ///     Creates a managed BitmapSource wrapper around a pre-existing 
        ///     unmanaged bitmap.
        /// </summary>
        internal CachedBitmap(BitmapSourceSafeMILHandle bitmap) : base(true)
        {
            ArgumentNullException.ThrowIfNull(bitmap);

            // We're not calling CachedBitmap.Begin/EndInit because that would
            // invoke FinalizeCreation which calls CreateCachedBitmap that does
            // unnecessary work and only deals with BitmapFrames
            _bitmapInit.BeginInit();

            _source = null;
            _createOptions = BitmapCreateOptions.None;
            _cacheOption = BitmapCacheOption.OnLoad;

            //
            // This constructor is used by D3DImage, which does not calculate memory pressure for
            // the bitmap parameter before calling the constructor.
            //
            bitmap.CalculateSize();
            // This will QI to IWICBitmapSource for us. QI also AddRefs, of course.
            WicSourceHandle = bitmap;
            _syncObject = WicSourceHandle;

            IsSourceCached = true;
            CreationCompleted = true;

            _bitmapInit.EndInit();
        }

        /// <summary>
        /// Create an Empty CachedBitap
        /// </summary>
        private CachedBitmap()
            : base(true) // Use base class virtuals
        {
        }

        /// <summary>
        /// </summary>
        internal unsafe CachedBitmap(
            int pixelWidth,
            int pixelHeight,
            double dpiX,
            double dpiY,
            PixelFormat pixelFormat,
            BitmapPalette palette,
            System.Array pixels,
            int stride
            )
            : base(true) // Use base class virtuals
        {
            ArgumentNullException.ThrowIfNull(pixels);

            if (pixels.Rank != 1)
                throw new ArgumentException(SR.Collection_BadRank, nameof(pixels));

            int elementSize = -1;

            if (pixels is byte[])
                elementSize = 1;
            else if (pixels is short[] || pixels is ushort[])
                elementSize = 2;
            else if (pixels is int[] || pixels is uint[] || pixels is float[])
                elementSize = 4;
            else if (pixels is double[])
                elementSize = 8;

            if (elementSize == -1)
                throw new ArgumentException(SR.Image_InvalidArrayForPixel);

            int destBufferSize = elementSize * pixels.Length;

            fixed (byte* pixelArray = &MemoryMarshal.GetArrayDataReference(pixels))
                InitFromMemoryPtr(pixelWidth, pixelHeight, dpiX, dpiY, pixelFormat, palette, (nint)pixelArray, destBufferSize, stride);
        }

        /// <summary>
        /// Common implementation for CloneCore(), CloneCurrentValueCore(),
        /// GetAsFrozenCore(), and GetCurrentValueAsFrozenCore().
        /// </summary>
        private void CopyCommon(CachedBitmap sourceBitmap)
        {
            // Avoid Animatable requesting resource updates for invalidations that occur during construction
            Animatable_IsResourceInvalidationNecessary = false;

            if (sourceBitmap._source != null)
            {
                BeginInit();
                _syncObject = sourceBitmap._syncObject;
                _source = sourceBitmap._source;
                RegisterDownloadEventSource(_source);
                _createOptions = sourceBitmap._createOptions;
                _cacheOption = sourceBitmap._cacheOption;

                if (_cacheOption == BitmapCacheOption.OnDemand)
                    _cacheOption = BitmapCacheOption.OnLoad;
                EndInit();
            }
            else
            {
                InitFromWICSource(sourceBitmap.WicSourceHandle);
            }
            // The next invalidation will cause Animatable to register an UpdateResource callback
            Animatable_IsResourceInvalidationNecessary = true;
        }


        // ISupportInitialize

        /// <summary>
        /// Prepare the bitmap to accept initialize paramters.
        /// </summary>
        private void BeginInit()
        {
            _bitmapInit.BeginInit();
        }

        /// <summary>
        /// Prepare the bitmap to accept initialize paramters.
        /// </summary>
        private void EndInit()
        {
            _bitmapInit.EndInit();

            // if we don't need to delay, let 'er rip
            if (!DelayCreation)
                FinalizeCreation();
        }

        ///
        /// Create the unmanaged resources
        ///
        internal override void FinalizeCreation()
        {
            lock (_syncObject)
            {
                WicSourceHandle = CreateCachedBitmap(_source as BitmapFrame, _source.WicSourceHandle, _createOptions, _cacheOption, _source.Palette);
            }

            IsSourceCached = (_cacheOption != BitmapCacheOption.None);
            CreationCompleted = true;
            UpdateCachedSettings();
        }

        #region Public Methods
        /// <summary>
        ///     Shadows inherited Copy() with a strongly typed
        ///     version for convenience.
        /// </summary>
        public new CachedBitmap Clone()
        {
            return (CachedBitmap)base.Clone();
        }

        /// <summary>
        ///     Shadows inherited CloneCurrentValue() with a
        ///     strongly typed version for convenience.
        /// </summary>
        public new CachedBitmap CloneCurrentValue()
        {
            return (CachedBitmap)base.CloneCurrentValue();
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CreateInstanceCore">Freezable.CreateInstanceCore</see>.
        /// </summary>
        /// <returns>The new Freezable.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new CachedBitmap();
        }

        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CloneCore(Freezable)">Freezable.CloneCore</see>.
        /// </summary>
        protected override void CloneCore(Freezable sourceFreezable)
        {
            CachedBitmap sourceBitmap = (CachedBitmap) sourceFreezable;

            base.CloneCore(sourceFreezable);

            CopyCommon(sourceBitmap);
        }

        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CloneCurrentValueCore(Freezable)">Freezable.CloneCurrentValueCore</see>.
        /// </summary>
        protected override void CloneCurrentValueCore(Freezable sourceFreezable)
        {
            CachedBitmap sourceBitmap = (CachedBitmap) sourceFreezable;

            base.CloneCurrentValueCore(sourceFreezable);

            CopyCommon(sourceBitmap);
        }


        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.GetAsFrozenCore(Freezable)">Freezable.GetAsFrozenCore</see>.
        /// </summary>
        protected override void GetAsFrozenCore(Freezable sourceFreezable)
        {
            CachedBitmap sourceBitmap = (CachedBitmap)sourceFreezable;

            base.GetAsFrozenCore(sourceFreezable);

            CopyCommon(sourceBitmap);
        }


        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.GetCurrentValueAsFrozenCore(Freezable)">Freezable.GetCurrentValueAsFrozenCore</see>.
        /// </summary>
        protected override void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            CachedBitmap sourceBitmap = (CachedBitmap)sourceFreezable;

            base.GetCurrentValueAsFrozenCore(sourceFreezable);

            CopyCommon(sourceBitmap);
        }

        #endregion ProtectedMethods

        ///
        /// Create from WICBitmapSource
        ///
        private void InitFromWICSource(
                    SafeMILHandle wicSource
                    )
        {
            _bitmapInit.BeginInit();

            BitmapSourceSafeMILHandle bitmapSource = null;

            lock (_syncObject)
            {
                using (FactoryMaker factoryMaker = new FactoryMaker())
                {
                    HRESULT.Check(UnsafeNativeMethods.WICImagingFactory.CreateBitmapFromSource(
                            factoryMaker.ImagingFactoryPtr,
                            wicSource,
                            WICBitmapCreateCacheOptions.WICBitmapCacheOnLoad,
                            out bitmapSource));
                }

                bitmapSource.CalculateSize();
            }
            WicSourceHandle = bitmapSource;
            _isSourceCached = true;

            _bitmapInit.EndInit();

            UpdateCachedSettings();
        }

        ///
        /// Create from memory
        ///
        private void InitFromMemoryPtr(
                    int pixelWidth,
                    int pixelHeight,
                    double dpiX,
                    double dpiY,
                    PixelFormat pixelFormat,
                    BitmapPalette palette,
                    IntPtr buffer,
                    int bufferSize,
                    int stride
                    )
        {
            if (pixelFormat.Palettized && palette == null)
                throw new InvalidOperationException(SR.Image_IndexedPixelFormatRequiresPalette);

            if (pixelFormat.Format == PixelFormatEnum.Default && pixelFormat.Guid == WICPixelFormatGUIDs.WICPixelFormatDontCare)
            {
                throw new System.ArgumentException(
                        SR.Format(SR.Effect_PixelFormat, pixelFormat),
                        nameof(pixelFormat)
                        );
            }

            _bitmapInit.BeginInit();

            try
            {
                BitmapSourceSafeMILHandle wicBitmap;

                // Create the unmanaged resources
                Guid guidFmt = pixelFormat.Guid;

                using (FactoryMaker factoryMaker = new FactoryMaker())
                {
                    HRESULT.Check(UnsafeNativeMethods.WICImagingFactory.CreateBitmapFromMemory(
                            factoryMaker.ImagingFactoryPtr,
                            (uint)pixelWidth, (uint)pixelHeight,
                            ref guidFmt,
                            (uint)stride,
                            (uint)bufferSize,
                            buffer,
                            out wicBitmap));

                    wicBitmap.CalculateSize();
                }

                HRESULT.Check(UnsafeNativeMethods.WICBitmap.SetResolution(
                        wicBitmap,
                        dpiX,
                        dpiY));

                if (pixelFormat.Palettized)
                {
                    HRESULT.Check(UnsafeNativeMethods.WICBitmap.SetPalette(
                        wicBitmap,
                        palette.InternalPalette));
                }

                WicSourceHandle = wicBitmap;
                _isSourceCached = true;
            }
            catch
            {
                _bitmapInit.Reset();
                throw;
            }

            _createOptions = BitmapCreateOptions.PreservePixelFormat;
            _cacheOption = BitmapCacheOption.OnLoad;
            _syncObject = WicSourceHandle;
            _bitmapInit.EndInit();

            UpdateCachedSettings();
        }

        private BitmapSource        _source;
        private BitmapCreateOptions _createOptions = BitmapCreateOptions.None;
        private BitmapCacheOption   _cacheOption = BitmapCacheOption.Default;
    }
    #endregion // CachedBitmap
}
