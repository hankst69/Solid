//----------------------------------------------------------------------------------
// File: "Vector3DAssertions.cs"
// Date: 2015-2019
//----------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Solid.Infrastructure.Math;
using MathsGlobal = Solid.Infrastructure.Math.Vector3D;

namespace Solid.TestInfrastructure.FluentAssertions
{
    [DebuggerNonUserCode]
    // ReSharper disable once InconsistentNaming for consistency with syngo
    public class Vector3DAssertions : ObjectAssertions
    {
        protected internal Vector3DAssertions(Vector3D value)
            : base(value)
        {
        }

        /// <summary>
        /// Asserts that an Vector is almost equal to another object using its <see cref="Vector3D.IsAlmostEqual(Vector3D)" /> implementation.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="tolerance">The Tolerance value</param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="reasonArgs">
        /// Zero or more objects to format using the placeholders in <see cref="because" />.
        /// </param>
        public AndConstraint<Vector3DAssertions> BeAlmostEqual(Vector3D expected, double tolerance = MathsGlobal.DEFAULT_TOLERANCE, string because = "", params object[] reasonArgs)
        {
            if (expected == null)
            {
                throw new ArgumentNullException("expected", "Cannot verify equivalence against a <null> vector.");
            }

            Execute.Assertion
                .ForCondition(!ReferenceEquals(Subject, null))
                .BecauseOf(because, reasonArgs)
                .FailWith("Expected Vector3D to be almost {0}{reason}, but found <null>.", expected);

            Execute.Assertion
                .BecauseOf(because, reasonArgs)
                .ForCondition(expected.IsAlmostEqual((Vector3D)Subject, tolerance))
                .FailWith("Expected Vector3D {0} to be almost {1} +/- {2}{reason}, but it differed.",
                    Subject, expected, tolerance);

            return new AndConstraint<Vector3DAssertions>(this);
        }

        /// <summary>
        /// Asserts that an Vector is not almost equal to another object using its <see cref="Vector3D.IsAlmostEqual(Vector3D)" /> implementation.
        /// </summary>
        /// <param name="unexpected">The unexpected value</param>
        /// <param name="tolerance">The tolerance value</param>
        /// <param name="because">
        /// A formatted phrase explaining why the assertion should be satisfied. If the phrase does not
        /// start with the word <i>because</i>, it is prepended to the message.
        /// </param>
        /// <param name="reasonArgs">
        /// Zero or more values to use for filling in any <see cref="string.Format(string,object[])" /> compatible placeholders.
        /// </param>
        public AndConstraint<Vector3DAssertions> NotBeAlmostEqual(Vector3D unexpected, double tolerance = MathsGlobal.DEFAULT_TOLERANCE, string because = "", params object[] reasonArgs)
        {
            if (unexpected == null)
            {
                throw new ArgumentNullException("unexpected", "Cannot verify equivalence against a <null> vector.");
            }

            Execute.Assertion
                .ForCondition(!ReferenceEquals(Subject, null))
                .BecauseOf(because, reasonArgs)
                .FailWith("Expected Vector3D not to be almost {0}{reason}, but found <null>.", unexpected);

            Execute.Assertion
                .ForCondition(!unexpected.IsAlmostEqual((Vector3D)Subject, tolerance))
                .BecauseOf(because, reasonArgs)
                .FailWith("Did not expect Vector3D {0} to be almost {1} +/- {2}{reason}.",
                    Subject, unexpected, tolerance);

            return new AndConstraint<Vector3DAssertions>(this);
        }
    }
}