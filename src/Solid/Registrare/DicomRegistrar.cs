//----------------------------------------------------------------------------------
// File: "DicomRegistrar.cs"
// Author: Steffen Hanke
// Date: 2023-2024
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

            container.RegisterType<IDicomFrameDataSetCreator, Solid.Dicom.Impl.DicomFrameDataSetCreator>();

            container.RegisterType<IMrDicomAccess, Solid.Dicom.Impl.MrDicomAccess>();

            //container.RegisterType<IImageDataClassifier, Solid.Dicom.ImageData.Impl.ImageDataClassifier>();
            container.RegisterType<IImageDataCreator, Solid.Dicom.ImageData.Impl.ImageDataCreator>();
            container.RegisterType<IImageDataVolumeValidator, Solid.Dicom.ImageData.Impl.ImageDataVolumeValidator>();
            container.RegisterType<IImageDataVolumeGrouper, Solid.Dicom.ImageData.Impl.ImageDataVolumeGrouper>();
            container.RegisterType<IVolumeDataCreator, Solid.Dicom.VolumeData.Impl.VolumeDataCreator>();
        }
    }
}
