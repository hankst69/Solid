//----------------------------------------------------------------------------------
// <copyright file="DiContainer_ResolveTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using FluentAssertions;
using NUnit.Framework;
using Solid.Infrastructure.DiContainer.Impl;

namespace Solid.Infrastructure_uTest.DiContainer
{
    public class DiContainer_ResolveTests : DiContainerTestBase
    {
        [Test]
        public void Resolve_ShouldThrow_WhenTypeIsNotRegistered()
        {
            // Arrange
            // Act
            Action action = () => _target.Resolve<ITypeToResolve>();

            // Assert
            action.Should().Throw<TypeNotRegisteredException>();
        }

        [Test]
        public void TryResolve_ShouldReturnNull_WhenTypeIsNotRegistered()
        {
            // Arrange
            // Act
            var instance = _target.TryResolve<ITypeToResolve>();

            // Assert
            instance.Should().BeNull();
        }

        [Test]
        public void Resolve_ShouldThrow_WhenConcreteTypeIsRegisteredButConstructorParametersAreNotRegistered()
        {
            // Arrange
            _target.RegisterType<ITypeToResolveWithConstructorParams, ConcreteTypeWithConstructorParams>();

            // Act
            Action action = () => _target.Resolve<ITypeToResolveWithConstructorParams>();

            // Assert
            action.Should().Throw<TypeNotRegisteredException>();
        }

        [Test]
        public void TryResolve_ShouldThrow_WhenConcreteTypeIsRegisteredButConstructorParametersAreNotRegistered()
        {
            // Arrange
            _target.RegisterType<ITypeToResolveWithConstructorParams, ConcreteTypeWithConstructorParams>();

            // Act
            Action action = () => _target.TryResolve<ITypeToResolveWithConstructorParams>();

            // Assert
            action.Should().Throw<TypeNotRegisteredException>();
        }

        [Test]
        public void Resolve_ShouldNotThrow_WhenConcreteTypeWithDefaultConstructorParamsIsRegisteredButConstructorParametersAreNotRegistered()
        {
            // Arrange
            _target.RegisterType<ITypeToResolveWithDefaultConstructorParams, ConcreteTypeWithDefaultConstructorParams>();

            // Act
            var instance = _target.Resolve<ITypeToResolveWithDefaultConstructorParams>();

            // Assert
            instance.Should().BeOfType<ConcreteTypeWithDefaultConstructorParams>();
        }

        [Test]
        public void Resolve_ShouldThrow_WhenConstructorParametersDependOnSelfConcreteType()
        {
            // Arrange
            _target.RegisterType<ITypeToResolveWithConstructorParamSelfConcrete, ConcreteTypeWithConstructorParamSelfConcrete>();

            // Act
            //var result = new ConcreteTypeWithConstructorParamSelfConcrete(null);
            Action action = () => _target.Resolve<ITypeToResolveWithConstructorParamSelfConcrete>();

            // Assert
            action.Should().Throw<CircularDependencyException>();
        }

        [Test]
        public void Resolve_ShouldThrow_WhenConstructorParametersDependOnSelfRegisteredType()
        {
            // Arrange
            _target.RegisterType<ITypeToResolveWithConstructorParamSelfRegistered, ConcreteTypeWithConstructorParamSelfRegistered>();

            // Act
            //var result = new ConcreteTypeWithConstructorParamSelfRegistered(null);
            Action action = () => _target.Resolve<ITypeToResolveWithConstructorParamSelfRegistered>();

            // Assert
            action.Should().Throw<CircularDependencyException>();
        }

        [Test]
        public void Resolve_ShouldThrow_WhenCircularDependencyOfConstructorParametersToSelfRegisteredType()
        {
            // Arrange
            _target.RegisterType<ITypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered, ConcreteTypeWithCircularDependencyOfConstructorParamToSelfRegistered>();
            _target.RegisterType<ITypeToResolveWithParameterOfTypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered, ConcreteTypeWithParameterOfTypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered>();

            // Act
            Action action = () => _target.Resolve<ITypeToResolveWithCircularDependencyOfConstructorParamToSelfRegistered>();

            // Assert
            action.Should().Throw<CircularDependencyException>();
        }

