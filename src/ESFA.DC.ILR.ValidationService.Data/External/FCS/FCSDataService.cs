using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS
{
    public class FCSDataService : IFCSDataService
    {
        private readonly IExternalDataCache _externalDataCache;

        public FCSDataService(IExternalDataCache externalDataCache)
        {
            _externalDataCache = externalDataCache;
        }

        public bool ConRefNumberExists(string conRefNumber)
        {
            return _externalDataCache.FCSContracts
                .SelectMany(c => c.FcsContractAllocations)
                .Where(ca => ca.ContractAllocationNumber == conRefNumber)
                .Any();
        }

        public bool FundingRelationshipFCTExists(IEnumerable<string> fundingStreamPeriodCodes)
        {
            return _externalDataCache.FCSContracts
               .SelectMany(c => c.FcsContractAllocations)
               .Where(ca => fundingStreamPeriodCodes.Contains(ca.FundingStreamPeriodCode))
               .Any();
        }
    }
}
