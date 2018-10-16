using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    public class FcsContractAllocation
    {
        public string ContractAllocationNumber { get; set; }

        public string FundingStreamCode { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public string Period { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
