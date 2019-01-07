using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Extensions
{
    public static class EnumerableExtensions
    {
        public static HashSet<T> ToCaseInsensitiveHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source, StringComparer.OrdinalIgnoreCase as IEqualityComparer<T>);
        }

        /// <summary>
        /// As case insensitive read only dictionary.
        /// ordinal ignore case, because it's faster than it's invariant culture cousin
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>a case insensitive readonly dictionary</returns>
        public static IReadOnlyDictionary<string, TValue> AsCIReadOnlyDictionary<TValue>(this IReadOnlyDictionary<string, TValue> source)
        {
            if (source == null)
            {
                return new Dictionary<string, TValue>(StringComparer.OrdinalIgnoreCase);
            }

            return new Dictionary<string, TValue>(source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), StringComparer.OrdinalIgnoreCase);
        }

        public static Dictionary<string, TValue> ToCaseInsensitiveDictionary<TValue>(this IDictionary<string, TValue> dictionary)
        {
            return new Dictionary<string, TValue>(dictionary, StringComparer.OrdinalIgnoreCase);
        }

        public static Dictionary<string, TValue> ToCaseInsensitiveDictionary<TValue, TInput>(this IEnumerable<TInput> enumerable, Func<TInput, string> keySelector, Func<TInput, TValue> valueSelector)
        {
            return enumerable.ToDictionary(keySelector, valueSelector, StringComparer.OrdinalIgnoreCase);
        }

        public static Task<Dictionary<string, TValue>> ToCaseInsensitiveAsyncDictionary<TValue, TInput>(this IQueryable<TInput> enumerable, Func<TInput, string> keySelector, Func<TInput, TValue> valueSelector, CancellationToken cancellationToken)
        {
            return enumerable.ToDictionaryAsync(keySelector, valueSelector, StringComparer.OrdinalIgnoreCase, cancellationToken);
        }

        public static IEnumerable<IEnumerable<T>> SplitList<T>(this IEnumerable<T> source, int nSize = 30)
        {
            var l = source.ToList();

            for (var i = 0; i < l.Count; i += nSize)
            {
                yield return l.GetRange(i, Math.Min(nSize, l.Count - i)).ToCaseInsensitiveHashSet();
            }
        }
    }
}