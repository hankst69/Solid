﻿//----------------------------------------------------------------------------------
// File: "ImageOrderInfo.cs"
// Author: Steffen Hanke
// Date: 2019-2020
//----------------------------------------------------------------------------------
using System;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Dicom.ImageInfo.Impl
{
    /// <summary>
    /// API:NO
    /// ImageOrderInfo
    /// </summary>
    public class ImageOrderInfo : IImageOrderInfo
    {
        private readonly IImageAttributes m_ImageAttributes;

        internal ImageOrderInfo(IImageAttributes dicomAccess)
        {
            ConsistencyCheck.EnsureArgument(dicomAccess).IsNotNull();
            m_ImageAttributes = dicomAccess;

            // --- calculate derived predicates ---
        }

        // A) dicom attributes
        public DateTime AcquisitionDateTime => m_ImageAttributes.AcquisitionDateTime;
        public DateTime ContentDateTime => throw new NotImplementedException(); // m_MrDicomAccess.ContentDateTime;
        public string MeasurementUid => throw new NotImplementedException(); // m_MrDicomAccess.MeasurementUid;
        public int SpatialPostitionIndex => throw new NotImplementedException(); //m_MrDicomAccess.SpatialPostitionIndex;
        public int TemporalPostitionIndex => throw new NotImplementedException(); //m_MrDicomAccess.TemporalPostitionIndex;

        // B) calculated predicates
    }
}

