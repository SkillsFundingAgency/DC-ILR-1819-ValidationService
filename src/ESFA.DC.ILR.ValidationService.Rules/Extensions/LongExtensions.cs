using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Extensions
{
    public static class LongExtensions
    {
        public static IEnumerable<int> ToDigitEnumerable(this long i)
        {
            IList<int> result = new List<int>();

            if (i == 0)
            {
                return new List<int> { 0 };
            }

            while (i != 0)
            {
                result.Add((int)(i % 10));
                i /= 10;
            }

            return result.Reverse();
        }
    }
}
