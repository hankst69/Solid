//----------------------------------------------------------------------------------
// <copyright file="IResolver.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2021. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IResolver
    /// </summary>
    public interface IResolver
    {
        TTypeToResolve Resolve<TTypeToResolve>();
        TTypeToResolve TryResolve<TTypeToResolve>();
        IEnumerable<TTypeToResolve> ResolveAllImplementing<TTypeToResolve>();
    }
}
