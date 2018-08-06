using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    public interface IFCSDataService
    {
        bool ConRefNumberExists(string conRefNumber);

        bool FundingRelationshipFCTExists(IEnumerable<string> fundingStreamPeriodCodes);
    }
}
