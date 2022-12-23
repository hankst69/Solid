//----------------------------------------------------------------------------------
// <copyright file="DiContainer_IsRegisteredTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using FluentAssertions;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.DiContainer
{
    public class DiContainer_IsRegisteredTests : DiContainerTests
    {
        [Test]
        public void IsTypeRegisterd_ShouldReturnFalse_WhenTypeIsNotRegistered()
        {
            // Arrange
            // Act
            var result = _target.IsTypeRegistered<ITypeToResolve>();
            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsTypeRegisterd_ShouldReturnTrue_WhenRegisteredAsInstance()
        {
            // Arrange
            _target.RegisterInstance<ITypeToResolve>(new ConcreteType());
            // Act
            var result = _target.IsTypeRegistered<ITypeToResolve>();
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsTypeRegisterd_ShouldReturnTrue_WhenRegisteredAsType()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();
            // Act
            var result = _target.IsTypeRegistered<ITypeToResolve>();
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsTypeRegisterd_ShouldReturnTrue_WhenRegisteredAsCreator()
        {
            // Arrange
            _target.RegisterCreator<ITypeToResolve>(creator => new ConcreteType());
            // Act
            var result = _target.IsTypeRegistered<ITypeToResolve>();
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsTypeRegisterd_ShouldReturnTrue_WhenRegisteredTransientAsType()
        {
            // Arrange
            _target.RegisterTypeAsTransient<ITypeToResolve, ConcreteType>();
            // Act
            var result = _target.IsTypeRegistered<ITypeToResolve>();
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsTypeRegisterd_ShouldReturnTrue_WhenRegisteredTransientAsCreator()
        {
            // Arrange
            _target.RegisterCreatorAsTransient<ITypeToResolve>(creator => new ConcreteType());
            // Act
            var result = _target.IsTypeRegistered<ITypeToResolve>();
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsTypeRegisterd_ShouldReturnTrue_WhenConcreteTypeWithDefaultConstructorParamsIsRegisteredButConstructorParametersAreNotRegistered()
        {
            // Arrange
            _target.RegisterType<ITypeToResolveWithDefaultConstructorParams, ConcreteTypeWithDefaultConstructorParams>();
            // Act
            var result = _target.IsTypeRegistered<ITypeToResolveWithDefaultConstructorParams>();
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsTypeRegisterd_ShouldReturnFalse_WhenTypeIsRegisteredButConcreteTypeIsQueried()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();
            // Act
            var result = _target.IsTypeRegistered<ConcreteType>();
            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsTypeImplementationRegistered_ShouldReturnTrue_WhenTypeIsRegisteredButConcreteTypeIsQueried()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();
            // Act
            var result = _target.IsTypeImplementationRegistered<ConcreteType>();
            // Assert
            result.Should().BeTrue();
        }

    }
}
