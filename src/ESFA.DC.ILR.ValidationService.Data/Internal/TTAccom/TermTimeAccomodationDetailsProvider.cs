using ESFA.DC.ILR.ValidationService.Data.Interface;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.TTAccom
{
    /// <summary>
    /// the term time accomodation details provider
    /// </summary>
    /// <seealso cref="IProvideTermTimeAccomodationDetails" />
    public sealed class TermTimeAccomodationDetailsProvider :
        IProvideTermTimeAccomodationDetails
    {
        private readonly IInternalDataCache _internalDataCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermTimeAccomodationDetailsProvider"/> class.
        /// </summary>
        /// <param name="internalDataCache">The internal data cache.</param>
        public TermTimeAccomodationDetailsProvider(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        /// <summary>
        /// Determines whether [contains] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(int candidate)
        {
            return _internalDataCache.TTAccoms.ContainsKey(candidate);
        }

        /// <summary>
        /// Determines whether the specified candidate is current.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="reference">The reference.</param>
        /// <returns>
        /// <c>true</c> if the specified candidate is current; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrent(int candidate, DateTime reference)
        {
            return Contains(candidate)
                && _internalDataCache.TTAccoms
                    .Where(x => x.Key == candidate)
                    .Any(y => IsBetween(y.Value.ValidFrom, y.Value.ValidTo, reference));
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
    }
}
