//----------------------------------------------------------------------------------
// <copyright file="ConsistencyCheck.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

// redefine JetBrains.Annotations.NoEnumerationAttribute (without referencing JetBrains.Annotations.dll)

namespace Solid.Infrastructure.Diagnostics
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NoEnumerationAttribute : Attribute { }
}

namespace Solid.Infrastructure.Diagnostics
{
    /// <summary>
    /// ConsistencyCheck
    /// </summary>
    public static class ConsistencyCheck
    {
        public static Validation<T> EnsureArgument<T>(
            [NoEnumeration] T argument, 
            [CallerArgumentExpression("argument")] string argumentName = default)
        {
            return new Validation<T>(argument, argumentName);
        }

        public static Validation<T> EnsureValue<T>(
            [NoEnumeration] T argument,
            [CallerArgumentExpression("argument")] string argumentName = default)
        {
            return new Validation<T>(argument, argumentName);
        }

        public class Validation<T>
        {
            public T Argument { get; }
            public string ArgumentName { get; }

            internal Validation(T argument, string argumentName)
            {
                Argument = argument;
                ArgumentName = argumentName;
            }
        }


        public static Validation<T> IsNotNull<T>(this Validation<T> validation)
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);
            return validation;
        }

        public static Validation<T> IsNotEmpty<T>(this Validation<T> validation)
            where T : IEnumerable
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            if (validation.Argument is ICollection coll && coll.Count < 1)
            {
                throw new ArgumentException("Value is empty", validation.ArgumentName);
            }
            else
            {
                // we need to do the enumeration (which we promised not to do!
                var enumerator = validation.Argument.GetEnumerator();
                enumerator.Reset();
                if (!enumerator.MoveNext())
                    throw new ArgumentException("Value is empty", validation.ArgumentName);
            }
            return validation;
        }

        public static Validation<string> IsNotNullOrEmpty(this Validation<string> validation)
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            if (string.IsNullOrEmpty(validation.Argument))
                throw new ArgumentException("Value is empty", validation.ArgumentName);
            return validation;
        }

        public static Validation<string> IsExistingFile(this Validation<string> validation)
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            if (!System.IO.File.Exists(validation.Argument))
                throw new ArgumentException($"The file '{validation.Argument}' does not exist", validation.ArgumentName);
            return validation;
        }

        public static Validation<string> IsExistingDirectory(this Validation<string> validation)
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            if (!System.IO.Directory.Exists(validation.Argument))
                throw new ArgumentException($"The directory '{validation.Argument}' does not exist", validation.ArgumentName);
            return validation;
        }

        public static Validation<T> IsGreaterThan<T>(this Validation<T> validation, T value)
            where T : IComparable
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            var comparer = Comparer<T>.Default;

            if (!(comparer.Compare(validation.Argument, value) > 0))
                throw new ArgumentOutOfRangeException(string.Concat(validation.ArgumentName, " should be greater than ", value));

            return validation;
        }

        public static Validation<T> IsNotGreaterThan<T>(this Validation<T> validation, T value)
            where T : IComparable
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            var comparer = Comparer<T>.Default;

            if (comparer.Compare(validation.Argument, value) > 0)
                throw new ArgumentOutOfRangeException(string.Concat(validation.ArgumentName, " should not be greater than ", value));

            return validation;
        }

        public static Validation<T> IsGreaterOrEqual<T>(this Validation<T> validation, T value)
            where T : IComparable
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            var comparer = Comparer<T>.Default;

            if (!(comparer.Compare(validation.Argument, value) >= 0))
                throw new ArgumentOutOfRangeException(string.Concat(validation.ArgumentName, " should be greater or equal ", value));

            return validation;
        }

        public static Validation<T> IsEqual<T>(this Validation<T> validation, T value)
            where T : IComparable
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            var comparer = Comparer<T>.Default;

            if (comparer.Compare(validation.Argument, value) != 0)
                throw new ArgumentOutOfRangeException(string.Concat(validation.ArgumentName, " should be equal ", value));

            return validation;
        }

        public static Validation<T> IsNotEqual<T>(this Validation<T> validation, T value)
            where T : IComparable
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            var comparer = Comparer<T>.Default;

            if (comparer.Compare(validation.Argument, value) == 0)
                throw new ArgumentOutOfRangeException(string.Concat(validation.ArgumentName, " should not be equal ", value));

            return validation;
        }

        public static Validation<bool> IsTrue(this Validation<bool> validation)
        {
            if (!validation.Argument)
                throw new ArgumentOutOfRangeException(string.Concat(validation.ArgumentName, " should be true"));

            return validation;
        }

        public static Validation<bool> IsFalse(this Validation<bool> validation)
        {
            if (validation.Argument)
                throw new ArgumentOutOfRangeException(string.Concat(validation.ArgumentName, " should be false"));

            return validation;
        }

        public static Validation<T> IsOfType<T>(this Validation<T> validation, Type expectedType)
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            var argumentType = validation.Argument as Type ?? validation.Argument.GetType();

            if (argumentType != expectedType)
                throw new ArgumentException(string.Concat(validation.ArgumentName, " should be of Type ", expectedType.Name, " but was ", argumentType.Name));

            return validation;
        }

        public static Validation<T> IsOfAnyType<T>(this Validation<T> validation, IEnumerable<Type> expectedTypes)
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            var argumentType = validation.Argument as Type ?? validation.Argument.GetType();

            var typesArray = expectedTypes as Type[] ?? expectedTypes.ToArray();
            if (typesArray.All(x => x != argumentType))
            {
                var typeNames = typesArray.Select(x => x.Name);
                throw new ArgumentOutOfRangeException(string.Concat(validation.ArgumentName, " should be any of these Types: ", string.Join(",", typeNames), " but was ", argumentType.Name));
            }

            return validation;
        }

        public static Validation<T> IsNotOfType<T>(this Validation<T> validation, Type expectedType)
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            var argumentType = validation.Argument as Type ?? validation.Argument.GetType();

            if (argumentType == expectedType)
                throw new ArgumentException(string.Concat(validation.ArgumentName, " should not be of Type ", expectedType.Name));

            return validation;
        }

        public static Validation<T> IsNotOfAnyType<T>(this Validation<T> validation, IEnumerable<Type> expectedTypes)
        {
            if (validation.Argument == null)
                throw new ArgumentNullException(validation.ArgumentName);

            var argumentType = validation.Argument as Type ?? validation.Argument.GetType();

            if (expectedTypes.Any(x => x == argumentType))
                throw new ArgumentOutOfRangeException(string.Concat(validation.ArgumentName, " should not be of Type ", argumentType.Name));

            return validation;
        }
    }
}
