using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class InternalDataCacheProviderServiceStub : IValidationItemProviderService<IInternalDataCache>
    {
        private readonly IInternalDataCache _internalDataCache;

        public InternalDataCacheProviderServiceStub(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        public IInternalDataCache Provide()
        {
            return (InternalDataCache)_internalDataCache;
        }
    }
}
