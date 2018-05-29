using System;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class ExternalDataCacheWithDataPopulationServiceStub : IExternalDataCacheWithDataPopulationService
    {
        private IExternalDataCache _externalDataCache;

        public ExternalDataCacheWithDataPopulationServiceStub(IExternalDataCache externalDataCache)
        {
            _externalDataCache = externalDataCache;
        }

        public void Populate(IExternalDataCache data)
        {
            _externalDataCache = data;
        }
    }
}
