using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
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
        private readonly IValidationErrorsDataRetrievalService _validationErrorsDataRetrievalService;
        private readonly ICache<IMessage> _messageCache;

        public ExternalDataCachePopulationService(
            IExternalDataCache externalDataCache,
            ILARSLearningDeliveryDataRetrievalService larsLearningDeliveryDataRetrievalService,
            ILARSFrameworkDataRetrievalService larsFrameworkDataRetrievalService,
            IULNDataRetrievalService ulnDataRetrievalService,
            IPostcodesDataRetrievalService postcodesDataRetrievalService,
            IOrganisationsDataRetrievalService organisationsDataRetrievalService,
            IValidationErrorsDataRetrievalService validationErrorsDataRetrievalService,
            ICache<IMessage> messageCache)
        {
            _externalDataCache = externalDataCache;
            _larsLearningDeliveryDataRetrievalService = larsLearningDeliveryDataRetrievalService;
            _larsFrameworkDataRetrievalService = larsFrameworkDataRetrievalService;
            _ulnDataRetrievalService = ulnDataRetrievalService;
            _postcodesDataRetrievalService = postcodesDataRetrievalService;
            _organisationsDataRetrievalService = organisationsDataRetrievalService;
            _validationErrorsDataRetrievalService = validationErrorsDataRetrievalService;
            _messageCache = messageCache;
        }

        public void Populate()
        {
            var externalDataCache = (ExternalDataCache)_externalDataCache;

            externalDataCache.LearningDeliveries = _larsLearningDeliveryDataRetrievalService.Retrieve();
            externalDataCache.Frameworks = _larsFrameworkDataRetrievalService.Retrieve().ToList();
            externalDataCache.ULNs = new HashSet<long>(_ulnDataRetrievalService.Retrieve());
            externalDataCache.Postcodes = new HashSet<string>(_postcodesDataRetrievalService.Retrieve());
            externalDataCache.Organisations = _organisationsDataRetrievalService.Retrieve();
            externalDataCache.ValidationErrors = _validationErrorsDataRetrievalService.Retrieve();
        }
    }
}
