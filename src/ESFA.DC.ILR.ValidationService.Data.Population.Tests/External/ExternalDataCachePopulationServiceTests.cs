using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class ExternalDataCachePopulationServiceTests
    {
        private ExternalDataCachePopulationService NewService(
            IExternalDataCache externalDataCache = null,
            ILARSStandardValidityDataRetrievalService larsStandardValidityDataRetrievalService = null,
            ILARSLearningDeliveryDataRetrievalService larsLearningDeliveryDataRetrievalService = null,
            ILARSFrameworkDataRetrievalService larsFrameworkDataRetrievalService = null,
            IULNDataRetrievalService ulnDataRetrievalService = null,
            IPostcodesDataRetrievalService postcodesDataRetrievalService = null,
            IOrganisationsDataRetrievalService organisationsDataRetrievalService = null,
            IFCSDataRetrievalService fcsDataRetrievalService = null)
        {
            return new ExternalDataCachePopulationService(
                externalDataCache,
                larsStandardValidityDataRetrievalService,
                larsLearningDeliveryDataRetrievalService,
                larsFrameworkDataRetrievalService,
                ulnDataRetrievalService,
                postcodesDataRetrievalService,
                organisationsDataRetrievalService,
                fcsDataRetrievalService);
        }
    }
}
