//----------------------------------------------------------------------------------
// <copyright file="DicomRegistrar.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using Solid.Dicom;
using Solid.Dicom.ImageData;
using Solid.Dicom.VolumeData;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.DiContainer;

namespace Solid.Registrare
{
    public class DicomRegistrar: IDiRegistrar
    {
        public void Register(IDiContainer container)
        {
            ConsistencyCheck.EnsureArgument(container).IsNotNull();

            container.RegisterType<IMrDicomAccess, Solid.Dicom.Impl.MrDicomAccess>();
            //container.RegisterType<IImageDataClassifier, Solid.Dicom.ImageData.Impl.ImageDataClassifier>();
            container.RegisterType<IImageDataCreator, Solid.Dicom.ImageData.Impl.ImageDataCreator>();
            container.RegisterType<IImageDataVolumeValidator, Solid.Dicom.ImageData.Impl.ImageDataVolumeValidator>();
            container.RegisterType<IImageDataVolumeGrouper, Solid.Dicom.ImageData.Impl.ImageDataVolumeGrouper>();
            container.RegisterType<IVolumeDataCreator, Solid.Dicom.VolumeData.Impl.VolumeDataCreator>();
        }
    }
}
