﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
//

using MS.Internal;

namespace System.Windows.Media.Imaging
{
    #region BitmapPalettes
    /// <summary>
    /// Pre-defined palette types
    /// </summary>
    public static class BitmapPalettes
    {
        /// <summary>
        /// BlackAndWhite
        /// </summary>
        public static Imaging.BitmapPalette BlackAndWhite
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedBW, false);
            }
        }

        /// <summary>
        /// BlackAndWhiteTransparent
        /// </summary>
        public static Imaging.BitmapPalette BlackAndWhiteTransparent
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedBW, true);
            }
        }

        /// <summary>
        /// Halftone8
        /// </summary>
        public static Imaging.BitmapPalette Halftone8
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone8, false);
            }
        }

        /// <summary>
        /// Halftone8Transparent
        /// </summary>
        public static Imaging.BitmapPalette Halftone8Transparent
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone8, true);
            }
        }

        /// <summary>
        /// Halftone27
        /// </summary>
        public static Imaging.BitmapPalette Halftone27
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone27, false);
            }
        }

        /// <summary>
        /// Halftone27Transparent
        /// </summary>
        public static Imaging.BitmapPalette Halftone27Transparent
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone27, true);
            }
        }


        /// <summary>
        /// Halftone64
        /// </summary>
        public static Imaging.BitmapPalette Halftone64
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone64, false);
            }
        }

        /// <summary>
        /// Halftone64Transparent
        /// </summary>
        public static Imaging.BitmapPalette Halftone64Transparent
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone64, true);
            }
        }
        
        /// <summary>
        /// Halftone125
        /// </summary>
        public static Imaging.BitmapPalette Halftone125
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone125, false);
            }
        }

        /// <summary>
        /// Halftone125Transparent
        /// </summary>
        public static Imaging.BitmapPalette Halftone125Transparent
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone125, true);
            }
        }

        /// <summary>
        /// Halftone216
        /// </summary>
        public static Imaging.BitmapPalette Halftone216
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone216, false);
            }
        }

        /// <summary>
        /// Halftone216Transparent
        /// </summary>
        public static Imaging.BitmapPalette Halftone216Transparent
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone216, true);
            }
        }

        /// <summary>
        /// Halftone252
        /// </summary>
        public static Imaging.BitmapPalette Halftone252
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone252, false);
            }
        }

        /// <summary>
        /// Halftone252Transparent
        /// </summary>
        public static Imaging.BitmapPalette Halftone252Transparent
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone252, true);
            }
        }

        /// <summary>
        /// Halftone256
        /// </summary>
        public static Imaging.BitmapPalette Halftone256
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone256, false);
            }
        }

        /// <summary>
        /// Halftone256Transparent
        /// </summary>
        public static Imaging.BitmapPalette Halftone256Transparent
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone256, true);
            }
        }

        /// <summary>
        /// Gray4
        /// </summary>
        public static Imaging.BitmapPalette Gray4
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedGray4, false);
            }
        }

        /// <summary>
        /// Gray4Transparent
        /// </summary>
        public static Imaging.BitmapPalette Gray4Transparent
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedGray4, true);
            }
        }

        /// <summary>
        /// Gray16
        /// </summary>
        public static Imaging.BitmapPalette Gray16
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedGray16, false);
            }
        }

        /// <summary>
        /// Gray16Transparent
        /// </summary>
        public static Imaging.BitmapPalette Gray16Transparent
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedGray16, true);
            }
        }

        /// <summary>
        /// Gray256
        /// </summary>
        public static Imaging.BitmapPalette Gray256
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedGray256, false);
            }
        }

        /// <summary>
        /// Gray256Transparent
        /// </summary>
        public static Imaging.BitmapPalette Gray256Transparent
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedGray256, true);
            }
        }

        /// <summary>
        /// WebPalette 
        /// </summary>
        public static Imaging.BitmapPalette WebPalette
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone216, false);
            }
        }

        /// <summary>
        /// WebpaletteTransparent
        /// </summary>
        public static Imaging.BitmapPalette WebPaletteTransparent
        {
            get
            {
                return BitmapPalettes.FromMILPaletteType(WICPaletteType.WICPaletteTypeFixedHalftone216, true);
            }
        }

        internal static Imaging.BitmapPalette FromMILPaletteType(WICPaletteType type, bool hasAlpha)
        {
            int key = (int)type;

            Debug.Assert(key < c_maxPalettes);

            Imaging.BitmapPalette palette;
            Imaging.BitmapPalette[] palettes;

            if (hasAlpha)
            {
                palettes = transparentPalettes;
            }
            else
            {
                palettes = opaquePalettes;
            }

            palette = palettes[key];

            if (palette == null)
            {
                lock (palettes)
                {
                    // palettes might have changed while waiting for the lock.
                    // Need to check again.

                    palette = palettes[key];
                    if (palette == null)
                    {
                        palette = new Imaging.BitmapPalette(type, hasAlpha);
                        palettes[key] = palette;
                    }
                }
            }

            return palette;
        }

        private static Imaging.BitmapPalette[] transparentPalettes
        {
            get
            {
                if (s_transparentPalettes == null)
                {
                    s_transparentPalettes = new Imaging.BitmapPalette[c_maxPalettes];
                }

                return s_transparentPalettes;
            }
        }

        private static Imaging.BitmapPalette[] opaquePalettes
        {
            get
            {
                if (s_opaquePalettes == null)
                {
                    s_opaquePalettes = new Imaging.BitmapPalette[c_maxPalettes];
                }

                return s_opaquePalettes;
            }
        }
        
        private static Imaging.BitmapPalette[] s_transparentPalettes;
        private static Imaging.BitmapPalette[] s_opaquePalettes;

        private const int c_maxPalettes = 64;
    }
    #endregion // Imaging.BitmapPalettes
}


