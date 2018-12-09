using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the FCS data service definition
    /// </summary>
    public interface IFCSDataService
    {
        /// <summary>
        /// Contract reference number exists.
        /// </summary>
        /// <param name="conRefNumber">The con reference number.</param>
        /// <returns>true if it does</returns>
        bool ConRefNumberExists(string conRefNumber);

        /// <summary>
        /// Fundings the relationship FCT exists.
        /// </summary>
        /// <param name="fundingStreamPeriodCodes">The funding stream period codes.</param>
        /// <returns>true if it does</returns>
        bool FundingRelationshipFCTExists(IEnumerable<string> fundingStreamPeriodCodes);

        /// <summary>
        /// Gets the eligibility rule employment status.
        /// </summary>
        /// <param name="forContractReference">For contract reference.</param>
        /// <returns>
        /// the eligibility rule employment status (if found)
        /// </returns>
        IEsfEligibilityRuleEmploymentStatus GetEligibilityRuleEmploymentStatus(string forContractReference);

        /// <summary>
        /// Gets the eligibility rule local authority.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// an eligibility rule local authority (if found)
        /// </returns>
        IEsfEligibilityRuleLocalAuthority GetEligibilityRuleLocalAuthority(string contractReference);

        IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> GetSectorSubjectAreaLevelsForContract(string conRefNumber);

        bool IsSectorSubjectAreaCodeExistsForContract(string conRefNumber);
    }
}
