using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class ExternalDataCachePopulationServiceTests
    {
        private ExternalDataCachePopulationService NewService(
            IExternalDataCache externalDataCache = null,
            ILARSLearningDeliveryDataRetrievalService larsLearningDeliveryDataRetrievalService = null,
            ILARSFrameworkDataRetrievalService larsFrameworkDataRetrievalService = null,
            IULNDataRetrievalService ulnDataRetrievalService = null,
            IPostcodesDataRetrievalService postcodesDataRetrievalService = null,
            IValidationErrorsDataRetrievalService validationErrorsDataRetrievalService = null)
        {
            return new ExternalDataCachePopulationService(
                externalDataCache,
                larsLearningDeliveryDataRetrievalService,
                larsFrameworkDataRetrievalService,
                ulnDataRetrievalService,
                postcodesDataRetrievalService,
                validationErrorsDataRetrievalService);
        }
    }
}
