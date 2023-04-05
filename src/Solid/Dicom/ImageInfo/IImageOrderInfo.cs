//----------------------------------------------------------------------------------
// <copyright file="IImageOrderInfo.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;

namespace Solid.Dicom.ImageInfo
{
    /// <summary>
    /// IImageOrderInfo
    /// </summary>
    public interface IImageOrderInfo
    {
        // A) dicom attributes
        DateTime AcquisitionDateTime { get; }
        DateTime ContentDateTime { get; }
        string MeasurementUid { get; }
        int SpatialPostitionIndex { get; }
        int TemporalPostitionIndex { get; }

        // B) calculated predicates
    }
}
