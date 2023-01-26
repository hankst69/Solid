//----------------------------------------------------------------------------------
// <copyright file="FoDicomRegistrar.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
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
