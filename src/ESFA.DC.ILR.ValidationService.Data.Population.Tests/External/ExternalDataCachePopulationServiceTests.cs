using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.Data.Postcodes.Model.Interfaces;
using ESFA.DC.Data.ULN.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Keys;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class ExternalDataCachePopulationServiceTests
    {

        
        private ExternalDataCachePopulationService NewService(
            IExternalDataCache externalDataCache = null,
            ILARSLearningDeliveryDataRetrievalService larsLearningDeliveryDataRetrievalService = null,
            ILARSFrameworkDataRetrievalService larsFrameworkDataRetrievalService = null,
            IULNDataRetrievalService ulnDataRetrievalService = null,
            IPostcodesDataRetrievalService postcodesDataRetrievalService = null)
        {
            return new ExternalDataCachePopulationService(
                externalDataCache,
                larsLearningDeliveryDataRetrievalService,
                larsFrameworkDataRetrievalService,
                ulnDataRetrievalService,
                postcodesDataRetrievalService);
        }
    }
}
