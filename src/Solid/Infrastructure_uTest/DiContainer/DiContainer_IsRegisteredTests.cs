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
    public class DiContainer_IsRegisteredTests : DiContainerTestBase
    {
        [Test]
        public void IsRegisterd_ShouldReturnFalse_WhenTypeIsNotRegistered()
        {
            // Arrange
            // Act
            var result = _target.IsRegistered<ITypeToResolve>();
            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsRegisterd_ShouldReturnTrue_WhenRegisteredAsInstance()
        {
            // Arrange
            _target.RegisterInstance<ITypeToResolve>(new ConcreteType());
            // Act
            var result = _target.IsRegistered<ITypeToResolve>();
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsRegisterd_ShouldReturnTrue_WhenRegisteredAsType()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();
            // Act
            var result = _target.IsRegistered<ITypeToResolve>();
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsRegisterd_ShouldReturnTrue_WhenRegisteredAsCreator()
        {
            // Arrange
            _target.RegisterCreator<ITypeToResolve>(creator => new ConcreteType());
            // Act
            var result = _target.IsRegistered<ITypeToResolve>();
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsRegisterd_ShouldReturnTrue_WhenRegisteredTransientAsType()
        {
            // Arrange
            _target.RegisterTypeAsTransient<ITypeToResolve, ConcreteType>();
            // Act
            var result = _target.IsRegistered<ITypeToResolve>();
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsRegisterd_ShouldReturnTrue_WhenRegisteredTransientAsCreator()
        {
            // Arrange
            _target.RegisterCreatorAsTransient<ITypeToResolve>(creator => new ConcreteType());
            // Act
            var result = _target.IsRegistered<ITypeToResolve>();
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsRegisterd_ShouldReturnTrue_WhenConcreteTypeWithDefaultConstructorParamsIsRegisteredButConstructorParametersAreNotRegistered()
        {
            // Arrange
            _target.RegisterType<ITypeToResolveWithDefaultConstructorParams, ConcreteTypeWithDefaultConstructorParams>();
            // Act
            var result = _target.IsRegistered<ITypeToResolveWithDefaultConstructorParams>();
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsRegisterd_ShouldReturnFalse_WhenTypeIsRegisteredButConcreteTypeIsQueried()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();
            // Act
            var result = _target.IsRegistered<ConcreteType>();
            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsRegisteredAnyImplementing_ShouldReturnTrue_WhenTypeIsRegisteredButConcreteTypeIsQueried()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();
            // Act
            var result = _target.IsRegisteredAnyImplementing<ConcreteType>();
            // Assert
            result.Should().BeTrue();
        }

    }
}
