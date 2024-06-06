//----------------------------------------------------------------------------------
// File: "AssertionExtensions.cs"
// Date: 2015-2019
//----------------------------------------------------------------------------------

using System.Diagnostics;
using System.Runtime.CompilerServices;

using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Math;

[assembly: InternalsVisibleTo("Solid.TestInfrastructure_uTest")]

namespace Solid.TestInfrastructure.FluentAssertions
{
    [DebuggerNonUserCode]
    public static class AssertionExtensions
    {
        public static Vector3DAssertions Should(this Vector3D actualValue)
        {
            return new Vector3DAssertions(actualValue);
        }

        public static DumpableAssertions Should(this IDumpable actualValue)
        {
            return new DumpableAssertions(actualValue);
        }
    }
}
