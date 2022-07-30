//----------------------------------------------------------------------------------
// <copyright file="IDicomFrameDataSet.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Dicom
{
    /// <summary>
    /// API:YES
    /// IDicomFrameDataSet
    /// </summary>
    public interface IDicomFrameDataSet : IDicomDataSet
    {
        /// <summary>API:YES
        /// The number of the frame (in case this DataSet represents a specific frame out of an EnhancedMrMultiframeImage)
        /// </summary>
        int FrameNumber { get; }
    }
}
