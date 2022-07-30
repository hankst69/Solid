//----------------------------------------------------------------------------------
// <copyright file="IDiContainer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IDiContainer
    /// </summary>
    public interface IDiContainer : IResolver, IDisposable
    {
        void RegisterInstance<TTypeToResolve>(object instance);

        void RegisterType<TTypeToResolve, TConcrete>();
        void RegisterTypeAsTransient<TTypeToResolve, TConcrete>();

        void RegisterCreator<TTypeToResolve>(Func<IResolver, object> creator);
        void RegisterCreatorAsTransient<TTypeToResolve>(Func<IResolver, object> creator);

        void Register(IRegistrar registrar);
    }
}