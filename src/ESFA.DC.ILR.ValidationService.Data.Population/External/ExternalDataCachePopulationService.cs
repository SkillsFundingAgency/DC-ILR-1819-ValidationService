using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class ExternalDataCachePopulationService : IExternalDataCachePopulationService
    {
        private readonly IExternalDataCache _externalDataCache;
        private readonly ILARSStandardDataRetrievalService _larsStandardDataRetrievalService;
        private readonly ILARSStandardValidityDataRetrievalService _larsStandardValidityDataRetrievalService;
        private readonly ILARSLearningDeliveryDataRetrievalService _larsLearningDeliveryDataRetrievalService;
        private readonly ILARSFrameworkDataRetrievalService _larsFrameworkDataRetrievalService;
        private readonly IULNDataRetrievalService _ulnDataRetrievalService;
        private readonly IPostcodesDataRetrievalService _postcodesDataRetrievalService;
        private readonly IOrganisationsDataRetrievalService _organisationsDataRetrievalService;
        private readonly ICampusIdentifierDataRetrievalService _campusIdentifierDataRetrievalService;
        private readonly IFCSDataRetrievalService _fcsDataRetrievalService;

        public ExternalDataCachePopulationService(
            IExternalDataCache externalDataCache,
            ILARSStandardDataRetrievalService larsStandardDataRetrievalService,
            ILARSStandardValidityDataRetrievalService larsStandardValidityDataRetrievalService,
            ILARSLearningDeliveryDataRetrievalService larsLearningDeliveryDataRetrievalService,
            ILARSFrameworkDataRetrievalService larsFrameworkDataRetrievalService,
            IULNDataRetrievalService ulnDataRetrievalService,
            IPostcodesDataRetrievalService postcodesDataRetrievalService,
            IOrganisationsDataRetrievalService organisationsDataRetrievalService,
            ICampusIdentifierDataRetrievalService campusIdentifierDataRetrievalService,
            IFCSDataRetrievalService fcsDataRetrievalService)
        {
            _externalDataCache = externalDataCache;
            _larsStandardDataRetrievalService = larsStandardDataRetrievalService;
            _larsStandardValidityDataRetrievalService = larsStandardValidityDataRetrievalService;
            _larsLearningDeliveryDataRetrievalService = larsLearningDeliveryDataRetrievalService;
            _larsFrameworkDataRetrievalService = larsFrameworkDataRetrievalService;
            _ulnDataRetrievalService = ulnDataRetrievalService;
            _postcodesDataRetrievalService = postcodesDataRetrievalService;
            _organisationsDataRetrievalService = organisationsDataRetrievalService;
            _campusIdentifierDataRetrievalService = campusIdentifierDataRetrievalService;
            _fcsDataRetrievalService = fcsDataRetrievalService;
        }

        public async Task PopulateAsync(CancellationToken cancellationToken)
        {
            var externalDataCache = (ExternalDataCache)_externalDataCache;

            externalDataCache.Standards = await _larsStandardDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.StandardValidities = await _larsStandardValidityDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.LearningDeliveries = await _larsLearningDeliveryDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.Frameworks = await _larsFrameworkDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.ULNs = new HashSet<long>(await _ulnDataRetrievalService.RetrieveAsync(cancellationToken));

            externalDataCache.Postcodes = (await _postcodesDataRetrievalService.RetrieveAsync(cancellationToken)).ToCaseInsensitiveHashSet();
            externalDataCache.ONSPostcodes = await _postcodesDataRetrievalService.RetrieveONSPostcodesAsync(cancellationToken);

            externalDataCache.Organisations = await _organisationsDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.CampusIdentifiers = await _campusIdentifierDataRetrievalService.RetrieveAsync(cancellationToken);

            externalDataCache.FCSContracts = await _fcsDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.FCSContractAllocations = await _fcsDataRetrievalService.RetrieveContractAllocationsAsync(cancellationToken);
            externalDataCache.ESFEligibilityRuleLocalAuthorities = await _fcsDataRetrievalService.RetrieveEligibilityRuleLocalAuthoritiesAsync(cancellationToken);
            externalDataCache.ESFEligibilityRuleEnterprisePartnerships = await _fcsDataRetrievalService.RetrieveEligibilityRuleEnterprisePartnershipsAsync(cancellationToken);
            externalDataCache.ESFEligibilityRuleEmploymentStatuses = await _fcsDataRetrievalService.RetrieveEligibilityRuleEmploymentStatusesAsync(cancellationToken);
            externalDataCache.EsfEligibilityRuleSectorSubjectAreaLevels = await _fcsDataRetrievalService.RetrieveEligibilityRuleSectorSubjectAreaLevelAsync(cancellationToken);

            // TODO: FIX ME!!! this is sanjeev's 'test' emp id...
            externalDataCache.ERNs = new List<int> { 154549452 };
        }
    }
}
