//----------------------------------------------------------------------------------
// <copyright file="DumpableAssertionsTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using FluentAssertions;
using Solid.Infrastructure.Diagnostics;
using Solid.TestInfrastructure.FluentAssertions;
using NUnit.Framework;

namespace Solid.TestInfrastructure_uTest.FluentAssertions
{
    [TestFixture]
    public class DumpableAssertionsTests
    {
        internal class TestDumpable : IDumpable
        {
            private readonly string m_Dump;

            public TestDumpable(string dump)
            {
                m_Dump = dump;
            }

            public object Dump()
            {
                return m_Dump;
            }
        }

        [Test]
        public void BeDumpedTo_ShouldThrow_WhenExpectedNull()
        {
            var target = new DumpableAssertions(new TestDumpable("dump"));

            Assert.That(
                () => target.BeDumpedTo(null),
                Throws.InstanceOf<ArgumentNullException>()
                    .And.Message.StartsWith("Cannot verify equivalence against a <null> vector."));
        }

        [Test]
        public void BeDumpedTo_ShouldThrow_WhenSubjectNull()
        {
            var target = new DumpableAssertions(null);
            var expected = "dump";

            Assert.That(
                () => target.BeDumpedTo(expected),
                Throws.InstanceOf<AssertionException>()
                    .And.Message.StartsWith("Expected serialized string to be ")
                    .And.Message.EndsWith(", but found <null>."));
        }

        [Test]
        public void BeDumpedTo_ShouldThrow_WhenSubjectNotAlmostEqualExpected()
        {
            var subject = new TestDumpable("otherDump");
            var expected = "dump";
            var target = new DumpableAssertions(subject);

            Assert.That(
                () => target.BeDumpedTo(expected),
                Throws.InstanceOf<AssertionException>()
                    .And.Message.StartsWith("Expected serialized string")
                    .And.Message.EndsWith(", but it differed."));
        }

        [Test]
        public void BeDumpedTo_ShouldReturnAndConstraint_WhenSubjectAlmostEqualExpected()
        {
            var subject = new TestDumpable("matchingDump");
            var expected = "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">" + subject.Dump() + "</string>";
            var target = new DumpableAssertions(subject);

            var result = target.BeDumpedTo(expected);

            Assert.That(result, Is.Not.Null.And.InstanceOf<AndConstraint<DumpableAssertions>>());
        }
    }
}
