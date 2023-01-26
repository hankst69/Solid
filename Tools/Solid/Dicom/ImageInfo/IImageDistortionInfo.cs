//----------------------------------------------------------------------------------
// <copyright file="IImageDistortionInfo.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019-2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using Solid.Dicom.ImageInfo.Types;

namespace Solid.Dicom.ImageInfo
{
    /// <summary>
    /// IImageDistortionInfo
    /// </summary>
    public interface IImageDistortionInfo
    {
        // A) dicom attributes
        string DistortionCorrectionType { get; }
        string VolumetricProperties { get; }
        string FrameLevelVolumetricProperties { get; }
        string GradientCoilName { get; }
        string FrameOfReferenceUid { get; }

        // B) calculated predicates
        DistortionCorrectionType ConsolidatedDistortionCorrectionType { get; }
    }
}
