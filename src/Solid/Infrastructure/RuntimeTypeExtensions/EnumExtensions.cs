//----------------------------------------------------------------------------------
// File: "EnumeExtensions.cs"
// Author: Steffen Hanke
// Date: 2023
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
