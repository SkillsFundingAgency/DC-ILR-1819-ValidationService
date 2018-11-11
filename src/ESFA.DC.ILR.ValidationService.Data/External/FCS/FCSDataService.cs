using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS
{
    /// <summary>
    /// the FCS data service implementation
    /// </summary>
    /// <seealso cref="IFCSDataService" />
    public class FCSDataService :
        IFCSDataService
    {
        /// <summary>
        /// The employment statuses
        /// </summary>
        private readonly IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus> _employmentStatuses;

        /// <summary>
        /// The contract allocations
        /// </summary>
        private readonly IReadOnlyCollection<IFcsContractAllocation> _contractAllocations;

        public FCSDataService(IExternalDataCache externalDataCache)
        {
            _employmentStatuses = externalDataCache.ESFEligibilityRuleEmploymentStatuses;
            _contractAllocations = externalDataCache.FCSContractAllocations;
        }

        /// <summary>
        /// Contract reference number exists.
        /// </summary>
        /// <param name="conRefNumber">The con reference number.</param>
        /// <returns>true if it does</returns>
        public bool ConRefNumberExists(string conRefNumber)
        {
            return _contractAllocations
                .Where(ca => ca.ContractAllocationNumber == conRefNumber)
                .Any();
        }

        /// <summary>
        /// Fundings the relationship FCT exists.
        /// </summary>
        /// <param name="fundingStreamPeriodCodes">The funding stream period codes.</param>
        /// <returns>true if it does</returns>
        public bool FundingRelationshipFCTExists(IEnumerable<string> fundingStreamPeriodCodes)
        {
            var fsCodes = fundingStreamPeriodCodes.AsSafeReadOnlyList();

            return _contractAllocations
               .Where(ca => fsCodes.Contains(ca.FundingStreamPeriodCode))
               .Any();
        }

        /// <summary>
        /// Gets the eligibility rule employment status.
        /// 2018-11-08 CME: this routine may require refinement as i'm not convinced i'm filtering with all of the correct criteria
        /// </summary>
        /// <param name="forContractReference">For contract reference.</param>
        /// <returns>the eligibility rule employment status (should there be one)</returns>
        public IEsfEligibilityRuleEmploymentStatus GetEligibilityRuleEmploymentStatus(string forContractReference)
        {
            var allocation = _contractAllocations.FirstOrDefault(x => x.ContractAllocationNumber == forContractReference);
            return _employmentStatuses.FirstOrDefault(x => x.TenderSpecReference.ComparesWith(allocation.TenderSpecReference));
        }
    }
}
