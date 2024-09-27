//----------------------------------------------------------------------------------
// File: "IImageDistortionInfo.cs"
// Author: Steffen Hanke
// Date: 2019-2020
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
