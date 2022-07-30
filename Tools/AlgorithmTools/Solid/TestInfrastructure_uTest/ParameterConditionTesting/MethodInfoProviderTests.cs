using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Solid.TestInfrastructure.ParameterConditionTesting;

namespace Solid.TestInfrastructure_uTest.ParameterConditionTesting
{
    [TestFixture]
    public class MethodInfoProviderTests
    {
        [Test]
        public void Api_ShouldThrow_WhenNullChecksMissing()
        {
            // Arrange
            // Act
            // Assert
            ParameterConditionsChecker
                .For<MethodInfoProvider>()
                .CheckCtorParameters()
                .CheckApi()
                .Verify();
        }

        [Test]
        public void GetConstructorInfoWithMostParameters_ShouldReturnDefaultCtor_WhenNoCtorGiven()
        {
            // Arrange
            // Act
            var result = new MethodInfoProvider().GetConstructorInfoWithMostParameters(typeof(ClassWithoutCtor));

            // Assert
            result.ToString().Should().Be("Void .ctor()");
        }

        [Test]
        public void GetConstructorInfoWithMostParameters_ShouldWorkProperly()
        {
            // Arrange
            // Act
            var result = new MethodInfoProvider().GetConstructorInfoWithMostParameters(typeof(ClassWithManyCtors));

            // Assert
            result.GetParameters().Length.Should().Be(2);
        }

        [Test]
        public void GetConstructorInfoWithMostParameters_ShouldConsiderInternalCtors()
        {
            // Arrange

            // Act
            var result = new MethodInfoProvider().GetConstructorInfoWithMostParameters(typeof(ClassWithInternalConstructor));

            // Assert
            result.GetParameters().Length.Should().Be(1);
        }

        [Test]
        public void GetRelevantMethodInfosExcept_ShouldThrow_WhenMethodNameIsInvalid()
        {
            // Arrange
            var target = new MethodInfoProvider();

            // Act
            Action action = () => target.GetRelevantMethodInfosExcept(typeof(ClassWithoutCtor), new List<string> { "unkown" });

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void GetRelevantMethodInfosExcept_ShouldOnlyReturnPublicMethods()
        {
            // Arrange
            var target = new MethodInfoProvider();

            // Act
            var result = target.GetRelevantMethodInfosExcept(typeof(ClassWithManyMethods), new List<string>());

            // Assert
            result.Count().Should().Be(1);
            result.Single().Name.Should().Be("PublicMethod");
        }

        public class ClassWithInternalConstructor
        {
            public ClassWithInternalConstructor()
                : this("foo")
            {
            }

            internal ClassWithInternalConstructor(string input)
            {
            }
        }

        public class ClassWithoutCtor
        {
        }

        public class ClassWithManyCtors
        {
            public ClassWithManyCtors()
            {
            }

            public ClassWithManyCtors(int single)
            {
            }

            public ClassWithManyCtors(string a, double foo)
            {
            }
        }

        public class ClassWithManyMethods
        {
            public ClassWithManyMethods(int foo)
            {
            }

            private void PrivateMethod() { }

            protected void ProtectedMethod() { }

            public void PublicMethod() { }

            internal void InternalMethod() { }

            public bool OnlyGetter
            {
                get { return true; }
            }

            public bool OnlySetter
            {
                set { }
            }

            public bool GetterAndSetter
            {
                get { return true; }
                set { }
            }

            public event EventHandler PublicEvent;

            private void Invoke()
            {
                var handler = PublicEvent;
            }
        }
    }
}
