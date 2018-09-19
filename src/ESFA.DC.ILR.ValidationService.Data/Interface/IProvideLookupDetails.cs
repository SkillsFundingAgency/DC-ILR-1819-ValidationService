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
        /// As a set.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <returns>the domain of values pertinent to the coded lookup key</returns>
        IDictionary<int, ValidityPeriods> AsASet(LookupTimeRestrictedKey lookupKey);

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
    }
}
