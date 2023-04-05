//----------------------------------------------------------------------------------
// <copyright file="IDiIsRegistered.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IDiIsRegistered
    /// </summary>
    public interface IDiIsRegistered
    {
        ///<summary>checks if type is registered with container</summary>
        bool IsRegistered<TTypeToResolve>();

        ///<summary>checks if any registration exists that implements the given type</summary>
        bool IsRegisteredAnyImplementing<TTypeToResolve>();
    }
}
