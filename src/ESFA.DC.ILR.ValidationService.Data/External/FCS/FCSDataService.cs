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
        /// The contract allocations
        /// </summary>
        private readonly IReadOnlyDictionary<string, IFcsContractAllocation> _contractAllocations;

        /// <summary>
        /// Initializes a new instance of the <see cref="FCSDataService"/> class.
        /// </summary>
        /// <param name="externalDataCache">The external data cache.</param>
        public FCSDataService(IExternalDataCache externalDataCache)
        {
            _contractAllocations = externalDataCache.FCSContractAllocations.ToCaseInsensitiveDictionary();
        }

        /// <summary>
        /// Gets the contract allocation for.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>a contract allocation (if found)</returns>
        public IFcsContractAllocation GetContractAllocationFor(string contractReference)
        {
            if (contractReference == null)
            {
                return null;
            }

            _contractAllocations.TryGetValue(contractReference, out IFcsContractAllocation fcsContractAllocation);

            return fcsContractAllocation;
        }

        /// <summary>
        /// Gets the eligibility rule for.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>an eligibility rule (if found)</returns>
        public IEsfEligibilityRule GetEligibilityRuleFor(string contractReference)
        {
            var contractAllocation = GetContractAllocationFor(contractReference);

            return contractAllocation?.EsfEligibilityRule;
        }

        /// <summary>
        /// Gets the eligibility rule employment status.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// the eligibility rule employment status (if found)
        /// </returns>
        public IEnumerable<IEsfEligibilityRuleEmploymentStatus> GetEligibilityRuleEmploymentStatusesFor(string contractReference)
        {
            var eligibility = GetEligibilityRuleFor(contractReference);

            return eligibility?.EmploymentStatuses
                ?? Collection.EmptyAndReadOnly<IEsfEligibilityRuleEmploymentStatus>();
        }

        /// <summary>
        /// Gets the eligibility rule local authority.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// an eligibility rule local authority (if found)
        /// </returns>
        public IEnumerable<IEsfEligibilityRuleLocalAuthority> GetEligibilityRuleLocalAuthoritiesFor(string contractReference)
        {
            var eligibility = GetEligibilityRuleFor(contractReference);

            return eligibility?.LocalAuthorities
                ?? Collection.EmptyAndReadOnly<IEsfEligibilityRuleLocalAuthority>();
        }

        /// <summary>
        /// Gets the eligibility rule enterprise partnership.
        /// </summary>
        /// <param name="contractReference">The contract reference.</param>
        /// <returns>
        /// an eligibility rule enterprise partnership (if found)
        /// </returns>
        public IEnumerable<IEsfEligibilityRuleLocalEnterprisePartnership> GetEligibilityRuleEnterprisePartnershipsFor(string contractReference)
        {
            var eligibility = GetEligibilityRuleFor(contractReference);

            return eligibility?.LocalEnterprisePartnerships
                ?? Collection.EmptyAndReadOnly<IEsfEligibilityRuleLocalEnterprisePartnership>();
        }

        public IEnumerable<IEsfEligibilityRuleSectorSubjectAreaLevel> GetSectorSubjectAreaLevelsFor(string contractReference)
        {
            var eligibility = GetEligibilityRuleFor(contractReference);

            return eligibility?.SectorSubjectAreaLevels
                ?? Collection.EmptyAndReadOnly<IEsfEligibilityRuleSectorSubjectAreaLevel>();
        }

        /// <summary>
        /// Contract reference number exists.
        /// </summary>
        /// <param name="conRefNumber">The con reference number.</param>
        /// <returns>true if it does</returns>
        public bool ConRefNumberExists(string conRefNumber)
        {
            return !string.IsNullOrEmpty(conRefNumber) && _contractAllocations.ContainsKey(conRefNumber);
        }

        /// <summary>
        /// Fundings the relationship FCT exists.
        /// </summary>
        /// <param name="fundingStreamPeriodCodes">The funding stream period codes.</param>
        /// <returns>true if it does</returns>
        public bool FundingRelationshipFCTExists(IEnumerable<string> fundingStreamPeriodCodes)
        {
            var fsCodes = fundingStreamPeriodCodes.AsSafeReadOnlyList().ToCaseInsensitiveHashSet();

            return _contractAllocations.Values.Any(ca => fsCodes.Contains(ca.FundingStreamPeriodCode));
        }

        // TODO: should be in the rule Learn Aim Ref 71
        public bool IsSectorSubjectAreaCodeExistsForContract(string conRefNumber)
        {
            return GetSectorSubjectAreaLevelsFor(conRefNumber)?
                .Any(
                s => s.SectorSubjectAreaCode.HasValue
                && (string.IsNullOrEmpty(s.MinLevelCode)
                && string.IsNullOrEmpty(s.MaxLevelCode))) ?? false;
        }

        // TODO: should be in the rule Learn Aim Ref 72
        public bool IsSectorSubjectAreaCodeNullForContract(string conRefNumber)
        {
            return GetSectorSubjectAreaLevelsFor(conRefNumber)?
                .Any(
                s => s.SectorSubjectAreaCode == null
                && (!string.IsNullOrEmpty(s.MinLevelCode)
                    || !string.IsNullOrEmpty(s.MaxLevelCode))) ?? false;
        }

        // TODO: should be in the rule Learn Aim Ref 72
        public bool IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(int notionalNVQLevel2, string conRefNumber)
        {
            return GetSectorSubjectAreaLevelsFor(conRefNumber)?
                .Any(s => (!string.IsNullOrEmpty(s.MinLevelCode)
                    && notionalNVQLevel2 < Convert.ToInt32(s.MinLevelCode))
                    || (!string.IsNullOrEmpty(s.MaxLevelCode)
                    && notionalNVQLevel2 > Convert.ToInt32(s.MaxLevelCode))) ?? false;
        }
    }
}
