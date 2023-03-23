//----------------------------------------------------------------------------------
// <copyright file="EnumeExtensions.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;

namespace Solid.Infrastructure.RuntimeTypeExtensions
{
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this object enumIn)
            where T : struct
            => (T)Enum.Parse(typeof(T), enumIn.ToString());
    }
}
