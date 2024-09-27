//----------------------------------------------------------------------------------
// File: "EnumerableExtensions.cs"
// Author: Steffen Hanke
// Date: 2016-2020
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Infrastructure.RuntimeTypeExtensions
{
    public static class EnumerableExtensions
    {
        public static IList<TSource> ToIList<TSource>(this IEnumerable<TSource> source)
        {
            ConsistencyCheck.EnsureArgument(source).IsNotNull();

            return source as IList<TSource> ?? Enumerable.ToList(source);
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            ConsistencyCheck.EnsureArgument(items).IsNotNull();
            ConsistencyCheck.EnsureArgument(action).IsNotNull();

            foreach (var item in items)
            {
                action(item);
            }
        }

        public static bool SequenceEquivalent<T>(this IEnumerable<T> items, IEnumerable<T> otherItems)
        {
            return SequenceEquivalent(items, otherItems, (arg1, arg2) => arg1.Equals(arg2));
        }

        public static bool SequenceEquivalent<T>(this IEnumerable<T> items, IEnumerable<T> otherItems, IEqualityComparer<T> comparer)
        {
            ConsistencyCheck.EnsureArgument(comparer).IsNotNull();

            return SequenceEquivalent(items, otherItems, comparer.Equals);
        }

        public static bool SequenceEquivalent<T>(this IEnumerable<T> items, IEnumerable<T> otherItems, Func<T, T, bool> equalityPredicate)
        {
            ConsistencyCheck.EnsureArgument(equalityPredicate).IsNotNull();

            if (ReferenceEquals(items, otherItems))
            {
                return true;
            }

            if (items == null && otherItems == null)
            {
                return true;
            }

            if (items == null || otherItems == null)
            {
                return false;
            }

            var itemList = items as IList<T> ?? items.ToList();
            var otherItemList = otherItems.ToList();

            if (itemList.Count != otherItemList.Count)
            {
                return false;
            }

            foreach (var item in itemList)
            {
                var found = false;
                for (var j = 0; j < otherItemList.Count; j++)
                {
                    if (!equalityPredicate(otherItemList[j], item))
                    {
                        continue;
                    }
                    found = true;
                    otherItemList.RemoveAt(j);
                    break;
                }

                if (!found)
                {
                    return false;
                }
            }

            return true;
        }
/*
        public static T MaxElementBy<T>(this IEnumerable<T> items, Func<T, double> weightFunc)
        {
            Condition.Requires(items, "items")
                     .IsNotNull()
                     .IsNotEmpty();
            Condition.Requires(weightFunc, "weightFunc")
                     .IsNotNull();

            return items.Aggregate((i1, i2) => weightFunc(i1) > weightFunc(i2) ? i1 : i2);
        }

        public static T MinElementBy<T>(this IEnumerable<T> items, Func<T, double> weightFunc)
        {
            Condition.Requires(items, "items")
                     .IsNotNull()
                     .IsNotEmpty();
            Condition.Requires(weightFunc, "weightFunc")
                     .IsNotNull();

            return items.MaxElementBy(i => (-1) * weightFunc(i));
        }
*/
    }
}