//----------------------------------------------------------------------------------
// <copyright file="BasicInfrastructureDiContainer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure
{
    public class BasicInfrastructureDiContainer : DiContainer.Impl.DiContainer
    {
        public BasicInfrastructureDiContainer()
        {
            this.Register(new BasicInfrastructureRegistrar());
        }
    }
}
