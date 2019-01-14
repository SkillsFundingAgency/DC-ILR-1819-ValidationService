using System;
using System.Globalization;

namespace ESFA.DC.ILR.ValidationService.Utility
{
    /// <summary>
    /// Class Format, performs an invariant culture string formatting
    /// </summary>
    public static class Format
    {
        /// <summary>
        /// Compares (the item) with any candidate.
        /// this is a case and culture insensitive comparison
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="anyCandidate">Any candidate.</param>
        /// <returns>true if the item matches any of the candididates</returns>
        /// <exception cref="ArgumentNullException">if there aren't any Candidate(s)</exception>
        public static bool ComparesWith(this string item, params string[] anyCandidate)
        {
            It.IsNull(anyCandidate)
                .AsGuard<ArgumentNullException>(nameof(anyCandidate));

            foreach (var candidate in anyCandidate)
            {
                var result = string.Compare(item, candidate, StringComparison.InvariantCultureIgnoreCase) == 0;
                if (result)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
