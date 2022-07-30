//----------------------------------------------------------------------------------
// <copyright file="DiContainerTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2021. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
//using Microsoft.Practices.Unity;
using Moq;
using Solid.Infrastructure.DiContainer;
using Solid.Infrastructure.DiContainer.Impl;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.DiContainer
{
    public class DiContainerTests
    {
        private IDiContainer m_Target;

        [SetUp]
        public void SetUp()
        {
            m_Target = new Infrastructure.DiContainer.Impl.DiContainer();
        }

        [Test]
        public void RegisterInstance_ShouldThrow_WhenInstanceIsNull()
        {
            // Arrange
            // Act
            Action action = () => m_Target.RegisterInstance<ITypeToResolve>(null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Resolve_ShouldThrow_WhenTypeIsNotRegistered()
        {
            // Arrange
            // Act
            Action action = () => m_Target.Resolve<ITypeToResolve>();

            // Assert
            action.Should().Throw<TypeNotRegisteredException>();
        }

        [Test]
        public void TryResolve_ShouldReturnNull_WhenTypeIsNotRegistered()
        {
            // Arrange
            // Act
            var instance = m_Target.TryResolve<ITypeToResolve>();

            // Assert
            instance.Should().BeNull();
        }

        [Test]
        public void Resolve_ShouldThrow_WhenConcreteTypeIsRegisteredButConstructorParametersAreNotRegistered()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolveWithConstructorParams, ConcreteTypeWithConstructorParams>();

            // Act
            Action action = () => m_Target.Resolve<ITypeToResolveWithConstructorParams>();

            // Assert
            action.Should().Throw<TypeNotRegisteredException>();
        }

        [Test]
        public void TryResolve_ShouldThrow_WhenConcreteTypeIsRegisteredButConstructorParametersAreNotRegistered()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolveWithConstructorParams, ConcreteTypeWithConstructorParams>();

            // Act
            Action action = () => m_Target.TryResolve<ITypeToResolveWithConstructorParams>();

            // Assert
            action.Should().Throw<TypeNotRegisteredException>();
        }

        [Test]
        public void Resolve_ShouldNotThrow_WhenConcreteTypeWithDefaultConstructorParamsIsRegisteredButConstructorParametersAreNotRegistered()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolveWithDefaultConstructorParams, ConcreteTypeWithDefaultConstructorParams>();

            // Act
            var instance = m_Target.Resolve<ITypeToResolveWithDefaultConstructorParams>();

            // Assert
            instance.Should().BeOfType<ConcreteTypeWithDefaultConstructorParams>();
        }

        [Test]
        public void Resolve_ShouldThrow_WhenConstructorParametersDependOnSelfConcreteType()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolveWithConstructorParamSelfConcrete, ConcreteTypeWithConstructorParamSelfConcrete>();

            // Act
            //var result = new ConcreteTypeWithConstructorParamSelfConcrete(null);
            Action action = () => m_Target.Resolve<ITypeToResolveWithConstructorParamSelfConcrete>();

            // Assert
            action.Should().Throw<CircularDependencyException>();
        }

        [Test]
        public void Resolve_ShouldThrow_WhenConstructorParametersDependOnSelfRegisteredType()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolveWithConstructorParamSelfRegistered, ConcreteTypeWithConstructorParamSelfRegistered>();

            // Act
            //var result = new ConcreteTypeWithConstructorParamSelfRegistered(null);
            Action action = () => m_Target.Resolve<ITypeToResolveWithConstructorParamSelfRegistered>();

            // Assert
            action.Should().Throw<CircularDependencyException>();
        }

        [Test]
        public void Resolve_ShouldThrow_WhenCircularDependencyOfConstructorParametersToSelfRegisteredType()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered, ConcreteTypeWithCircularDependencyOfConstructorParamToSelfRegistered>();
            m_Target.RegisterType<ITypeToResolveWithParameterOfTypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered, ConcreteTypeWithParameterOfTypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered>();
   
            // Act
            Action action = () => m_Target.Resolve<ITypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered>();

            // Assert
            action.Should().Throw<CircularDependencyException>();
        }

        [Test]
        public void Resolve_ShouldThrow_WhenCircularDependencyOfConstructorParametersToSelfConcreteType()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolveWithCircularDependencyOfConstructorParamToSelfConcrete, ConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete>();
            m_Target.RegisterType<ITypeToResolveWithParameterOfConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete, ConcreteTypeWithParameterOfConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete>();

            // Act
            Action action = () => m_Target.Resolve<ITypeToResolveWithCircularDependencyOfConstructorParamToSelfConcrete>();

            // Assert
            action.Should().Throw<CircularDependencyException>();
        }

        [Test]
        public void Resolve_ShouldResolveConcreteType_WhenInstanceIsRegistered()
        {
            // Arrange
            m_Target.RegisterInstance<ITypeToResolve>(new ConcreteType());

            // Act
            var instance = m_Target.Resolve<ITypeToResolve>();

            // Assert
            instance.Should().BeOfType<ConcreteType>();
        }

        [Test]
        public void TryResolve_ShouldResolveConcreteType_WhenInstanceIsRegistered()
        {
            // Arrange
            m_Target.RegisterInstance<ITypeToResolve>(new ConcreteType());

            // Act
            var instance = m_Target.TryResolve<ITypeToResolve>();

            // Assert
            instance.Should().BeOfType<ConcreteType>();
        }

        [Test]
        public void Resolve_ShouldResolveConcreteType_WhenConcreteTypeIsRegistered()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolve, ConcreteType>();

            // Act
            var instance = m_Target.Resolve<ITypeToResolve>();

            // Assert
            instance.Should().BeOfType<ConcreteType>();
        }

        [Test]
        public void TryResolve_ShouldResolveConcreteType_WhenConcreteTypeIsRegistered()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolve, ConcreteType>();

            // Act
            var instance = m_Target.TryResolve<ITypeToResolve>();

            // Assert
            instance.Should().BeOfType<ConcreteType>();
        }

        [Test]
        public void Resolve_ShouldResolveConcreteType_WhenConcreteTypeAndConstructorParametersAreRegistered()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolve, ConcreteType>();
            m_Target.RegisterType<ITypeToResolveWithConstructorParams, ConcreteTypeWithConstructorParams>();

            // Act
            var instance = m_Target.Resolve<ITypeToResolveWithConstructorParams>();

            // Assert
            instance.Should().BeOfType<ConcreteTypeWithConstructorParams>();
        }

        [Test]
        public void TryResolve_ShouldResolveConcreteType_WhenConcreteTypeAndConstructorParametersAreRegistered()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolve, ConcreteType>();
            m_Target.RegisterType<ITypeToResolveWithConstructorParams, ConcreteTypeWithConstructorParams>();

            // Act
            var instance = m_Target.TryResolve<ITypeToResolveWithConstructorParams>();

            // Assert
            instance.Should().BeOfType<ConcreteTypeWithConstructorParams>();
        }

        // Resolve test for Creator registration is deactivated because this scenario is already covered by the several RegisterCreator... tests
        //[Test]
        //public void Resolve_ShouldResolveConcreteType_WhenCreatorIsRegistered()
        //{
        //    // Arrange
        //    m_Target.RegisterCreator<ITypeToResolve>(r => new ConcreteType());
        //    // Act
        //    var instance = m_Target.Resolve<ITypeToResolve>();
        //    // Assert
        //    instance.Should().BeOfType<ConcreteType>();
        //}

        [Test]
        public void TryResolve_ShouldResolveConcreteType_WhenCreatorIsRegistered()
        {
            // Arrange
            m_Target.RegisterCreator<ITypeToResolve>(r => new ConcreteType());
            // Act
            var instance = m_Target.TryResolve<ITypeToResolve>();
            // Assert
            instance.Should().BeOfType<ConcreteType>();
        }

        [Test]
        public void RegisterInstance_ShouldResolveSingleton()
        {
            // Arrange
            // Act
            var instance = new ConcreteType();
            m_Target.RegisterInstance<ITypeToResolve>(instance);

            // Assert
            var instance1 = m_Target.Resolve<ITypeToResolve>();
            instance1.Should().BeSameAs(instance);
            var instance2 = m_Target.Resolve<ITypeToResolve>();
            instance2.Should().BeSameAs(instance);
        }

        [Test]
        public void RegisterType_ShouldResolveSingleton()
        {
            // Arrange
            // Act
            m_Target.RegisterType<ITypeToResolve, ConcreteType>();

            // Assert
            var instance1 = m_Target.Resolve<ITypeToResolve>();
            var instance2 = m_Target.Resolve<ITypeToResolve>();
            instance1.Should().BeSameAs(instance2);
        }

        [Test]
        public void RegisterTypeAsTransient_ShouldResolveTransient()
        {
            // Arrange
            // Act
            m_Target.RegisterTypeAsTransient<ITypeToResolve, ConcreteType>();

            // Assert
            var instance1 = m_Target.Resolve<ITypeToResolve>();
            var instance2 = m_Target.Resolve<ITypeToResolve>();
            instance1.Should().NotBeSameAs(instance2);
        }

        [Test]
        public void RegisterCreator_ShouldResolveSingleton()
        {
            // Arrange
            // Act
            m_Target.RegisterCreator<ITypeToResolve>(creator => new ConcreteType());
            // Assert
            var instance1 = m_Target.Resolve<ITypeToResolve>();
            var instance2 = m_Target.Resolve<ITypeToResolve>();
            instance1.Should().BeSameAs(instance2);
        }

        [Test]
        public void RegisterCreatorAsTransient_ShouldResolveTransient()
        {
            // Arrange
            // Act
            m_Target.RegisterCreatorAsTransient<ITypeToResolve>(r => new ConcreteType());
            // Assert
            var instance1 = m_Target.Resolve<ITypeToResolve>();
            var instance2 = m_Target.Resolve<ITypeToResolve>();
            instance1.Should().NotBeSameAs(instance2);
        }

        [Test]
        public void RegisterCreator_ShouldResolveSingleton_WhenCreatorForTypeWithConstructorParametersIsRegistered()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolve, ConcreteType>();
            // Act
            m_Target.RegisterCreator<ITypeToResolveWithConstructorParams>(r => new ConcreteTypeWithConstructorParams(r.Resolve<ITypeToResolve>()));
            // Assert
            var instance1 = m_Target.Resolve<ITypeToResolveWithConstructorParams>();
            var instance2 = m_Target.Resolve<ITypeToResolveWithConstructorParams>();
            instance1.Should().BeSameAs(instance2);
        }

        [Test]
        public void RegisterCreatorAsTransient_ShouldResolveTransient_WhenCreatorForTypeWithConstructorParametersIsRegistered()
        {
            // Arrange
            m_Target.RegisterType<ITypeToResolve, ConcreteType>();
            // Act
            m_Target.RegisterCreatorAsTransient<ITypeToResolveWithConstructorParams>(r => new ConcreteTypeWithConstructorParams(r.Resolve<ITypeToResolve>()));
            // Assert
            var instance1 = m_Target.Resolve<ITypeToResolveWithConstructorParams>();
            var instance2 = m_Target.Resolve<ITypeToResolveWithConstructorParams>();
            instance1.Should().NotBeSameAs(instance2);
        }

        [Test]
        public void Register_ShouldCallRegisterAtRegistrar()
        {
            // Arrange
            var registrarMock = new Mock<IRegistrar>();
            // Act
            m_Target.Register(registrarMock.Object);
            // Assert
            registrarMock.Verify(x => x.Register(It.Is<IDiContainer>(c => c == m_Target)), Times.Once);
        }

        [Test]
        public void ResolveAllImplementing_ShouldReturnCorrectInstances()
        {
            // Arrange
            m_Target.RegisterInstance<ITypeToResolve>(new ConcreteType());
            m_Target.RegisterInstance<ITypeNotToResolve>(new ConcreteTypeNotToResolve());
            m_Target.RegisterInstance<ITypeToResolveDerived>(new ConcreteTypeDerived());

            // Act
            var allResolvedTypes = m_Target.ResolveAllImplementing<ITypeToResolve>().ToList();

            // Assert
            allResolvedTypes.Count().Should().Be(2);
            allResolvedTypes.ForEach(x => x.Should().BeAssignableTo<ITypeToResolve>());
        }

        //[Test]
        //public void ResolveAll_OfUnityContainer_ShouldReturnCorrectInstances_WhenRegisteredAsInstance()
        //{
        //    // Arrange
        //    var container = new UnityContainer();
        //    container.RegisterInstance<ITypeToResolve>(new ConcreteType());
        //    container.RegisterInstance<ITypeToResolve>("A", new ConcreteType());
        //    container.RegisterInstance<ITypeToResolve>("B", new ConcreteType());
        //    container.RegisterInstance<ITypeNotToResolve>("C", new ConcreteTypeNotToResolve());
        //    container.RegisterInstance<ITypeToResolveDerived>("D", new ConcreteTypeDerived());

        //    // Act
        //    //var resolved = container.Resolve<ITypeToResolve>();
        //    //var resolved = container.Resolve<ITypeToResolve>("B");
        //    var allResolvedTypes = container.ResolveAll<ITypeToResolve>().ToList();

        //    // Assert
        //    allResolvedTypes.Count().Should().Be(2);
        //    allResolvedTypes.ForEach(x => x.Should().BeAssignableTo<ITypeToResolve>());
        //}

        //[Test]
        //public void ResolveAll_OfUnityContainer_ShouldReturnCorrectInstances_WhenRegisteredAsType()
        //{
        //    // Arrange
        //    var container = new UnityContainer();
        //    container.RegisterType<ITypeToResolve, ConcreteType>();
        //    container.RegisterType<ITypeToResolve, ConcreteType>("A");
        //    container.RegisterType<ITypeToResolve, ConcreteType>("B");
        //    container.RegisterType<ITypeNotToResolve, ConcreteTypeNotToResolve>("C");
        //    container.RegisterType<ITypeToResolveDerived, ConcreteTypeDerived>("D");

        //    // Act
        //    //var resolved = container.Resolve<ITypeToResolve>();
        //    //var resolved = container.Resolve<ITypeToResolve>("B");
        //    var allResolvedTypes = container.ResolveAll<ITypeToResolve>().ToList();

        //    // Assert
        //    allResolvedTypes.Count().Should().Be(2);
        //    allResolvedTypes.ForEach(x => x.Should().BeAssignableTo<ITypeToResolve>());
        //}
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