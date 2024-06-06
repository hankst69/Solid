//----------------------------------------------------------------------------------
// File: "IImageDataVolumeValidator.cs"
// Author: Steffen Hanke
// Date: 2019-2022
//----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Solid.Dicom.ImageData
{
    public interface IVolumeValidationResult
    {
        bool ImageCountSufficient { get; }
        bool ImageMatrixesMatch { get; }
        bool ImageFoVsMatch { get; }
        bool ImageNormalsParallel { get; }
        bool ImagePositionsInLine { get; }
        bool ImagePositionsLineParallelToImageNormals { get; }
        bool ImageDistancesEquidistant { get; }

        bool Valid { get; }
        bool AllValid { get; }
    }

    /// <summary>
    /// IImageDataVolumeValidator
    /// </summary>
    public interface IImageDataVolumeValidator
    {
        IVolumeValidationResult ValidateVolumeImageData(IEnumerable<IImageData> inputImages, int minimumSliceCount = 3, double tolerance = 0.0001);

        bool IsImageDataValidForVolumeCreation(IEnumerable<IImageData> inputImages, bool extendedRequirements);
    }
}
