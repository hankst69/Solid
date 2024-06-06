//----------------------------------------------------------------------------------
// File: "DiContainerTestBase.cs"
// Author: Steffen Hanke
// Date: 2017-2023
//----------------------------------------------------------------------------------

using Solid.Infrastructure.DiContainer;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.DiContainer
{
    public class DiContainerTestBase
    {
        protected IDiContainer _target;

        [SetUp]
        public void SetUp()
        {
            _target = new Infrastructure.DiContainer.Impl.DiContainer();
        }

        public interface ITypeToResolve
        {
        }

        public interface ITypeToResolveDerived : ITypeToResolve
        {
        }

        public interface ITypeNotToResolve
        {
        }

        public class ConcreteType : ITypeToResolve
        {
        }

        public class ConcreteTypeDerived : ITypeToResolveDerived
        {
        }

        public class ConcreteTypeNotToResolve : ITypeNotToResolve
        {
        }

        public interface ITypeToResolveWithConstructorParams {}
        public class ConcreteTypeWithConstructorParams : ITypeToResolveWithConstructorParams
        {
            public ConcreteTypeWithConstructorParams(ITypeToResolve typeToResolve)
            {
            }
        }

        public interface ITypeToResolveWithDefaultConstructorParams { }
        public class ConcreteTypeWithDefaultConstructorParams : ITypeToResolveWithDefaultConstructorParams
        {
            public ConcreteTypeWithDefaultConstructorParams(ITypeToResolve typeToResolve = null)
            {
            }
        }

        public interface  ITypeToResolveWithConstructorParamSelfRegistered {}
        public class ConcreteTypeWithConstructorParamSelfRegistered : ITypeToResolveWithConstructorParamSelfRegistered
        {
            public ConcreteTypeWithConstructorParamSelfRegistered(ITypeToResolveWithConstructorParamSelfRegistered selfArgument)
            {
            }
        }

        public interface ITypeToResolveWithConstructorParamSelfConcrete { }
        public class ConcreteTypeWithConstructorParamSelfConcrete : ITypeToResolveWithConstructorParamSelfConcrete
        {
            public ConcreteTypeWithConstructorParamSelfConcrete(ConcreteTypeWithConstructorParamSelfConcrete selfArgument)
            {
            }
        }

        public interface ITypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered { }
        public class ConcreteTypeWithCircularDependencyOfConstructorParamToSelfRegistered 
            : ITypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered
        {
            public ConcreteTypeWithCircularDependencyOfConstructorParamToSelfRegistered(
                ITypeToResolveWithParameterOfTypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered param)
            {
            }
        }

        public interface ITypeToResolveWithParameterOfTypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered { }
        public class ConcreteTypeWithParameterOfTypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered 
            : ITypeToResolveWithParameterOfTypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered
        {
            public ConcreteTypeWithParameterOfTypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered(
                ITypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered param)
            {
            }
        }

        public interface ITypeToResolveWithCircularDependencyOfConstructorParamToSelfConcrete { }
        public class ConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete 
            : ITypeToResolveWithCircularDependencyOfConstructorParamToSelfConcrete
        {
            public ConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete(
                ITypeToResolveWithParameterOfConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete param)
            {
            }
        }

        public interface ITypeToResolveWithParameterOfConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete { }
        public class ConcreteTypeWithParameterOfConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete 
            : ITypeToResolveWithParameterOfConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete
        {
            public ConcreteTypeWithParameterOfConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete(
                ConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete param)
            {
            }
        }
    }
}