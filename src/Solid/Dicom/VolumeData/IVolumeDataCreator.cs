//----------------------------------------------------------------------------------
// <copyright file="IVolumeDataCreator.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
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
