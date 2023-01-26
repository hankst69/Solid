//----------------------------------------------------------------------------------
// <copyright file="InfrastructureDiContainer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2022-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure
{
    public class InfrastructureDiContainer : DiContainer.Impl.DiContainer
    {
        public InfrastructureDiContainer()
        {
            this.Register(new InfrastructureRegistrar());
        }
    }
}
