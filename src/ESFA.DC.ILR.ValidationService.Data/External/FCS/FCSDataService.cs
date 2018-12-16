using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
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
        /// The local authorities
        /// </summary>
        private readonly IReadOnlyCollection<IEsfEligibilityRuleLocalAuthority> _localAuthorities;

        /// <summary>
        /// The enterprise partnerships
        /// </summary>
        private readonly IReadOnlyCollection<IEsfEligibilityRuleLocalEnterprisePartnership> _enterprisePartnerships;

        /// <summary>
        /// The contract allocations
        /// </summary>
        private readonly IReadOnlyCollection<IFcsContractAllocation> _contractAllocations;

        /// <summary>
        /// The Sector Subject Area Levels
        /// </summary>
        private readonly IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> _sectorSubjectAreaLevels;

        /// <summary>
        /// Initializes a new instance of the <see cref="FCSDataService"/> class.
        /// </summary>
        /// <param name="externalDataCache">The external data cache.</param>
        public FCSDataService(IExternalDataCache externalDataCache)
        {
            _employmentStatuses = externalDataCache.ESFEligibilityRuleEmploymentStatuses.AsSafeReadOnlyList();
            _localAuthorities = externalDataCache.ESFEligibilityRuleLocalAuthorities.AsSafeReadOnlyList();
            _enterprisePartnerships = externalDataCache.ESFEligibilityRuleEnterprisePartnerships.AsSafeReadOnlyList();
            _contractAllocations = externalDataCache.FCSContractAllocations.AsSafeReadOnlyList();
            _sectorSubjectAreaLevels = externalDataCache.EsfEligibilityRuleSectorSubjectAreaLevels;
        }

        /// <summary>
        /// Contract reference number exists.
        /// </summary>
        /// <param name="conRefNumber">The con reference number.</param>
        /// <returns>true if it does</returns>
        public bool ConRefNumberExists(string conRefNumber)
        {
            return _contractAllocations
                .Where(ca => ca.ContractAllocationNumber.CaseInsensitiveEquals(conRefNumber))
                .Any();
        }

        /// <summary>
        /// Fundings the relationship FCT exists.
        /// </summary>
        /// <param name="fundingStreamPeriodCodes">The funding stream period codes.</param>
        /// <returns>true if it does</returns>
        public bool FundingRelationshipFCTExists(IEnumerable<string> fundingStreamPeriodCodes)
        {
            var fsCodes = fundingStreamPeriodCodes.AsSafeReadOnlyList().ToCaseInsensitiveHashSet();

            return _contractAllocations
               .Where(ca => fsCodes.Contains(ca.FundingStreamPeriodCode))
               .Any();
        }

        /// <summary>
        /// Gets the eligibility rule employment status.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// the eligibility rule employment status (if found)
        /// </returns>
        public IEsfEligibilityRuleEmploymentStatus GetEligibilityRuleEmploymentStatusFor(string contractReference)
        {
            return GetEligibilityRuleItemFor(contractReference, _employmentStatuses);
        }

        /// <summary>
        /// Gets the eligibility rule local authority.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// an eligibility rule local authority (if found)
        /// </returns>
        public IEsfEligibilityRuleLocalAuthority GetEligibilityRuleLocalAuthorityFor(string contractReference)
        {
            return GetEligibilityRuleItemFor(contractReference, _localAuthorities);
        }

        /// <summary>
        /// Gets the eligibility rule enterprise partnership.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// an eligibility rule enterprise partnership (if found)
        /// </returns>
        public IEsfEligibilityRuleLocalEnterprisePartnership GetEligibilityRuleEnterprisePartnershipFor(string contractReference)
        {
            return GetEligibilityRuleItemFor(contractReference, _enterprisePartnerships);
        }

        /// <summary>
        /// Gets the contract allocation for.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>a contract allocation (if found)</returns>
        public IFcsContractAllocation GetContractAllocationFor(string contractReference) =>
            _contractAllocations.FirstOrDefault(x => x.ContractAllocationNumber.ComparesWith(contractReference));

        /// <summary>
        /// That matches reference to allocation
        /// 2018-12-05 CME: this routine may require refinement as i'm not convinced i'm filtering with all of the correct criteria
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="allocation">The allocation.</param>
        /// <returns>
        ///   <c>true</c> if [that matches] [the specified reference]; otherwise, <c>false</c>.
        /// </returns>
        public bool ThatMatches(IEsfEligibilityRuleReferences reference, IFcsContractAllocation allocation) =>
            reference.TenderSpecReference.ComparesWith(allocation.TenderSpecReference);

        /// <summary>
        /// Gets the eligibility rule item for.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="contractReference">The contract reference.</param>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>
        /// an eligibility rule item of <typeparamref name="TResult" /> (if found)
        /// </returns>
        public TResult GetEligibilityRuleItemFor<TResult>(string contractReference, IReadOnlyCollection<TResult> usingSources)
            where TResult : class, IEsfEligibilityRuleReferences
        {
            var thisAllocation = GetContractAllocationFor(contractReference);
            return It.Has(thisAllocation)
                ? usingSources.FirstOrDefault(x => ThatMatches(x, thisAllocation))
                : null;
        }

        public IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> GetSectorSubjectAreaLevelsForContract(string conRefNumber)
        {
            return _sectorSubjectAreaLevels?
                .Join(
                    _contractAllocations?.Where(ca => ca.ContractAllocationNumber.CaseInsensitiveEquals(conRefNumber)).ToList(),
                    ers => new { ers.TenderSpecReference, ers.LotReference },
                    ca => new { ca.TenderSpecReference, ca.LotReference },
                    (ers, ca) => ers).ToList();
        }

        public bool IsSectorSubjectAreaCodeExistsForContract(string conRefNumber)
        {
            return GetSectorSubjectAreaLevelsForContract(conRefNumber)?
                .Any(
                s => s.SectorSubjectAreaCode.HasValue
                && (string.IsNullOrEmpty(s.MinLevelCode)
                && string.IsNullOrEmpty(s.MaxLevelCode))) ?? false;
        }

        public bool IsSectorSubjectAreaCodeNullForContract(string conRefNumber)
        {
            return GetSectorSubjectAreaLevelsForContract(conRefNumber)?
                .Any(
                s => s.SectorSubjectAreaCode == null
                && (!string.IsNullOrEmpty(s.MinLevelCode)
                    || !string.IsNullOrEmpty(s.MaxLevelCode))) ?? false;
        }

        public bool IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(int notionalNVQLevel2, string conRefNumber)
        {
            return GetSectorSubjectAreaLevelsForContract(conRefNumber)?
                .Any(s => (!string.IsNullOrEmpty(s.MinLevelCode)
                    && notionalNVQLevel2 < Convert.ToInt32(s.MinLevelCode))
                    || (!string.IsNullOrEmpty(s.MaxLevelCode)
                    && notionalNVQLevel2 > Convert.ToInt32(s.MaxLevelCode))) ?? false;
        }
    }
}
