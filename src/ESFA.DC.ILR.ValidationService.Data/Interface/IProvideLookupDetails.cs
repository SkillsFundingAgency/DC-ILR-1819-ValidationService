using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    /// <summary>
    /// i provide lookup details
    /// </summary>
    public interface IProvideLookupDetails
    {
        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(TypeOfIntegerCodedLookup lookupKey, int candidate);

        /// <summary>
        /// As a set.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <returns>the domain of values pertinent to the coded lookup key</returns>
        IReadOnlyCollection<int> AsASet(TypeOfIntegerCodedLookup lookupKey);

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(TypeOfStringCodedLookup lookupKey, string candidate);

        /// <summary>
        /// As a set.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <returns>the domain of values pertinent to the coded lookup key</returns>
        IReadOnlyCollection<string> AsASet(TypeOfStringCodedLookup lookupKey);

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(TypeOfLimitedLifeLookup lookupKey, int candidate);

        /// <summary>
        /// Determines whether [contains] [the specified lookup key].
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified lookup key]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(TypeOfLimitedLifeLookup lookupKey, string candidate);

        /// <summary>
        /// Determines whether [contains] [for the specified lookup] [the key and value].
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="keyCandidate">The key candidate.</param>
        /// <param name="valueCandidate">The value candidate.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified lookup key]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(TypeOfListItemLookup lookupKey, string keyCandidate, string valueCandidate);

        /// <summary>
        /// Determines whether [the specified lookup key] is current on the given date
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        ///   <c>true</c> if [the specified lookup] [is current] for the given date; otherwise, <c>false</c>.
        /// </returns>
        bool IsCurrent(TypeOfLimitedLifeLookup lookupKey, int candidate, DateTime referenceDate);

        /// <summary>
        /// Determines whether the specified candidate lookup is current.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        ///   <c>true</c> if the specified lookup key is current; otherwise, <c>false</c>.
        /// </returns>
        bool IsCurrent(TypeOfLimitedLifeLookup lookupKey, string candidate, DateTime referenceDate);
    }
}
