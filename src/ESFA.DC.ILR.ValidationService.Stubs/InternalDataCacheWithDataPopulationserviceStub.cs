using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class InternalDataCacheWithDataPopulationserviceStub : IInternalDataCacheWithDataPopulationService
    {
        private IInternalDataCache _internalDataCache;

        public InternalDataCacheWithDataPopulationserviceStub(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        public void Populate(IInternalDataCache data)
        {
            _internalDataCache = data;
        }
    }
}
