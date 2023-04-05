//----------------------------------------------------------------------------------
// <copyright file="ConsistencyCheckTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using FluentAssertions;
using Solid.Infrastructure.Diagnostics;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.Diagnostics
{
    public class ConsistencyCheckTests
    {
        [Test]
        public void EnsureArgument_IsNotNull_ShouldThrow_WhenArgumentIsNull()
        {
            // Arrange
            object x = null;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsNotNull();

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EnsureArgument_IsNotNull_ShouldNotThrow_WhenArgumentIsNotNull()
        {
            // Arrange
            object x = new Object();

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsNotNull();

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void EnsureArgument_IsNotNullOrEmpty_ShouldThrow_WhenArgumentIsNull()
        {
            // Arrange
            string x = null;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsNotNullOrEmpty();

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EnsureArgument_IsNotNullOrEmpty_ShouldThrow_WhenArgumentIsEmpty()
        {
            // Arrange
            string x = string.Empty;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsNotNullOrEmpty();

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void EnsureArgument_IsNotNullOrEmpty_ShouldNotThrow_WhenArgumentIsNotNull()
        {
            // Arrange
            string x = "anyString";

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsNotNullOrEmpty();

            // Assert
            action.Should().NotThrow();
        }


        [Test]
        public void EnsureArgument_IsGreaterThan_ShouldThrow_WhenArgumentIsNull()
        {
            // Arrange
            string x = null;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsGreaterThan("0");

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EnsureArgument_IsGreaterThan_ShouldNotThrow_WhenArgumentIsGreater()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsGreaterThan(3);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void EnsureArgument_IsGreaterThan_ShouldThrow_WhenArgumentIsEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsGreaterThan(4);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void EnsureArgument_IsGreaterThan_ShouldThrow_WhenArgumentIsLess()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsGreaterThan(5);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }


        [Test]
        public void EnsureArgument_IsGreaterOrEqualThan_ShouldThrow_WhenArgumentIsNull()
        {
            // Arrange
            string x = null;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsGreaterOrEqual("0");

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EnsureArgument_IsGreaterOrEqualThan_ShouldNotThrow_WhenArgumentIsGreaterOrEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsGreaterOrEqual(3);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void EnsureArgument_IsGreaterOrEqualThan_ShouldNotThrow_WhenArgumentIsEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsGreaterOrEqual(4);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void EnsureArgument_IsGreaterOrEqualThan_ShouldThrow_WhenArgumentIsLess()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsGreaterOrEqual(5);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void EnsureArgument_IsEqual_ShouldThrow_WhenArgumentIsNull()
        {
            // Arrange
            string x = null;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsEqual("0");

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EnsureArgument_IsEqual_ShouldThrow_WhenArgumentIsNotEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsEqual(3);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void EnsureArgument_IsEqual_ShouldNotThrow_WhenArgumentIsEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsEqual(4);

            // Assert
            action.Should().NotThrow();
        }


        [Test]
        public void EnsureArgument_IsNotEqual_ShouldThrow_WhenArgumentIsNull()
        {
            // Arrange
            string x = null;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsNotEqual("0");

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EnsureArgument_IsNotEqual_ShouldThrow_WhenArgumentIsEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsNotEqual(4);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void EnsureArgument_IsNotEqual_ShouldNotThrow_WhenArgumentIsNotEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureArgument(x).IsNotEqual(3);

            // Assert
            action.Should().NotThrow();
        }

        //------------------------------------------------------------------------------------
        //EnsureValue tests

        [Test]
        public void EnsureValue_IsNotNull_ShouldThrow_WhenArgumentIsNull()
        {
            // Arrange
            object x = null;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsNotNull();

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EnsureValue_IsNotNull_ShouldNotThrow_WhenArgumentIsNotNull()
        {
            // Arrange
            object x = new Object();

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsNotNull();

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void EnsureValue_IsNotNullOrEmpty_ShouldThrow_WhenArgumentIsNull()
        {
            // Arrange
            string x = null;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsNotNullOrEmpty();

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EnsureValue_IsNotNullOrEmpty_ShouldThrow_WhenArgumentIsEmpty()
        {
            // Arrange
            string x = string.Empty;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsNotNullOrEmpty();

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void EnsureValue_IsNotNullOrEmpty_ShouldNotThrow_WhenArgumentIsNotNull()
        {
            // Arrange
            string x = "anyString";

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsNotNullOrEmpty();

            // Assert
            action.Should().NotThrow();
        }


        [Test]
        public void EnsureValue_IsGreaterThan_ShouldThrow_WhenArgumentIsNull()
        {
            // Arrange
            string x = null;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsGreaterThan("0");

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EnsureValue_IsGreaterThan_ShouldNotThrow_WhenArgumentIsGreater()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsGreaterThan(3);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void EnsureValue_IsGreaterThan_ShouldThrow_WhenArgumentIsEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsGreaterThan(4);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void EnsureValue_IsGreaterThan_ShouldThrow_WhenArgumentIsLess()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsGreaterThan(5);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }


        [Test]
        public void EnsureValue_IsGreaterOrEqualThan_ShouldThrow_WhenArgumentIsNull()
        {
            // Arrange
            string x = null;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsGreaterOrEqual("0");

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EnsureValue_IsGreaterOrEqualThan_ShouldNotThrow_WhenArgumentIsGreaterOrEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsGreaterOrEqual(3);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void EnsureValue_IsGreaterOrEqualThan_ShouldNotThrow_WhenArgumentIsEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsGreaterOrEqual(4);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void EnsureValue_IsGreaterOrEqualThan_ShouldThrow_WhenArgumentIsLess()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsGreaterOrEqual(5);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void EnsureValue_IsEqual_ShouldThrow_WhenArgumentIsNull()
        {
            // Arrange
            string x = null;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsEqual("0");

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EnsureValue_IsEqual_ShouldThrow_WhenArgumentIsNotEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsEqual(3);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void EnsureValue_IsEqual_ShouldNotThrow_WhenArgumentIsEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsEqual(4);

            // Assert
            action.Should().NotThrow();
        }


        [Test]
        public void EnsureValue_IsNotEqual_ShouldThrow_WhenArgumentIsNull()
        {
            // Arrange
            string x = null;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsNotEqual("0");

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void EnsureValue_IsNotEqual_ShouldThrow_WhenArgumentIsEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsNotEqual(4);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void EnsureValue_IsNotEqual_ShouldNotThrow_WhenArgumentIsNotEqual()
        {
            // Arrange
            int x = 4;

            // Act
            Action action = () => ConsistencyCheck.EnsureValue(x).IsNotEqual(3);

            // Assert
            action.Should().NotThrow();
        }

        // todo: write tests for
        // IsNotEmpty (IEnumerable)
        // IsExistingFile
        // IsExistingDirectory
        // IsNotGreaterThan
        // IsTrue
        // IsFalse
        // IsOfType
        // IsOfAnyType
        // IsNotOfType
        // IsNotOfAnyType

    }
}
