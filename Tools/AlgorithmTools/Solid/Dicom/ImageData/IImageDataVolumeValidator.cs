//----------------------------------------------------------------------------------
// <copyright file="IVolumeDataCreator.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Solid.Dicom.ImageData
{
    /// <summary>
    /// IImageDataVolumeValidator
    /// </summary>
    public interface IImageDataVolumeValidator
    {
        bool ValidateVolume(IEnumerable<IImageData> inputImages);
    }
}
