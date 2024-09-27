//----------------------------------------------------------------------------------
// File: "ObjectExtensions.cs"
// Author: Steffen Hanke
// Date: 2016-2020
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Solid.Infrastructure.RuntimeTypeExtensions
{
    public static class ObjectExtensions
    {
        public static T As<T>(this object obj)
            where T : class
        {
            return obj as T;
        }

        public static T CastTo<T>(this object obj)
        {
            if (obj is T)
            {
                return (T)obj;
            }

            var type = typeof(T);

            // Handle nullable types correctly
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);

                if (obj == null)
                {
                    return Activator.CreateInstance<T>();
                }
            }

            return (T)Convert.ChangeType(obj, type);
        }

        public static IEnumerable<T> AsEnumerable<T>(this T obj)
        {
            yield return obj;
        }

        public static int SizeOf<T>(this T obj)
        {
            return SizeOfCache<T>.SizeOf;
        }

        private static class SizeOfCache<T>
        {
            internal static readonly int SizeOf;

            static SizeOfCache()
            {
                var dm = new DynamicMethod("func", typeof(int),
                    Type.EmptyTypes, typeof(ObjectExtensions));

                ILGenerator il = dm.GetILGenerator();
                il.Emit(OpCodes.Sizeof, typeof(T));
                il.Emit(OpCodes.Ret);

                var func = (Func<int>)dm.CreateDelegate(typeof(Func<int>));
                SizeOf = func();
            }
        }
    }
}