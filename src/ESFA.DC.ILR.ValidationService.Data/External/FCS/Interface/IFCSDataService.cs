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
        /// Gets the eligibility rule employment status for.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// the eligibility rule employment status (if found)
        /// </returns>
        IEsfEligibilityRuleEmploymentStatus GetEligibilityRuleEmploymentStatusFor(string contractReference);

        /// <summary>
        /// Gets the eligibility rule local authority for.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// an eligibility rule local authority (if found)
        /// </returns>
        IEsfEligibilityRuleLocalAuthority GetEligibilityRuleLocalAuthorityFor(string contractReference);

        /// <summary>
        /// Gets the eligibility rule enterprise partnership for.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// an eligibility rule enterprise partnership (if found)
        /// </returns>
        IEsfEligibilityRuleLocalEnterprisePartnership GetEligibilityRuleEnterprisePartnershipFor(string contractReference);

        /// <summary>
        /// Gets the eligibility sector subject area levels for.
        /// </summary>
        /// <param name="conRefNumber">The contract reference number.</param>
        /// <returns>
        /// esf eligibility rule sector subject area levels
        /// </returns>
        IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> GetSectorSubjectAreaLevelsForContract(string conRefNumber);

        bool IsSectorSubjectAreaCodeExistsForContract(string conRefNumber);

        bool IsSectorSubjectAreaCodeNullForContract(string conRefNumber);

        bool IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(int notionalNVQLevel2, string conRefNumber);
    }
}
