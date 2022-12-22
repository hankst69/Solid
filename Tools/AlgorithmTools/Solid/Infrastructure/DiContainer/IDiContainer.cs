//----------------------------------------------------------------------------------
// <copyright file="IDiContainer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;

using static Solid.Infrastructure.DiContainer.Impl.DiContainer;

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IDiContainer
    /// </summary>
    public interface IDiContainer : IResolver, IDisposable
    {
        ///<summary>accepts an instance of type IRegistrar that then can register multiple related types at the DiContainer</summary>
        ///<param name="registrar"></param>
        void Register(IRegistrar registrar);

        ///<summary>checks if type is registered with container (optionally with specific registration name)</summary>
        bool IsTypeRegistered<TTypeToResolve>(string name = null);

        ///<summary>checks if any registration exists that implements the given type (optionally with specific registration name)</summary>
        bool IsTypeImplementationRegistered<TTypeToResolve>(string name = null);

        ///<summary>registers an existing instance to be resolvable for the given type TTypeToResolve</summary>
        ///<remarks>
        ///the lifecycle type of this registration is implicitely LifeCycle.Singleton
        ///todo: in future this function could proof it the instance really implements the TTypeToResolve
        ///</remarks>
        void RegisterInstance<TTypeToResolve>(object instance);

        void RegisterType<TTypeToResolve, TConcrete>();

        void RegisterTypeAsTransient<TTypeToResolve, TConcrete>();

        void RegisterCreator<TTypeToResolve>(Func<IResolver, object> creator);
        void RegisterCreator<TTypeToResolve>(Func<IResolver, Type, object> creator);

        void RegisterCreatorAsTransient<TTypeToResolve>(Func<IResolver, object> creator);
        void RegisterCreatorAsTransient<TTypeToResolve>(Func<IResolver, Type, object> creator);
    }
}