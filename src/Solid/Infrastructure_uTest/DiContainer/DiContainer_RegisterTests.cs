//----------------------------------------------------------------------------------
// File: "DiContainer_RegisterTests.cs"
// Author: Steffen Hanke
// Date: 2017-2022
//----------------------------------------------------------------------------------
using FluentAssertions;
using NUnit.Framework;
using System;

namespace Solid.Infrastructure_uTest.DiContainer
{
    public class DiContainer_RegisterTests : DiContainerTestBase
    {
        [Test]
        public void RegisterInstance_ShouldThrow_WhenInstanceIsNull()
        {
            // Arrange
            // Act
            Action action = () => _target.RegisterInstance<ITypeToResolve>(null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void RegisterInstance_ShouldResolveSingleton()
        {
            // Arrange
            // Act
            var instance = new ConcreteType();
            _target.RegisterInstance<ITypeToResolve>(instance);

            // Assert
            var instance1 = _target.Resolve<ITypeToResolve>();
            instance1.Should().BeSameAs(instance);
            var instance2 = _target.Resolve<ITypeToResolve>();
            instance2.Should().BeSameAs(instance);
        }

        [Test]
        public void RegisterType_ShouldResolveSingleton()
        {
            // Arrange
            // Act
            _target.RegisterType<ITypeToResolve, ConcreteType>();

            // Assert
            var instance1 = _target.Resolve<ITypeToResolve>();
            var instance2 = _target.Resolve<ITypeToResolve>();
            instance1.Should().BeSameAs(instance2);
        }

        [Test]
        public void RegisterTypeAsTransient_ShouldResolveTransient()
        {
            // Arrange
            // Act
            _target.RegisterTypeAsTransient<ITypeToResolve, ConcreteType>();

            // Assert
            var instance1 = _target.Resolve<ITypeToResolve>();
            var instance2 = _target.Resolve<ITypeToResolve>();
            instance1.Should().NotBeSameAs(instance2);
        }

        [Test]
        public void RegisterCreator_ShouldResolveSingleton()
        {
            // Arrange
            // Act
            _target.RegisterCreator<ITypeToResolve>(creator => new ConcreteType());
            // Assert
            var instance1 = _target.Resolve<ITypeToResolve>();
            var instance2 = _target.Resolve<ITypeToResolve>();
            instance1.Should().BeSameAs(instance2);
        }

        [Test]
        public void RegisterCreatorAsTransient_ShouldResolveTransient()
        {
            // Arrange
            // Act
            _target.RegisterCreatorAsTransient<ITypeToResolve>(r => new ConcreteType());
            // Assert
            var instance1 = _target.Resolve<ITypeToResolve>();
            var instance2 = _target.Resolve<ITypeToResolve>();
            instance1.Should().NotBeSameAs(instance2);
        }

        [Test]
        public void RegisterCreator_ShouldResolveSingleton_WhenCreatorForTypeWithConstructorParametersIsRegistered()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();
            // Act
            _target.RegisterCreator<ITypeToResolveWithConstructorParams>(r => new ConcreteTypeWithConstructorParams(r.Resolve<ITypeToResolve>()));
            // Assert
            var instance1 = _target.Resolve<ITypeToResolveWithConstructorParams>();
            var instance2 = _target.Resolve<ITypeToResolveWithConstructorParams>();
            instance1.Should().BeSameAs(instance2);
        }

        [Test]
        public void RegisterCreatorAsTransient_ShouldResolveTransient_WhenCreatorForTypeWithConstructorParametersIsRegistered()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();
            // Act
            _target.RegisterCreatorAsTransient<ITypeToResolveWithConstructorParams>(r => new ConcreteTypeWithConstructorParams(r.Resolve<ITypeToResolve>()));
            // Assert
            var instance1 = _target.Resolve<ITypeToResolveWithConstructorParams>();
            var instance2 = _target.Resolve<ITypeToResolveWithConstructorParams>();
            instance1.Should().NotBeSameAs(instance2);
        }

        //[Test]
        //public void RegisterType_Twice_ShouldNotThrow()
        //{
        //    // Arrange
        //    _target.RegisterType<ITypeToResolve, ConcreteType>();
        //    // Act
        //    Action action = () => _target.RegisterType<ITypeToResolve, ConcreteType>();
        //    // Assert
        //    action.Should().NotThrow();
        //}
        //[Test]
        //public void RegisterInstance_Twice_ShouldNotThrow()
        //{
        //    // Arrange
        //    var instance = new ConcreteType();
        //    _target.RegisterInstance<ITypeToResolve>(instance);
        //    // Act
        //    Action action = () => _target.RegisterInstance<ITypeToResolve>(instance);
        //    // Assert
        //    action.Should().NotThrow();
        //}
        //[Test]
        //public void RegisterCreator_Twice_ShouldNotThrow()
        //{
        //    // Arrange
        //    _target.RegisterCreator<ITypeToResolve>(creator => new ConcreteType());
        //    // Act
        //    Action action = () => _target.RegisterCreator<ITypeToResolve>(creator => new ConcreteType());
        //    // Assert
        //    action.Should().NotThrow();
        //}

        [Test]
        public void RegisterType_Twice_ShouldOverwriteFirstRegistration()
        {
            // Arrange
            _target.RegisterType<ITypeToResolve, ConcreteType>();
            // Act
            _target.RegisterType<ITypeToResolve, ConcreteTypeDerived>();
            // Assert
            var instance = _target.Resolve<ITypeToResolve>();
            instance.Should().BeOfType<ConcreteTypeDerived>();
        }

        [Test]
        public void RegisterInstance_Twice_ShouldOverwriteFirstRegistration()
        {
            // Arrange
            var instance1 = new ConcreteType();
            _target.RegisterInstance<ITypeToResolve>(instance1);
            var instance2 = new ConcreteTypeDerived();
            // Act
            _target.RegisterInstance<ITypeToResolve>(instance2);
            // Assert
            var instance = _target.Resolve<ITypeToResolve>();
            instance.Should().BeOfType<ConcreteTypeDerived>()
                .And.BeSameAs(instance2);
        }

        [Test]
        public void RegisterCreator_Twice_ShouldOverwriteFirstRegistration()
        {
            // Arrange
            _target.RegisterCreator<ITypeToResolve>(creator => new ConcreteType());
            // Act
            _target.RegisterCreator<ITypeToResolve>(creator => new ConcreteTypeDerived());
            // Assert
            var instance = _target.Resolve<ITypeToResolve>();
            instance.Should().BeOfType<ConcreteTypeDerived>();
        }
    }
}