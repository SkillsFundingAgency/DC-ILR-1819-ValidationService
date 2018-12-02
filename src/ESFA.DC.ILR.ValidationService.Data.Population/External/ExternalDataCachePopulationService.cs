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
        private readonly ILARSStandardValidityDataRetrievalService _larsStandardValidityDataRetrievalService;
        private readonly ILARSLearningDeliveryDataRetrievalService _larsLearningDeliveryDataRetrievalService;
        private readonly ILARSFrameworkDataRetrievalService _larsFrameworkDataRetrievalService;
        private readonly IULNDataRetrievalService _ulnDataRetrievalService;
        private readonly IPostcodesDataRetrievalService _postcodesDataRetrievalService;
        private readonly IOrganisationsDataRetrievalService _organisationsDataRetrievalService;
        private readonly IFCSDataRetrievalService _fcsDataRetrievalService;

        public ExternalDataCachePopulationService(
            IExternalDataCache externalDataCache,
            ILARSStandardValidityDataRetrievalService larsStandardValidityDataRetrievalService,
            ILARSLearningDeliveryDataRetrievalService larsLearningDeliveryDataRetrievalService,
            ILARSFrameworkDataRetrievalService larsFrameworkDataRetrievalService,
            IULNDataRetrievalService ulnDataRetrievalService,
            IPostcodesDataRetrievalService postcodesDataRetrievalService,
            IOrganisationsDataRetrievalService organisationsDataRetrievalService,
            IFCSDataRetrievalService fcsDataRetrievalService)
        {
            _externalDataCache = externalDataCache;
            _larsStandardValidityDataRetrievalService = larsStandardValidityDataRetrievalService;
            _larsLearningDeliveryDataRetrievalService = larsLearningDeliveryDataRetrievalService;
            _larsFrameworkDataRetrievalService = larsFrameworkDataRetrievalService;
            _ulnDataRetrievalService = ulnDataRetrievalService;
            _postcodesDataRetrievalService = postcodesDataRetrievalService;
            _organisationsDataRetrievalService = organisationsDataRetrievalService;
            _fcsDataRetrievalService = fcsDataRetrievalService;
        }

        public async Task PopulateAsync(CancellationToken cancellationToken)
        {
            var externalDataCache = (ExternalDataCache)_externalDataCache;

            externalDataCache.StandardValidities = await _larsStandardValidityDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.LearningDeliveries = await _larsLearningDeliveryDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.Frameworks = await _larsFrameworkDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.ULNs = new HashSet<long>(await _ulnDataRetrievalService.RetrieveAsync(cancellationToken));
            externalDataCache.Postcodes = new HashSet<string>(await _postcodesDataRetrievalService.RetrieveAsync(cancellationToken));
            externalDataCache.Organisations = await _organisationsDataRetrievalService.RetrieveAsync(cancellationToken);

            externalDataCache.FCSContracts = await _fcsDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.FCSContractAllocations = await _fcsDataRetrievalService.RetrieveContractAllocationsAsync(cancellationToken);
            externalDataCache.ESFEligibilityRuleEmploymentStatuses = await _fcsDataRetrievalService.RetrieveEligibilityRuleEmploymentStatusesAsync(cancellationToken);
            externalDataCache.EsfEligibilityRuleSectorSubjectAreaLevels = await _fcsDataRetrievalService.RetrieveEligibilityRuleSectorSubjectAreaLevelAsync(cancellationToken);

            // TODO: FIX ME!!! this is sanjeev's 'test' emp id...
            externalDataCache.ERNs = new List<int> { 154549452 };
        }
    }
}
