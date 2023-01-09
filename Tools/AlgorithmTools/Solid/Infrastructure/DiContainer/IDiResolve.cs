//----------------------------------------------------------------------------------
// <copyright file="IDiResolve.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IDiResolve
    /// </summary>
    public interface IDiResolve
    {
        TTypeToResolve Resolve<TTypeToResolve>();

        TTypeToResolve TryResolve<TTypeToResolve>();
    }
}
