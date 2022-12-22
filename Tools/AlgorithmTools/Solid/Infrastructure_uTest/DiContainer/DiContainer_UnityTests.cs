//----------------------------------------------------------------------------------
// <copyright file="DiContainer_UnityTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

/*
using System.Linq;
using FluentAssertions;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.DiContainer
{
    public partial class DiContainerTests
    {
        [Test]
        public void ResolveAll_OfUnityContainer_ShouldReturnCorrectInstances_WhenRegisteredAsInstance()
        {
            // Arrange
            var container = new UnityContainer();
            container.RegisterInstance<ITypeToResolve>(new ConcreteType());
            container.RegisterInstance<ITypeToResolve>("A", new ConcreteType());
            container.RegisterInstance<ITypeToResolve>("B", new ConcreteType());
            container.RegisterInstance<ITypeNotToResolve>("C", new ConcreteTypeNotToResolve());
            container.RegisterInstance<ITypeToResolveDerived>("D", new ConcreteTypeDerived());

            // Act
            //var resolved = container.Resolve<ITypeToResolve>();
            //var resolved = container.Resolve<ITypeToResolve>("B");
            var allResolvedTypes = container.ResolveAll<ITypeToResolve>().ToList();

            // Assert
            allResolvedTypes.Count().Should().Be(2);
            allResolvedTypes.ForEach(x => x.Should().BeAssignableTo<ITypeToResolve>());
        }

        [Test]
        public void ResolveAll_OfUnityContainer_ShouldReturnCorrectInstances_WhenRegisteredAsType()
        {
            // Arrange
            var container = new UnityContainer();
            container.RegisterType<ITypeToResolve, ConcreteType>();
            container.RegisterType<ITypeToResolve, ConcreteType>("A");
            container.RegisterType<ITypeToResolve, ConcreteType>("B");
            container.RegisterType<ITypeNotToResolve, ConcreteTypeNotToResolve>("C");
            container.RegisterType<ITypeToResolveDerived, ConcreteTypeDerived>("D");

            // Act
            //var resolved = container.Resolve<ITypeToResolve>();
            //var resolved = container.Resolve<ITypeToResolve>("B");
            var allResolvedTypes = container.ResolveAll<ITypeToResolve>().ToList();

            // Assert
            allResolvedTypes.Count().Should().Be(2);
            allResolvedTypes.ForEach(x => x.Should().BeAssignableTo<ITypeToResolve>());
        }
    }
}
*/