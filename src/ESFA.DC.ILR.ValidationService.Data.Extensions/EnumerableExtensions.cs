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