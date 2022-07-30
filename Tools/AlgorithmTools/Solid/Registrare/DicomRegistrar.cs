//----------------------------------------------------------------------------------
// <copyright file="DicomRegistrar.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using Solid.Dicom.ImageData;
using Solid.Dicom.ImageData.Impl;
using Solid.Dicom.Impl;
using Solid.Dicom.VolumeData;
using Solid.Dicom.VolumeData.Impl;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.DiContainer;

namespace Solid.Dicom
{
    public class DicomRegistrar: IRegistrar
    {
        public void Register(IDiContainer container)
        {
            ConsistencyCheck.EnsureArgument(container).IsNotNull();

            container.RegisterType<IMrDicomAccess, MrDicomAccess>();
            //container.RegisterType<IImageDataClassifier, ImageDataClassifier>();
            container.RegisterType<IImageDataCreator, ImageDataCreator>();
            container.RegisterType<IImageDataVolumeValidator, ImageDataVolumeValidator>();
            container.RegisterType<IImageDataVolumeGrouper, ImageDataVolumeGrouper>();
            container.RegisterType<IVolumeDataCreator, VolumeDataCreator>();
        }
    }
}
