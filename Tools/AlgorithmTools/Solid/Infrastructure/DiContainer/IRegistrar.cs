//----------------------------------------------------------------------------------
// <copyright file="IRegistrar.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IRegistrar
    /// </summary>
    public interface IRegistrar
    {
        void Register(IDiContainer container);
    }
}
