//----------------------------------------------------------------------------------
// File: "IMeanSquareErrorImageComparer.cs"
// Author: Steffen Hanke
// Date: 2020-2024
//----------------------------------------------------------------------------------
using System;
using Solid.Dicom.ImageData;

namespace MeanSquareErrorImageCompare
{
    /// <inheritdoc />
    /// <summary>
    /// API:YES
    /// ICrossImageMeanSquareError
    /// Algorithm interface to calculate the MeanSquareError of 2 images
    /// </summary>
    public interface IMeanSquareErrorImageComparer : IDisposable
    {
        /// <summary>
        /// calculates the MeanSquareError of 2 images
        /// </summary>
        /// <param name="image1">the first image to compare</param>
        /// <param name="image2">the second image to compare</param>
        /// <returns>the MeanSqureError calculation result with warnings (if any) or errors</returns>
        /// <seealso cref="IMeanSquareErrorImageCompareResult">see also the description of the result structure</seealso>
        IMeanSquareErrorImageCompareResult CompareImages(IImageData image1, IImageData image2);
    }
}