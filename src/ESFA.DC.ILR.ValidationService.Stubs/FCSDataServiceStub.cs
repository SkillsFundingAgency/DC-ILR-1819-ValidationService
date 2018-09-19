using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class FCSDataServiceStub : IFCSDataService
    {
        public bool ConRefNumberExists(string conRefNumber)
        {
            return true;
        }

        public bool FundingRelationshipFCTExists(IEnumerable<string> fundingStreamPeriodCodes)
        {
            // TODO: check condition ContractAllocation.FundingStreamPeriodCode = AEBC1819 (will be passed in parameter fundingStreamPeriodCodes)
            return true;
        }
    }
}
