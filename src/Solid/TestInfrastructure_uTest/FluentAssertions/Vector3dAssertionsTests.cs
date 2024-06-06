//----------------------------------------------------------------------------------
// File: "Vector3DAssertionsTests.cs"
// Date: 2015-2019
//----------------------------------------------------------------------------------

using System;
using System.Globalization;
using FluentAssertions;
using Solid.TestInfrastructure.FluentAssertions;
using NUnit.Framework;
using Solid.Infrastructure.Math;

namespace Solid.TestInfrastructure_uTest.FluentAssertions
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming for consistency with syngo
    public class Vector3DAssertionsTests
    {
        // ReSharper disable once InconsistentNaming for consistency with syngo
        private class TestVector3D : Vector3D
        {
            public TestVector3D()
            {
            }

            public TestVector3D(double x, double y, double z)
                : base(x, y, z)
            {
            }

            public override string ToString()
            {
                return ToString("E7", CultureInfo.InvariantCulture);
            }
        }

        [Test]
        public void BeAlmostEqual_ShouldThrow_WhenExpectedNull()
        {
            var target = new Vector3DAssertions(new Vector3D());

            Assert.That(
                () => target.BeAlmostEqual(null),
                Throws.InstanceOf<ArgumentNullException>()
                    .And.Message.StartsWith("Cannot verify equivalence against a <null> vector."));
        }

        [Test]
        public void BeAlmostEqual_ShouldThrow_WhenSubjectNull()
        {
            var target = new Vector3DAssertions(null);

            Assert.That(
                () => target.BeAlmostEqual(new TestVector3D()),
                Throws.InstanceOf<AssertionException>()
                    .And.Message.StartsWith("Expected Vector3D to be almost")
                    .And.Message.EndsWith(", but found <null>."));
        }

        [Test]
        public void BeAlmostEqual_ShouldThrow_WhenSubjectNotAlmostEqualExpected()
        {
            var subject = new TestVector3D(1, 2, 3);
            var expected = new TestVector3D(2, 3, 4);
            var target = new Vector3DAssertions(subject);

            Assert.That(
                () => target.BeAlmostEqual(expected),
                Throws.InstanceOf<AssertionException>()
                    .And.Message.StartsWith("Expected Vector3D")
                    .And.Message.EndsWith(", but it differed."));
        }

        [Test]
        public void BeAlmostEqual_ShouldReturnAndConstraint_WhenSubjectAlmostEqualExpected()
        {
            var subject = new Vector3D(1, 2, 3);
            var expected = new Vector3D(subject);
            var target = new Vector3DAssertions(subject);

            var result = target.BeAlmostEqual(expected);

            Assert.That(result, Is.Not.Null.And.InstanceOf<AndConstraint<Vector3DAssertions>>());
        }

        [Test]
        public void NotBeAlmostEqual_ShouldThrow_WhenUnexpectedNull()
        {
            var target = new Vector3DAssertions(new Vector3D());

            Assert.That(() => target.NotBeAlmostEqual(null),
                Throws.InstanceOf<ArgumentNullException>()
                .And.Message
                .StartsWith("Cannot verify equivalence against a <null> vector."));
        }

        [Test]
        public void NotBeAlmostEqual_ShouldThrow_WhenSubjectNull()
        {
            var target = new Vector3DAssertions(null);

            Assert.That(() => target.NotBeAlmostEqual(new TestVector3D()),
                Throws.InstanceOf<AssertionException>()
                    .And.Message.StartsWith("Expected Vector3D not to be almost")
                    .And.Message.EndsWith(", but found <null>."));
        }

        [Test]
        public void NotBeAlmostEqual_ShouldThrow_WhenSubjectAlmostEqualExpected()
        {
            var target = new Vector3DAssertions(new TestVector3D());

            Assert.That(() => target.NotBeAlmostEqual(new TestVector3D()),
                Throws.InstanceOf<AssertionException>()
                    .And.Message.StartsWith("Did not expect Vector3D")
                    .And.Message.EndsWith("."));
        }

        [Test]
        public void NotBeAlmostEqual_ShouldReturnAndConstraint_WhenSubjectNotAlmostEqualToUnexpected()
        {
            var target = new Vector3DAssertions(new Vector3D(3, 5, 2));

            var result = target.NotBeAlmostEqual(new Vector3D());

            Assert.That(result, Is.Not.Null.And.InstanceOf<AndConstraint<Vector3DAssertions>>());
        }
    }
}