//----------------------------------------------------------------------------------
// File: "NullableBoolExtension.cs"
// Author: Steffen Hanke
// Date: 2022
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
