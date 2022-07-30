//----------------------------------------------------------------------------------
// <copyright file="ImagePixelInfo.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2021. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Dicom.ImageInfo.Impl
{
    /// <summary>
    /// API:NO
    /// ImagePixelInfo
    /// </summary>
    public class ImagePixelInfo : IImagePixelInfo
    {
        private readonly IImageAttributes m_ImageAttributes;

        private readonly Lazy<bool> m_PixelDataIsBigEndian;
        private readonly Lazy<bool> m_PixelDataIsSigned;
        private readonly Lazy<bool> m_PixelDataIsLossyCompressed;

        internal ImagePixelInfo(IImageAttributes dicomAccess)
        {
            ConsistencyCheck.EnsureArgument(dicomAccess).IsNotNull();
            m_ImageAttributes = dicomAccess;

            // --- calculate derived predicates ---
            m_PixelDataIsBigEndian = new Lazy<bool>(() =>
            {
                return m_ImageAttributes.TransferSyntaxUid == "1.2.840.10008.1.2.2";
            });

            m_PixelDataIsSigned = new Lazy<bool>(() =>
            {
                return m_ImageAttributes.PixelRepresentation > 0;
            });

            m_PixelDataIsLossyCompressed = new Lazy<bool>(() =>
            {
                return m_ImageAttributes.LossyImageCompression != null && m_ImageAttributes.LossyImageCompression != "00";
            });
        }

        // A) dicom attributes
        // --- PixelMatrix information ---
        public int NumberOfFrames => m_ImageAttributes.NumberOfFrames;
        public int PixelRows => m_ImageAttributes.MatrixRows;
        public int PixelColumns => m_ImageAttributes.MatrixCols;
        public double PixelSizeInRowDir => m_ImageAttributes.PixelSpacingRow;
        public double PixelSizeInColDir => m_ImageAttributes.PixelSpacingCol;
        public double SliceThickness => m_ImageAttributes.SliceThickness;

        // --- PixelData information ---
        public string TransferSyntaxUid => m_ImageAttributes.TransferSyntaxUid;
        public string PhotometricInterpretation => m_ImageAttributes.PhotometricInterpretation;
        public ushort SamplesPerPixel => m_ImageAttributes.SamplesPerPixel;
        public ushort PlanarConfiguration => m_ImageAttributes.PlanarConfiguration;
        public ushort BitsAllocated => m_ImageAttributes.BitsAllocated;
        public ushort BitsStored => m_ImageAttributes.BitsStored;
        public ushort HighBit => m_ImageAttributes.HighBit;
        public ushort PixelRepresentation => m_ImageAttributes.PixelRepresentation;
        public double WindowCenter => m_ImageAttributes.WindowCenter;
        public double WindowWidth => m_ImageAttributes.WindowWidth;
        public double RescaleIntercept => m_ImageAttributes.RescaleIntercept;
        public double RescaleSlope => m_ImageAttributes.RescaleSlope;
        public string LossyImageCompression => m_ImageAttributes.LossyImageCompression;

        // B) calculated predicates
        public bool PixelDataIsBigEndian => m_PixelDataIsBigEndian.Value;
        public bool PixelDataIsSigned => m_PixelDataIsSigned.Value;
        public bool PixelDataIsLossyCompressed => m_PixelDataIsLossyCompressed.Value;
    }
}
