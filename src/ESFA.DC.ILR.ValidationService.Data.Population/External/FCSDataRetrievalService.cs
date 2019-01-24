using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    /// <summary>
    /// the FCS data service implementation
    /// </summary>
    /// <seealso cref="AbstractDataRetrievalService" />
    /// <seealso cref="IFCSDataRetrievalService" />
    public class FCSDataRetrievalService :
        AbstractDataRetrievalService,
        IFCSDataRetrievalService
    {
        /// <summary>
        /// The FCS (DB Context)
        /// </summary>
        private readonly IFcsContext _fcs;

        /// <summary>
        /// Initializes a new instance of the <see cref="FCSDataRetrievalService"/> class.
        /// </summary>
        public FCSDataRetrievalService()
            : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FCSDataRetrievalService"/> class.
        /// </summary>
        /// <param name="fcs">The FCS.</param>
        /// <param name="messageCache">The message cache.</param>
        public FCSDataRetrievalService(IFcsContext fcs, ICache<IMessage> messageCache)
            : base(messageCache)
        {
            _fcs = fcs;
        }

        /// <summary>
        /// Retrieves the (FCS Contracts) asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// a task running the collection builder
        /// </returns>
        public async Task<IReadOnlyDictionary<string, IFcsContractAllocation>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var ukprn = UKPRNFromMessage(_messageCache.Item);

            var contractAllocations = await _fcs.ContractAllocations
                .Where(ca => ca.DeliveryUKPRN == ukprn)
                .Select(ca => new FcsContractAllocation
                {
                    ContractAllocationNumber = ca.ContractAllocationNumber,
                    FundingStreamPeriodCode = ca.FundingStreamPeriodCode,
                    LotReference = ca.LotReference,
                    TenderSpecReference = ca.TenderSpecReference,
                    StartDate = ca.StartDate
                })
                .ToListAsync(cancellationToken);

            // No Foreign key Relationship on Model for Eligibility Rules, do two queries and overlay in Memory, only around 300 Rules, not worth filter at this point.
            var eligibilityRules = await _fcs.EsfEligibilityRules
                .Select(r => new EsfEligibilityRule()
                    {
                        LotReference = r.LotReference,
                        TenderSpecReference = r.TenderSpecReference,
                        MinAge = r.MinAge,
                        MaxAge = r.MaxAge,
                        Benefits = r.Benefits,
                        EmploymentStatuses = r.EsfEligibilityRuleEmploymentStatuses
                            .Select(s => new EsfEligibilityRuleEmploymentStatus()
                            {
                                Code = s.Code,
                                LotReference = s.LotReference,
                                TenderSpecReference = s.TenderSpecReference,
                            }).ToList(),
                        LocalAuthorities = r.EsfEligibilityRuleLocalAuthorities
                            .Select(a => new EsfEligibilityRuleLocalAuthority()
                            {
                                Code = a.Code,
                                LotReference = a.LotReference,
                                TenderSpecReference = a.TenderSpecReference,
                            }).ToList(),
                        LocalEnterprisePartnerships = r.EsfEligibilityRuleLocalEnterprisePartnerships
                            .Select(p => new EsfEligibilityRuleLocalEnterprisePartnership()
                            {
                                Code = p.Code,
                                LotReference = p.LotReference,
                                TenderSpecReference = p.TenderSpecReference
                            }).ToList(),
                        SectorSubjectAreaLevels = r.EsfEligibilityRuleSectorSubjectAreaLevel
                            .Select(l => new EsfEligibilityRuleSectorSubjectAreaLevel()
                            {
                                MaxLevelCode = l.MaxLevelCode,
                                MinLevelCode = l.MinLevelCode,
                                SectorSubjectAreaCode = l.SectorSubjectAreaCode,
                                LotReference = l.LotReference,
                                TenderSpecReference = l.TenderSpecReference
                            }).ToList(),
                        MinPriorAttainment = r.MinPriorAttainment,
                        MaxPriorAttainment = r.MaxPriorAttainment
                    }).ToListAsync(cancellationToken);

            foreach (var contractAllocation in contractAllocations)
            {
                contractAllocation.EsfEligibilityRule =
                    eligibilityRules
                        .SingleOrDefault(r =>
                            r.LotReference.CaseInsensitiveEquals(contractAllocation.LotReference)
                            && r.TenderSpecReference.CaseInsensitiveEquals(contractAllocation.TenderSpecReference));
            }

            return contractAllocations.ToCaseInsensitiveDictionary<IFcsContractAllocation, IFcsContractAllocation>(ca => ca.ContractAllocationNumber, ca => ca);
        }

        /// <summary>
        /// Retrieves the eligibility rule employment statuses asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// a task running the collection builder
        /// </returns>
        public async Task<IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus>> RetrieveEligibilityRuleEmploymentStatusesAsync(CancellationToken cancellationToken)
        {
            return await _fcs.EsfEligibilityRuleEmploymentStatuses
                .Select(er => new EsfEligibilityRuleEmploymentStatus
                {
                    Code = er.Code,
                    LotReference = er.LotReference,
                    TenderSpecReference = er.TenderSpecReference
                })
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves the eligibility rule local authorities asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// a task running the collection builder
        /// </returns>
        public async Task<IReadOnlyCollection<IEsfEligibilityRuleLocalAuthority>> RetrieveEligibilityRuleLocalAuthoritiesAsync(CancellationToken cancellationToken)
        {
            return await _fcs.EsEligibilityRulefLocalAuthorities
                .Select(er => new EsfEligibilityRuleLocalAuthority
                {
                    Code = er.Code,
                    LotReference = er.LotReference,
                    TenderSpecReference = er.TenderSpecReference
                })
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves the eligibility rule enterprise partnerships asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// a task running the collection builder
        /// </returns>
        public async Task<IReadOnlyCollection<IEsfEligibilityRuleLocalEnterprisePartnership>> RetrieveEligibilityRuleEnterprisePartnershipsAsync(CancellationToken cancellationToken)
        {
            return await _fcs.EsfEligibilityRuleLocalEnterprisePartnerships
                .Select(er => new EsfEligibilityRuleLocalEnterprisePartnership
                {
                    Code = er.Code,
                    LotReference = er.LotReference,
                    TenderSpecReference = er.TenderSpecReference
                })
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves the eligibility rule sector subject area level for message specific contracts asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// a task running the collection builder for eligibility rule sector subject area level
        /// </returns>
        public async Task<IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel>> RetrieveEligibilityRuleSectorSubjectAreaLevelAsync(CancellationToken cancellationToken)
        {
            if (this._messageCache?
                .Item?
                .LearningProviderEntity == null)
            {
                return null;
            }

            var ukprn = this.UKPRNFromMessage(this._messageCache.Item);
            var messageConRefNumbers = this.ConRefNumbersFromMessage(this._messageCache.Item);
            if (messageConRefNumbers == null || messageConRefNumbers.Count() == 0)
            {
                return null;
            }

            var contractAllocations = this._fcs.ContractAllocations?
               .Where(ca =>
                   ca.DeliveryUKPRN == ukprn
                   && messageConRefNumbers.Contains(ca.ContractAllocationNumber))
               .Select(ca => new { ca.TenderSpecReference, ca.LotReference }).Distinct();

            if (contractAllocations == null || contractAllocations.Count() == 0)
            {
                return null;
            }

            var esfEligibilityRuleSectorSubjectAreaLevel = await this._fcs.EsfEligibilityRuleSectorSubjectAreaLevel.ToListAsync(cancellationToken);

            return esfEligibilityRuleSectorSubjectAreaLevel?
                .Join(
                    contractAllocations,
                    ers => new { ers.TenderSpecReference, ers.LotReference },
                    ca => new { ca.TenderSpecReference, ca.LotReference },
                    (ers, ca) => new EsfEligibilityRuleSectorSubjectAreaLevel
                    {
                        TenderSpecReference = ers.TenderSpecReference,
                        LotReference = ers.LotReference,
                        SectorSubjectAreaCode = ers.SectorSubjectAreaCode,
                        MaxLevelCode = ers.MaxLevelCode,
                        MinLevelCode = ers.MinLevelCode
                    }).ToList();
        }

        public IEnumerable<string> ConRefNumbersFromMessage(IMessage message)
        {
            return message?
                .Learners?
                .Where(l => l.LearningDeliveries != null)
                .SelectMany(l => l.LearningDeliveries)
                .Where(ld => !string.IsNullOrEmpty(ld.ConRefNumber))
                .Select(ld => ld.ConRefNumber).Distinct().ToCaseInsensitiveHashSet();
        }

        public int UKPRNFromMessage(IMessage message) =>
            message.LearningProviderEntity.UKPRN;
    }
}
