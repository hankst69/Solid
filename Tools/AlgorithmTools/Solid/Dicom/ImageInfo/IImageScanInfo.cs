//----------------------------------------------------------------------------------
// <copyright file="IImageScanInfo.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Dicom.ImageInfo
{
    /// <summary>
    /// IImageScanInfo
    /// </summary>
    public interface IImageScanInfo
    {
        // A) dicom attributes
        string ProtocolName { get; }
        string SequencelName { get; }
        string InPlanePhaseEncodingDirection { get; }

        // B) calculated predicates
        bool IsInplanePhaseInRowDirection { get; }
        bool IsPhaseEncodingDirectionPositive { get; }
    }
}
