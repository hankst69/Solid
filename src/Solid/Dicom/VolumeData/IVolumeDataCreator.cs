//----------------------------------------------------------------------------------
// File: "IVolumeDataCreator.cs"
// Author: Steffen Hanke
// Date: 2019
//----------------------------------------------------------------------------------

using System.Collections.Generic;
using Solid.Dicom.ImageData;

namespace Solid.Dicom.VolumeData
{
    /// <summary>
    /// IVolumeDataCreator
    /// </summary>
    public interface IVolumeDataCreator
    {
        IVolumeData CreateVolumeData(IEnumerable<IImageData> inputImages);

        IVolumeData CreateVolumeData(IEnumerable<IDicomFrameDataSet> inputImages);
    }
}
