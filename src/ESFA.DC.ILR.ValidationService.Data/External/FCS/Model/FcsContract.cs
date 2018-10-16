using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    public class FcsContract
    {
        public string ContractNumber { get; set; }

        public string OragnisationIdentifier { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IEnumerable<FcsContractAllocation> FcsContractAllocations { get; set; }
    }
}
