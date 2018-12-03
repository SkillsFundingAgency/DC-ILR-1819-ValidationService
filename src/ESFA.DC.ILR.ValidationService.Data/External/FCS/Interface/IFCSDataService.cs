using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the FCS data service definition
    /// </summary>
    public interface IFCSDataService
    {
        bool ConRefNumberExists(string conRefNumber);

        bool FundingRelationshipFCTExists(IEnumerable<string> fundingStreamPeriodCodes);

        /// <summary>
        /// Gets the eligibility rule employment status.
        /// 2018-11-08 CME: this routine may require refinement as i'm not convinced i'm filtering with all of the correct criteria
        /// </summary>
        /// <param name="forContractReference">For contract reference.</param>
        /// <returns>the eligibility rule employment status (should there be one)</returns>
        IEsfEligibilityRuleEmploymentStatus GetEligibilityRuleEmploymentStatus(string forContractReference);

        IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> GetSectorSubjectAreaLevelsForContract(string conRefNumber);

        bool IsSectorSubjectAreaCodeExistsForContract(string conRefNumber);
    }
}
