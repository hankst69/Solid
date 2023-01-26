using System;
using System.Collections.Generic;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Infrastructure.RuntimeTypeExtensions
{
    public static class ListExtensions
    {
        public static int IndexOf<T>(this IList<T> list, Func<T, bool> condition)
        {
            ConsistencyCheck.EnsureArgument(list).IsNotNull();
            ConsistencyCheck.EnsureArgument(condition).IsNotNull();

            for (var i = 0; i < list.Count; i++)
            {
                if (condition(list[i]))
                    return i;
            }
            return -1;
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            ConsistencyCheck.EnsureArgument(list).IsNotNull();
            ConsistencyCheck.EnsureArgument(items).IsNotNull();

            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        public static T[] Range<T>(this IList<T> collection, int startIndex, int endIndex)
        {
            ConsistencyCheck.EnsureArgument(collection).IsNotNull();

            if (startIndex < 0 || startIndex > collection.Count)
                throw new ArgumentOutOfRangeException("startIndex", "startIndex was outside of the range of the collection");
            if (endIndex < 0 || endIndex > collection.Count)
                throw new ArgumentOutOfRangeException("endIndex", "endIndex  was outside of the range of the collection");
            if (endIndex < startIndex)
                throw new ArgumentException("endIndex needs to be larger than startIndex");

            var result = new T[endIndex - startIndex + 1];
            for (int i = startIndex; i <= endIndex; i++)
            {
                result[i - startIndex] = collection[i];
            }
            return result;
        }
    }
}
