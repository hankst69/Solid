//----------------------------------------------------------------------------------
// <copyright file="IDiContainer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IDiContainer
    /// </summary>
    public interface IDiContainer : IDisposable, IDiRegister, IDiResolve, IDiIsRegistered, IDiContainerSpecial
    {
    }
}