using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.Organisation
{
    public class OrganisationDataService : IOrganisationDataService
    {
        private readonly IExternalDataCache _referenceDataCache;

        public OrganisationDataService(IExternalDataCache referenceDataCache)
        {
            _referenceDataCache = referenceDataCache;
        }

        public bool LegalOrgTypeMatchForUkprn(long ukprn, string legalOrgType)
        {
            _referenceDataCache.Organisations.TryGetValue(ukprn, out var organisation);

            return organisation != null && organisation.LegalOrgType == legalOrgType;
        }

        public bool UkprnExists(long ukprn)
        {
            return _referenceDataCache.Organisations.ContainsKey(ukprn);
        }
    }
}
