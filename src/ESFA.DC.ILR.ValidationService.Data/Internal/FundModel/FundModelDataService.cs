using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Internal.FundModel.Interface;
using IInternalDataCache = ESFA.DC.ILR.ValidationService.Data.Interface.IInternalDataCache;

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
