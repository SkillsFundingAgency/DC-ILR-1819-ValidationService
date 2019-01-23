using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
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
        bool Contains(LookupSimpleKey lookupKey, int candidate);

        /// <summary>
        /// As a set.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <returns>the domain of values pertinent to the coded lookup key</returns>
        IReadOnlyCollection<int> AsASet(LookupSimpleKey lookupKey);

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(LookupCodedKey lookupKey, string candidate);

        /// <summary>
        /// As a set.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <returns>the domain of values pertinent to the coded lookup key</returns>
        IReadOnlyCollection<string> AsASet(LookupCodedKey lookupKey);

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(LookupTimeRestrictedKey lookupKey, int candidate);

        /// <summary>
        /// Determines whether [contains] [the specified lookup key].
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified lookup key]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(LookupTimeRestrictedKey lookupKey, string candidate);

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="keyCandidate">The dictionary key candidate.</param>
        /// <param name="valueCandidate">The dictionary value candidate.</param>
        /// <returns>
        /// <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsValueForKey(LookupCodedKeyDictionary lookupKey, string keyCandidate, string valueCandidate);

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="keyCandidate">The key candidate.</param>
        /// <param name="valueCandidate">The value candidate.</param>
        /// <returns>
        ///   <c>true</c> if [contains value for key] [the specified lookup key]; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsValueForKey(LookupCodedKeyDictionary lookupKey, string keyCandidate, int valueCandidate);

        /// <summary>
        /// Determines whether [the specified lookup key] is current on the given date
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        ///   <c>true</c> if [the specified lookup] [is current] for the given date; otherwise, <c>false</c>.
        /// </returns>
        bool IsCurrent(LookupTimeRestrictedKey lookupKey, int candidate, DateTime referenceDate);

        /// <summary>
        /// Determines whether the specified lookup key is current.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        ///   <c>true</c> if the specified lookup key is current; otherwise, <c>false</c>.
        /// </returns>
        bool IsCurrent(LookupTimeRestrictedKey lookupKey, string candidate, DateTime referenceDate);

        bool IsCurrent(LookupComplexKey lookupKey, string key, string value, DateTime date);

        bool ContainsValueForKey(LookupItemKey lookupKey, string keyCandidate, string valueCandidate);
    }
}
