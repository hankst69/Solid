//----------------------------------------------------------------------------------
// <copyright file="DiContainerTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Moq;
using Solid.Infrastructure.DiContainer;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.DiContainer
{
    public partial class DiContainerTests
    {
        private IDiContainer _target;

        [SetUp]
        public void SetUp()
        {
            _target = new Infrastructure.DiContainer.Impl.DiContainer();
        }

        [Test]
        public void Register_IRegistrar_ShouldCallRegisterAtRegistrar()
        {
            // Arrange
            var registrarMock = new Mock<IDiRegistrar>();
            // Act
            _target.Register(registrarMock.Object);
            // Assert
            registrarMock.Verify(x => x.Register(It.Is<IDiContainer>(c => c == _target)), Times.Once);
        }

        [Test]
        public void ResolveAllImplementing_ShouldReturnCorrectInstances()
        {
            // Arrange
            _target.RegisterInstance<ITypeToResolve>(new ConcreteType());
            _target.RegisterInstance<ITypeNotToResolve>(new ConcreteTypeNotToResolve());
            _target.RegisterInstance<ITypeToResolveDerived>(new ConcreteTypeDerived());
            // Act
            var allResolvedTypes = _target.ResolveAllImplementing<ITypeToResolve>().ToList();
            // Assert
            allResolvedTypes.Count().Should().Be(2);
            allResolvedTypes.ForEach(x => x.Should().BeAssignableTo<ITypeToResolve>());
        }
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