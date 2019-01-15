using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.Extensions
{
    public static class IEnumerableExtensions
    {
        public static HashSet<string> ToCaseInsensitiveHashSet(this IEnumerable<string> source)
        {
            return new HashSet<string>(source, StringComparer.OrdinalIgnoreCase);
        }

        public static Dictionary<string, TValue> ToCaseInsensitiveDictionary<TValue, TInput>(this IEnumerable<TInput> enumerable, Func<TInput, string> keySelector, Func<TInput, TValue> valueSelector)
        {
            return enumerable.ToDictionary(keySelector, valueSelector, StringComparer.OrdinalIgnoreCase);
        }

        public static IEnumerable<IEnumerable<T>> SplitList<T>(this IEnumerable<T> source, int pageSize)
        {
            var l = source.ToList();

            for (var i = 0; i < l.Count; i += pageSize)
            {
                yield return l.GetRange(i, Math.Min(pageSize, l.Count - i));
            }
        }
    }
}