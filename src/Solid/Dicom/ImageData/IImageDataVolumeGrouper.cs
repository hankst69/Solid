//----------------------------------------------------------------------------------
// File: "IImageDataVolumeGrouper.cs"
// Author: Steffen Hanke
// Date: 2019
//----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Solid.Dicom.ImageData
{
    /// <summary>
    /// IImageDataVolumeGrouper
    /// </summary>
    public interface IImageDataVolumeGrouper
    {
        /// <summary>
        /// * groups the input images into subsets where each subset should represent a single volume
        /// </summary>
        /// <remarks>
        /// * this grouping is only done based on geometrical attriobutes of the image planes and on matching matrix sisze and FiledOfView of the pixel data
        /// * if necessary make use of the <see cref="IImageDataClassifier"/> to reduce the input images to a specific class before calling this function
        /// </remarks>
        /// <param name="inputImages"></param>
        /// <returns></returns>
        IEnumerable<IEnumerable<IImageData>> GroupIntoVolumes(IEnumerable<IImageData> inputImages);

        /// <summary>
        /// * extracts subsets out of the input images where each subset represents all images belonging to a common 4D volume group (in syngo aka "dynamic")
        /// </summary>
        /// <remarks>
        /// * this grouping is only done based on geometrical attributes of the image planes and on matching matrix size and FiledOfView of the pixel data
        /// * if necessary make use of the <see cref="IImageDataClassifier"/> to reduce the input images to a specific class before calling this function
        /// * the result can be used as input of <see cref="IImageDataVolumeGrouper.GroupIntoVolumes"/> to split it into the separate 3d volumes (in syngo aka "phases")
        /// </remarks>
        /// <param name="inputImages"></param>
        /// <returns></returns>
        IEnumerable<IEnumerable<IImageData>> Extract4dVolumes(IEnumerable<IImageData> inputImages);
    }
}
