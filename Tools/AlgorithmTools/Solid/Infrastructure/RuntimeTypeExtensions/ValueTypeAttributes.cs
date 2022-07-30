//----------------------------------------------------------------------------------
// <copyright file="ValueTypeAttributes.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Solid.Infrastructure.Diagnostics;
// ReSharper disable StaticMemberInGenericType

namespace Solid.Infrastructure.RuntimeTypeExtensions
{
    public static class ValueTypeAttributes<T> where T : struct, IComparable
    {
        private static readonly Lazy<Type> s_Type;
        private static readonly Lazy<bool> s_IsValueType;
        private static readonly Lazy<bool> s_IsFloatingPointType;
        private static readonly Lazy<bool> s_IsSigned;
        private static readonly Lazy<T> s_DefaultValue;
        private static readonly Lazy<T> s_MinValue;
        private static readonly Lazy<T> s_MaxValue;
        private static readonly Lazy<int> s_Size;
        private static readonly Lazy<int> s_TotalBits;
        private static readonly Lazy<int> s_UsableBits;

        public static Type Type => s_Type.Value;
        public static bool IsValueType => s_IsValueType.Value;
        public static bool IsFloatingPointType => s_IsFloatingPointType.Value;
        public static bool IsSigned => s_IsSigned.Value;
        public static T DefaultValue => s_DefaultValue.Value;
        public static T MinValue => s_MinValue.Value;
        public static T MaxValue => s_MaxValue.Value;
        public static int Size => s_Size.Value;
        public static int TotalBits => s_TotalBits.Value;
        public static int UsableBits => s_UsableBits.Value;

        static ValueTypeAttributes()
        {
            ConsistencyCheck.EnsureArgument(typeof(T)) //,"typeof(T)")
                .IsOfAnyType(new[] {
                    typeof(char),
                    typeof(bool),
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(decimal),
                    typeof(float),
                    typeof(double)
                });

            s_Type = new Lazy<Type>(() 
                => typeof(T));

            s_IsValueType = new Lazy<bool>(() 
                => Type.IsValueType);

            s_IsFloatingPointType = new Lazy<bool>(() 
                => Type == typeof(decimal) || Type == typeof(float) || Type == typeof(double));

            s_DefaultValue = new Lazy<T>(()
                => default(T));

            s_MinValue = new Lazy<T>(() 
                => (T)Type.GetField("MinValue").GetValue(null));

            s_MaxValue = new Lazy<T>(()
                => (T)Type.GetField("MaxValue").GetValue(null));

            s_IsSigned = new Lazy<bool>(()
                => Comparer<T>.Default.Compare(MinValue, DefaultValue) < 0);

            s_Size = new Lazy<int>(() 
                => s_DefaultValue.Value.SizeOf());

            s_TotalBits = new Lazy<int>(() 
                => Size * 8);

            s_UsableBits = new Lazy<int>(() 
                => IsSigned ? TotalBits - 1 : TotalBits);
        }
    }
}
