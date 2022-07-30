//----------------------------------------------------------------------------------
// <copyright file="AssertionExtensions.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2015-2019. All Rights Reserved. Confidential.
// Author: 
// </copyright>
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
