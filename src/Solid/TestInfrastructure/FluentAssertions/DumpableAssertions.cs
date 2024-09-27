//----------------------------------------------------------------------------------
// File: "DumpableAssertions.cs"
// Date: 2015-2019
//----------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Solid.Infrastructure.Diagnostics;

namespace Solid.TestInfrastructure.FluentAssertions
{
    [DebuggerNonUserCode]
    public class DumpableAssertions : ObjectAssertions
    {
        protected internal DumpableAssertions(IDumpable value)
            : base(value)
        {
        }

        public AndConstraint<DumpableAssertions> BeDumpedTo(string expected, StringComparison comparisonType = StringComparison.Ordinal, string because = "", params object[] reasonArgs)
        {
            if (expected == null)
            {
                throw new ArgumentNullException("expected", "Cannot verify equivalence against a <null> vector.");
            }

            Execute.Assertion
                .ForCondition(!ReferenceEquals(Subject, null))
                .BecauseOf(because, reasonArgs)
                .FailWith("Expected serialized string to be {0}{reason}, but found <null>.", expected);

            var dumped = Dump((IDumpable)Subject);

            Execute.Assertion
                .BecauseOf(because, reasonArgs)
                .ForCondition(String.Compare(expected, dumped, comparisonType) == 0)
                .FailWith("Expected serialized string {0} to be {1} compared with StringComparison.{2}{reason}, but it differed.",
                    dumped, expected, comparisonType);

            return new AndConstraint<DumpableAssertions>(this);
        }

        private static string Dump(IDumpable toDump)
        {
            var dumped = toDump.Dump();
            return DataContractSerialize(dumped);
        }

        private static string DataContractSerialize(object toSerialize)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(toSerialize.GetType());
                serializer.WriteObject(stream, toSerialize);

                stream.Seek(0, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(stream))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            }
        }
    }
}