        [Test]
        public void Resolve_ShouldThrow_WhenCircularDependencyOfConstructorParametersToSelfConcreteType()
        {
            // Arrange
            _target.RegisterType<ITypeToResolveWithCircularDependencyOfConstructorParamToSelfConcrete, ConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete>();
            _target.RegisterType<ITypeToResolveWithParameterOfConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete, ConcreteTypeWithParameterOfConcreteTypeWithCircularDependencyOfConstructorParamToSelfConcrete>();

            // Act
            Action action = () => _target.Resolve<ITypeToResolveWithCircularDependencyOfConstructorParamToSelfConcrete>();

            // Assert
            action.Should().Throw<CircularDependencyException>();
        }

        [Test]
        public void Resolve_ShouldResolveConcreteType_WhenInstanceIsRegistered()
        {
            // Arrange
            _target.RegisterInstance<ITypeToResolve>(new ConcreteType());

            // Act
            var instance = _target.Resolve<ITypeToResolve>();

            // Assert
            instance.Should().BeOfType<ConcreteType>();
        }

        [Test]
        public void TryResolve_ShouldResolveConcreteType_WhenInstanceIsRegistered()
        {
            // Arrange
            _target.RegisterInstance<ITypeToResolve>(new ConcreteType());

            // Act
            var instance = _target.TryResolve<ITypeToResolve>();

            // Assert
            instance.Should().BeOfType<ConcreteType>();
        }

        [Test]
        public void Resolve_ShouldResolveConcreteType_WhenConcreteTypeIsRegistered()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();

            // Act
            var instance = _target.Resolve<ITypeToResolve>();

            // Assert
            instance.Should().BeOfType<ConcreteType>();
        }

        [Test]
        public void TryResolve_ShouldResolveConcreteType_WhenConcreteTypeIsRegistered()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();

            // Act
            var instance = _target.TryResolve<ITypeToResolve>();

            // Assert
            instance.Should().BeOfType<ConcreteType>();
        }

        [Test]
        public void Resolve_ShouldResolveConcreteType_WhenConcreteTypeAndConstructorParametersAreRegistered()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();
            _target.RegisterType<ITypeToResolveWithConstructorParams, ConcreteTypeWithConstructorParams>();

            // Act
            var instance = _target.Resolve<ITypeToResolveWithConstructorParams>();

            // Assert
            instance.Should().BeOfType<ConcreteTypeWithConstructorParams>();
        }

        [Test]
        public void TryResolve_ShouldResolveConcreteType_WhenConcreteTypeAndConstructorParametersAreRegistered()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();
            _target.RegisterType<ITypeToResolveWithConstructorParams, ConcreteTypeWithConstructorParams>();

            // Act
            var instance = _target.TryResolve<ITypeToResolveWithConstructorParams>();

            // Assert
            instance.Should().BeOfType<ConcreteTypeWithConstructorParams>();
        }

        // Resolve test for Creator registration is deactivated because this scenario is already covered by the several RegisterCreator... tests
        //[Test]
        //public void Resolve_ShouldResolveConcreteType_WhenCreatorIsRegistered()
        //{
        //    // Arrange
        //    _target.RegisterCreator<ITypeToResolve>(r => new ConcreteType());
        //    // Act
        //    var instance = _target.Resolve<ITypeToResolve>();
        //    // Assert
        //    instance.Should().BeOfType<ConcreteType>();
        //}

        [Test]
        public void TryResolve_ShouldResolveConcreteType_WhenCreatorIsRegistered()
        {
            // Arrange
            _target.RegisterCreator<ITypeToResolve>(r => new ConcreteType());
            // Act
            var instance = _target.TryResolve<ITypeToResolve>();
            // Assert
            instance.Should().BeOfType<ConcreteType>();
        }
    }
}