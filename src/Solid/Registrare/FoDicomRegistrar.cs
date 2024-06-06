//----------------------------------------------------------------------------------
// File: "FoDicomRegistrar.cs"
// Author: Steffen Hanke
// Date: 2023-2024
//----------------------------------------------------------------------------------

using Solid.Dicom;
using Solid.DicomAdapters.FoDicom;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.DiContainer;

namespace Solid.Registrare
{
    public class FoDicomRegistrar : IDiRegistrar
    {
        public void Register(IDiContainer container)
        {
            ConsistencyCheck.EnsureArgument(container).IsNotNull();

            container.Register(new DicomRegistrar());
            container.RegisterType<IFoDicomDataSetProvider, Solid.DicomAdapters.FoDicom.Impl.FoDicomDataSetProvider>();
            container.RegisterCreator<IDicomDataSetProvider>((resolver) => resolver.Resolve<IFoDicomDataSetProvider>());
        }
    }
}
