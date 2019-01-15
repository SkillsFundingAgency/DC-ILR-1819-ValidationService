using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Extensions
{
    public static class IQueryableExtensions
    {
        public static Task<Dictionary<string, TValue>> ToCaseInsensitiveAsyncDictionary<TValue, TInput>(this IQueryable<TInput> enumerable, Func<TInput, string> keySelector, Func<TInput, TValue> valueSelector, CancellationToken cancellationToken)
        {
            return enumerable.ToDictionaryAsync(keySelector, valueSelector, StringComparer.OrdinalIgnoreCase, cancellationToken);
        }
    }
}