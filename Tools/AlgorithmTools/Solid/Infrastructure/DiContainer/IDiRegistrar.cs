//----------------------------------------------------------------------------------
// <copyright file="IDiRegistrar.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IDiRegistrar
    /// </summary>
    public interface IDiRegistrar
    {
        void Register(IDiContainer container);
    }
}
