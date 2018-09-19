using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation
{
    /// <summary>
    /// the EPA organisation details provider
    /// </summary>
    /// <seealso cref="IProvideEPAOrganisationDetails" />
    public class EPAOrganisationDetailsProvider :
        IProvideEPAOrganisationDetails
    {
        /// <summary>
        /// The reference data cache
        /// </summary>
        private IReadOnlyCollection<IEPAOrganisation> _referenceDataCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="EPAOrganisationDetailsProvider"/> class.
        /// </summary>
        /// <param name="referenceDataCache">The reference data cache.</param>
        public EPAOrganisationDetailsProvider(IExternalDataCache referenceDataCache)
        {
            _referenceDataCache = referenceDataCache.EPAOrganisations.AsSafeReadOnlyList();
        }

        /// <summary>
        /// Fetches the specified candidate identifier.
        /// </summary>
        /// <param name="candidateID">The candidate identifier.</param>
        /// <returns>
        /// the epa organisation details
        /// </returns>
        public IEPAOrganisation Fetch(string candidateID)
        {
            return _referenceDataCache.FirstOrDefault(x => x.ID == candidateID);
        }

        /// <summary>
        /// Determines whether the specified candidate identifier is current.
        /// </summary>
        /// <param name="candidateID">The candidate identifier.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        /// <c>true</c> if the specified candidate identifier is current; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrent(string candidateID, DateTime referenceDate)
        {
            var organisation = Fetch(candidateID);

            return It.Has(organisation)
                && It.IsBetween(referenceDate, organisation.EffectiveFrom, organisation.EffectiveTo);
        }

        /// <summary>
        /// Determines whether the specified candidate identifier is known.
        /// </summary>
        /// <param name="candidateID">The candidate identifier.</param>
        /// <returns>
        /// <c>true</c> if the specified candidate identifier is known; otherwise, <c>false</c>.
        /// </returns>
        public bool IsKnown(string candidateID)
        {
            // TODO: CME, fix this once the cache is done...
            // return _referenceDataCache.Any(x => x.ID == candidateID);
            return true;
        }
    }
}
