using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class ExternalDataCachePopulationService : IExternalDataCachePopulationService
    {
        private readonly IExternalDataCache _externalDataCache;
        private readonly ILARSLearningDeliveryDataRetrievalService _larsLearningDeliveryDataRetrievalService;
        private readonly ILARSFrameworkDataRetrievalService _larsFrameworkDataRetrievalService;
        private readonly IULNDataRetrievalService _ulnDataRetrievalService;
        private readonly IPostcodesDataRetrievalService _postcodesDataRetrievalService;
        private readonly IOrganisationsDataRetrievalService _organisationsDataRetrievalService;

        public ExternalDataCachePopulationService(
            IExternalDataCache externalDataCache,
            ILARSLearningDeliveryDataRetrievalService larsLearningDeliveryDataRetrievalService,
            ILARSFrameworkDataRetrievalService larsFrameworkDataRetrievalService,
            IULNDataRetrievalService ulnDataRetrievalService,
            IPostcodesDataRetrievalService postcodesDataRetrievalService,
            IOrganisationsDataRetrievalService organisationsDataRetrievalService)
        {
            _externalDataCache = externalDataCache;
            _larsLearningDeliveryDataRetrievalService = larsLearningDeliveryDataRetrievalService;
            _larsFrameworkDataRetrievalService = larsFrameworkDataRetrievalService;
            _ulnDataRetrievalService = ulnDataRetrievalService;
            _postcodesDataRetrievalService = postcodesDataRetrievalService;
            _organisationsDataRetrievalService = organisationsDataRetrievalService;
        }

        public async Task PopulateAsync(CancellationToken cancellationToken)
        {
            var externalDataCache = (ExternalDataCache)_externalDataCache;

            externalDataCache.LearningDeliveries = await _larsLearningDeliveryDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.Frameworks = await _larsFrameworkDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.ULNs = new HashSet<long>(await _ulnDataRetrievalService.RetrieveAsync(cancellationToken));
            externalDataCache.Postcodes = new HashSet<string>(await _postcodesDataRetrievalService.RetrieveAsync(cancellationToken));
            externalDataCache.Organisations = await _organisationsDataRetrievalService.RetrieveAsync(cancellationToken);
        }
    }
}
