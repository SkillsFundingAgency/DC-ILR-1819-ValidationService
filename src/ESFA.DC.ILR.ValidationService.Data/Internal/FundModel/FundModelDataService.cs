using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.FundModel.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.FundModel
{
    public class FundModelDataService : IFundModelDataService
    {
        private readonly IInternalDataCache _internalDataCache;

        public FundModelDataService(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        public bool Exists(int fundModel)
        {
            return _internalDataCache.FundModels.Contains(fundModel);
        }
    }
}
