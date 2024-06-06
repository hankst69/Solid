//----------------------------------------------------------------------------------
// 2020-2024
// Author: Steffen Hanke
//----------------------------------------------------------------------------------
using System.Collections.Generic;

namespace MeanSquareErrorImageCompare
{
    /// <summary>
    /// API:YES
    /// InputValidationFinding
    /// </summary>
    public enum InputValidationFinding
    {
        PixelDataMissing,
        DifferInMatrixSize,
        DifferInPixelSpacing,
        DifferInSliceThickness,
        ImagePlaneInfoMissing,
        DifferInImagePosition,
        DifferInImageOrientation,
        DifferInCompressionType,
        DifferInDistortionCorrectionType
    }

    /// <summary>
    /// API:YES
    /// ICrossImageMeanSquareErrorResult
    /// </summary>
    public interface IMeanSquareErrorImageCompareResult
    {
        /// <summary>
        /// MeanSquareError
        /// </summary>
        /// <returns>
        /// the double value that represents the calculated MeanSquareError of the 2 input images
        /// </returns>
        double MeanSquareError { get; }

        /// <summary>
        /// Errors:
        /// list of necessary prerequisites that the input images did not fulfill 
        /// -> there is no MeanSquareError result
        /// </summary>
        /// <returns>
        /// a collection of InputValidationFinding values
        /// </returns>
        IEnumerable<InputValidationFinding> Errors { get; }

        /// <summary>
        /// Warnings
        /// list of properties that are inconsistent between the input images and which therefore might render the MeanSquareError result useless
        /// -> the MeanSquareError result is questionable
        /// </summary>
        /// <returns>
        /// a collection of InputValidationFinding values
        /// </returns>
        IEnumerable<InputValidationFinding> Warnings { get; }
    }
}
