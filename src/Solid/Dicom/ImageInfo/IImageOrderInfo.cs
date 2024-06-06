//----------------------------------------------------------------------------------
// File: "IImageOrderInfo.cs"
// Author: Steffen Hanke
// Date: 2019-2020
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
