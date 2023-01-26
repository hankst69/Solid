using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;

using Solid.Infrastructure.Diagnostics;
using Solid.TestInfrastructure.ParameterConditionTesting;

namespace Solid.TestInfrastructure_uTest.ParameterConditionTesting
{
    [TestFixture]
    public class ParameterConditionsCheckerTests
    {
        [Test]
        public void Verify_ShouldPass_WhenOnlyDefaultCtorGiven()
        {
            // Arrange
            // Act
            // Assert
            ParameterConditionsChecker
                .For<ClassWithOnlyDefaultCtor>()
                .CheckCtorParameters()
                .Verify();
        }

        [Test]
        public void Verify_ShouldFail_WhenOnlyDefaultCtorGivenAndParametersGetExcluded()
        {
            // Arrange
            // Act
            // Assert
            var nameOfParametersToSkip = "param";
            try
            {
                ParameterConditionsChecker
                    .For<ClassWithOnlyDefaultCtor>()
                    .CheckCtorParametersExcept(nameOfParametersToSkip)
                    .Verify();
            }
            catch (Exception e)
            {
                e.Should().BeOfType<AssertionException>();
                var expectedMessage = ErrorMessageCreator.CreateMessageForSkippedParametersNotBelongingTo(".ctor", nameOfParametersToSkip);
                e.Message.Should().Be(expectedMessage);
            }
        }

        [Test]
        public void Verify_ShouldPass_WhenOnlyDefaultCtorGivenAndZeroParametersGetExcluded()
        {
            // Arrange
            // Act
            // Assert
            ParameterConditionsChecker
                .For<ClassWithOnlyDefaultCtor>()
                .CheckCtorParametersExcept()
                .Verify();
        }

        [Test]
        public void Verify_ShouldPass_WhenOnlyValueTypeParametersGiven()
        {
            // Arrange
            // Act
            // Assert
            ParameterConditionsChecker
                .For<ClassWithJustValueTypeParameters>()
                .CheckCtorParameters()
                .Verify();
        }

        [Test]
        public void Verify_ShouldPass_WhenOnlyReferenceTypeParametersGivenAndAllHaveNullChecks()
        {
            // Arrange
            // Act
            // Assert
            ParameterConditionsChecker
                .For<ClassWithReferenceValueParameterPlusNullCheck>()
                .CheckCtorParameters()
                .Verify();
        }

        [Test]
        public void VerifyExcludedParameters_ShouldPass_WhenOnlyReferenceTypeParametersGivenAndNotAllHaveNullChecks()
        {
            // Arrange
            // Act
            // Assert
            ParameterConditionsChecker
                .For<ClassWithReferenceValueParametersWithAndWithoutNullCheck>()
                .CheckCtorParametersExcept("withoutNullCheck")
                .Verify();
        }

        [Test]
        public void VerifyForAllParameters_ShouldFail_WhenOnlyReferenceTypeParametersGivenAndNotAllHaveNullChecks()
        {
            // Arrange
            // Act
            // Assert
            try
            {
                ParameterConditionsChecker
                    .For<ClassWithReferenceValueParametersWithAndWithoutNullCheck>()
                    .CheckCtorParameters()
                    .Verify();
            }
            catch (Exception e)
            {
                e.Should().BeOfType<AssertionException>();
                var expectedMessage = ErrorMessageCreator.CreateMessageForMissingArgumentNullExceptionFor(1, "withoutNullCheck", ".ctor");
                e.Message.Should().Be(expectedMessage);
            }
        }

        [Test]
        public void Verify_ShouldFail_WhenOtherExceptionIsThrown()
        {
            // Arrange
            // Act
            Action action = ParameterConditionsChecker
                .For<ClassWithCtorThrowingNotImplementedException>()
                .CheckCtorParameters()
                .Verify;

            // Assert
            action.Should().Throw<AssertionException>();
        }

        [Test]
        public void Verify_ShouldTestCtorWithMostParameters()
        {
            // Arrange
            // Act
            // Assert
            ParameterConditionsChecker
                .For<ClassWithTwoCtors>()
                .CheckCtorParameters()
                .Verify();
        }

        [Test]
        public void Verify_ShouldUseGivenParameter()
        {
            // Arrange
            // Act
            // Assert
            ParameterConditionsChecker
                .For<ClassWithNotOnlyNullChecks>()
                .UseParameters(new List<IAmASimpleInterface> { new Mock<IAmASimpleInterface>().Object })
                .CheckCtorParametersExcept("listOfSimpleInterfaces")
                .Verify();
        }

        public struct MyStruct
        {
        }

        public class ClassWithOnlyDefaultCtor
        {
        }

        public class ClassWithJustValueTypeParameters
        {
            public ClassWithJustValueTypeParameters(int intValue, bool boolValue, double doubleValue, MyStruct structValue)
            {
            }
        }

        public class ClassWithReferenceValueParameterPlusNullCheck
        {
            public ClassWithReferenceValueParameterPlusNullCheck(
                IAmASimpleInterface simpleInterface, string text)
            {
                ConsistencyCheck.EnsureArgument(simpleInterface).IsNotNull();
                ConsistencyCheck.EnsureArgument(text).IsNotNull();
            }
        }

        public class ClassWithReferenceValueParametersWithAndWithoutNullCheck
        {
            public ClassWithReferenceValueParametersWithAndWithoutNullCheck(
                IAmASimpleInterface withNullCheck,
                IAmASimpleInterface withoutNullCheck)
            {
                ConsistencyCheck.EnsureArgument(withNullCheck).IsNotNull();
            }
        }

        public class ClassWithCtorThrowingNotImplementedException
        {
            public ClassWithCtorThrowingNotImplementedException(IAmASimpleInterface simpleInterface)
            {
                throw new NotImplementedException();
            }
        }

        public class ClassWithTwoCtors
        {
            public ClassWithTwoCtors()
            {
                throw new NotImplementedException();
            }

            public ClassWithTwoCtors(IAmASimpleInterface simpleInterface)
            {
                ConsistencyCheck.EnsureArgument(simpleInterface).IsNotNull();
            }
        }

        public class ClassWithNotOnlyNullChecks
        {
            public ClassWithNotOnlyNullChecks(IAmASimpleInterface simpleInterface, IList<IAmASimpleInterface> listOfSimpleInterfaces)
            {
                ConsistencyCheck.EnsureArgument(simpleInterface).IsNotNull();
                ConsistencyCheck.EnsureArgument(listOfSimpleInterfaces).IsNotEmpty();
            }
        }

        public class ClassWithMethodWithoutNullCheck
        {
            public void SetMe(IAmASimpleInterface simpleInterface)
            {
            }
        }

        public interface IAmASimpleInterface
        {
            bool Boolean { get; set; }
            int GetInteger();
            void SetValue(string value);
        }
    }
}
