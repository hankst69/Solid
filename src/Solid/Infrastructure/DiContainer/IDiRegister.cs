//----------------------------------------------------------------------------------
// File: "IDiRegister.cs"
// Author: Steffen Hanke
// Date: 2022-2023
//----------------------------------------------------------------------------------
using System;

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IDiRegister
    /// </summary>
    public interface IDiRegister
    {
        ///<summary>registers an existing instance to be resolvable for the given type TTypeToResolve</summary>
        ///<remarks>
        ///the lifecycle type of this registration is implicitely LifeCycle.Singleton
        ///todo: in future this function could proof it the instance really implements the TTypeToResolve
        ///</remarks>
        void RegisterInstance<TTypeToResolve>(object instance);

        void RegisterType<TTypeToResolve, TConcrete>();

        void RegisterTypeAsTransient<TTypeToResolve, TConcrete>();

        void RegisterCreator<TTypeToResolve>(Func<IDiResolve, object> creator);

        void RegisterCreator<TTypeToResolve>(Func<IDiResolve, Type, object> creator);

        void RegisterCreatorAsTransient<TTypeToResolve>(Func<IDiResolve, object> creator);

        void RegisterCreatorAsTransient<TTypeToResolve>(Func<IDiResolve, Type, object> creator);
    }
}
