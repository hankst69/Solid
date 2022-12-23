//----------------------------------------------------------------------------------
// <copyright file="DiContainer_SpecialTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using FluentAssertions;
using Moq;
using NUnit.Framework;
using Solid.Infrastructure.DiContainer;
using System.Linq;

namespace Solid.Infrastructure_uTest.DiContainer
{
    public class DiContainer_SpecialTests : DiContainerTests
    {
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
}
