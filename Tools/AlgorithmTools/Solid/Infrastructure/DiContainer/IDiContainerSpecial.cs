//----------------------------------------------------------------------------------
// <copyright file="IDiContainerSpecial.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IDiContainerSpecial
    /// </summary>
    public interface IDiContainerSpecial
    {
        ///<summary>accepts an instance of type IDiRegistrar that then can register multiple related types at the DiContainer</summary>
        ///<param name="registrar"></param>
        void Register(IDiRegistrar registrar);

        ///<summary>resolves all objects that implement the given type (used e.g. in BootStrapper to resolve all registrations that oimplement IBootable)</summary>
        IEnumerable<TTypeToResolve> ResolveAllImplementing<TTypeToResolve>();

        /////<summary>accepts an collection of IDiRegistrar instances to register blocks of related types at the DiContainer</summary>
        /////<param name="registrars"></param>
        //void Register(IEnumerable<IDiRegistrar> registrars);
        /////<summary>accepts an instance of type IDiRegistrar that then can register multiple related types at the DiContainer</summary>
        /////<remarks>if a type is already registered at the container the registrars new registration will be ignored</remarks>
        /////<param name="registrar"></param>
        //void RegisterWithoutOverwrite(IDiRegistrar registrar);
        /////<summary>accepts an collection of IDiRegistrar instances to register blocks of related types at the DiContainer</summary>
        /////<param name="registrars"></param>
        //void RegisterWithoutOverwrite(IEnumerable<IDiRegistrar> registrars);
    }
}
