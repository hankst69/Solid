//----------------------------------------------------------------------------------
// <copyright file="NullableBoolExtension.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.RuntimeTypeExtensions
{
    public static class NullableBoolExtensions
    {
        public static bool IsTrue(this bool? nullableBool)
        {
            return nullableBool != null && nullableBool.HasValue && nullableBool.Value;
        }

        public static bool IsFalse(this bool? nullableBool)
        {
            return !nullableBool.IsTrue();
        }
    }
}
