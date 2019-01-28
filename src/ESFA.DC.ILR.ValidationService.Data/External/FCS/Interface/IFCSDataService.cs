using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the FCS data service definition
    /// </summary>
    public interface IFCSDataService
    {
        /// <summary>
        /// Gets the allocation for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// a contract allocation (if found)
        /// </returns>
        IFcsContractAllocation GetContractAllocationFor(string thisContractReference);

        /// <summary>
        /// Gets the eligibility rule for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">The this contract reference.</param>
        /// <returns>
        /// an eligibility rule (if found)
        /// </returns>
        IEsfEligibilityRule GetEligibilityRuleFor(string thisContractReference);

        /// <summary>
        /// Gets the eligibility rule employment statuses for.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// the eligibility rule employment status (if found)
        /// </returns>
        IEnumerable<IEsfEligibilityRuleEmploymentStatus> GetEligibilityRuleEmploymentStatusesFor(string contractReference);

        /// <summary>
        /// Gets the eligibility rule local authorities for.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// an eligibility rule local authority (if found)
        /// </returns>
        IEnumerable<IEsfEligibilityRuleLocalAuthority> GetEligibilityRuleLocalAuthoritiesFor(string contractReference);

        /// <summary>
        /// Gets the eligibility rule enterprise partnerships for.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// an eligibility rule enterprise partnership (if found)
        /// </returns>
        IEnumerable<IEsfEligibilityRuleLocalEnterprisePartnership> GetEligibilityRuleEnterprisePartnershipsFor(string contractReference);

        /// <summary>
        /// Gets the eligibility sector subject area levels for.
        /// </summary>
        /// <param name="conRefNumber">The contract reference number.</param>
        /// <returns>
        /// esf eligibility rule sector subject area levels
        /// </returns>
        IEnumerable<IEsfEligibilityRuleSectorSubjectAreaLevel> GetSectorSubjectAreaLevelsForContract(string conRefNumber);

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

        bool IsSectorSubjectAreaCodeExistsForContract(string conRefNumber);

        bool IsSectorSubjectAreaCodeNullForContract(string conRefNumber);

        bool IsSubjectAreaAndMinMaxLevelsExistsForContract(string conRefNumber);

        bool IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(int notionalNVQLevel2, string conRefNumber);

        bool IsSectorSubjectAreaTiersMatchingSubjectAreaCode(string conRefNumber, decimal? sectorSubjectAreaTier1, decimal? sectorSubjectAreaTier2);

        string GetMinPriorAttainment(string conRefNumber);

        string GetMaxPriorAttainment(string conRefNumber);
    }
}
