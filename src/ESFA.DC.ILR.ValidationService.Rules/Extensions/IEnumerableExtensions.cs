using System.Collections;

namespace ESFA.DC.ILR.ValidationService.Rules.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool HasValue(this IEnumerable enumerable)
        {
            return enumerable != null;
        }
    }
}
