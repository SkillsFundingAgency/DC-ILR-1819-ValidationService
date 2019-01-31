using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data
{
    /// <summary>
    /// the lookup details provider
    /// </summary>
    /// <seealso cref="IProvideLookupDetails" />
    public sealed class LookupDetailsProvider :
        IProvideLookupDetails
    {
        private readonly ICreateInternalDataCache _cacheFactory;
        private IInternalDataCache _internalCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="LookupDetailsProvider " /> class.
        /// </summary>
        /// <param name="cacheFactory">The cache factory.</param>
        public LookupDetailsProvider(ICreateInternalDataCache cacheFactory)
        {
            _cacheFactory = cacheFactory;
        }

        /// <summary>
        /// Gets the internal cache.
        /// </summary>
        public IInternalDataCache InternalCache
        {
            get
            {
                return _internalCache
                    ?? (_internalCache = _cacheFactory.Create());
            }
        }

        /// <summary>
        /// Determines whether the specified from date is between.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if the specified from date is between; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBetween(DateTime fromDate, DateTime toDate, DateTime candidate) => (candidate >= fromDate) && (candidate <= toDate);

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(LookupSimpleKey lookupKey, int candidate)
        {
            return InternalCache.SimpleLookups[lookupKey].Contains(candidate);
        }

        /// <summary>
        /// As a set.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <returns>the domain of values pertinent to the coded lookup key</returns>
        public IReadOnlyCollection<int> AsASet(LookupSimpleKey lookupKey)
        {
            return InternalCache.SimpleLookups[lookupKey];
        }

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(LookupCodedKey lookupKey, string candidate)
        {
            return !string.IsNullOrEmpty(candidate) &&
                   InternalCache.CodedLookups[lookupKey].Contains(candidate, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// As a set.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <returns>the domain of values pertinent to the coded lookup key</returns>
        public IReadOnlyCollection<string> AsASet(LookupCodedKey lookupKey)
        {
            return InternalCache.CodedLookups[lookupKey];
        }

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(LookupTimeRestrictedKey lookupKey, int candidate)
        {
            return Contains(lookupKey, candidate.ToString());
        }

        /// <summary>
        /// Determines whether [contains] [the specified lookup key].
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified lookup key]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(LookupTimeRestrictedKey lookupKey, string candidate)
        {
            return !string.IsNullOrEmpty(candidate)
                   && InternalCache.LimitedLifeLookups[lookupKey].ContainsKey(candidate);
        }

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="keyCandidate">The dictionary key candidate.</param>
        /// <param name="valueCandidate">The dictionary value candidate.</param>
        /// <returns>
        /// <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsValueForKey(LookupCodedKeyDictionary lookupKey, string keyCandidate, int valueCandidate)
        {
            return ContainsValueForKey(lookupKey, keyCandidate, valueCandidate.ToString());
        }

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="keyCandidate">The dictionary key candidate.</param>
        /// <param name="valueCandidate">The dictionary value candidate.</param>
        /// <returns>
        /// <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsValueForKey(LookupCodedKeyDictionary lookupKey, string keyCandidate, string valueCandidate)
        {
            return !string.IsNullOrEmpty(keyCandidate) &&
                   InternalCache.CodedDictionaryLookups[lookupKey].TryGetValue(keyCandidate, out IReadOnlyCollection<string> value)
                   && value.Any(x => x.CaseInsensitiveEquals(valueCandidate));
        }

        public bool ContainsValueForKey(LookupItemKey lookupKey, string keyCandidate, string valueCandidate)
        {
            return !string.IsNullOrEmpty(keyCandidate) &&
                   InternalCache.ItemLookups[lookupKey].TryGetValue(keyCandidate, out IReadOnlyCollection<string> value)
                   && value.Any(x => x.CaseInsensitiveEquals(valueCandidate));
        }

        public bool ContainsValueForKey(LookupComplexKey lookupKey, string keyCandidate, string valueCandidate)
        {
            return !string.IsNullOrEmpty(keyCandidate) && !string.IsNullOrEmpty(valueCandidate) &&
                InternalCache.CodedComplexLookups[lookupKey].TryGetValue(keyCandidate, out var value)
                    && value.ContainsKey(valueCandidate);
        }

        /// <summary>
        /// Determines whether [the specified lookup key] is current on the given date
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        /// <c>true</c> if [the specified lookup] [is current] for the given date; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrent(LookupTimeRestrictedKey lookupKey, int candidate, DateTime referenceDate)
        {
            return IsCurrent(lookupKey, candidate.ToString(), referenceDate);
        }

        /// <summary>
        /// Determines whether the specified lookup key is current.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        /// <c>true</c> if the specified lookup key is current; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrent(LookupTimeRestrictedKey lookupKey, string candidate, DateTime referenceDate)
        {
            return Contains(lookupKey, candidate)
                && InternalCache.LimitedLifeLookups[lookupKey].TryGetValue(candidate, out var value)
                && IsBetween(value.ValidFrom, value.ValidTo, referenceDate);
        }

        public bool IsCurrent(LookupComplexKey lookupKey, string key, string value, DateTime date)
        {
            return ContainsValueForKey(lookupKey, key, value) &&
                   InternalCache.CodedComplexLookups[lookupKey][key]
                       .Any(y => IsBetween(y.Value.ValidFrom, y.Value.ValidTo, date));
        }
    }
}
