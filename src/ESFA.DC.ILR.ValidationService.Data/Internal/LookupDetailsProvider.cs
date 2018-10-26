﻿using ESFA.DC.ILR.ValidationService.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data
{
    /// <summary>
    /// the lookup details provider
    /// </summary>
    /// <seealso cref="IProvideLookupDetails" />
    public sealed class LookupDetailsProvider :
        IProvideLookupDetails
    {
        private IInternalDataCache _internalCache;
        private ICreateInternalDataCache _cacheFactory;

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
            return InternalCache.CodedLookups[lookupKey].Contains(candidate);
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
            return Contains(lookupKey, $"{candidate}");
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
            return InternalCache.LimitedLifeLookups[lookupKey].ContainsKey(candidate);
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
            return IsCurrent(lookupKey, $"{candidate}", referenceDate);
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
                && InternalCache.LimitedLifeLookups[lookupKey]
                    .Where(x => x.Key == candidate)
                    .Any(y => IsBetween(y.Value.ValidFrom, y.Value.ValidTo, referenceDate));
        }

        /// <summary>
        /// Checks whether the key exists and specified date (LearnStartDate) is less than valid to date of key.
        /// </summary>
        /// <param name="lookupKey">The Lookup list key</param>
        /// <param name="candidate">The value to find in lookup list</param>
        /// <param name="dateToCheck">The date (LearnStartDate) to compare</param>
        /// <returns>
        ///    <c>true</c> if the specified candidate key exist in lookup list and dateToCheck is before or equal to ValidTo date; otherwise, <c>false</c>.
        /// </returns>
        public bool IsContainsAndDateBeforeValidToDate(LookupTimeRestrictedKey lookupKey, string candidate, DateTime dateToCheck)
        {
            return Contains(lookupKey, candidate)
                && (dateToCheck <= InternalCache.LimitedLifeLookups[lookupKey]
                    .FirstOrDefault(k => k.Key == candidate)
                    .Value.ValidTo);
        }
    }
}
