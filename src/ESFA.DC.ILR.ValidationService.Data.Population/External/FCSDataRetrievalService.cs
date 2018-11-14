using ESFA.DC.ILR.Model.Interface;
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
        public async Task<IReadOnlyCollection<FcsContract>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var ukprn = UKPRNFromMessage(_messageCache.Item);

            return await _fcs.Contractors
                .Where(c => c.Ukprn == ukprn)
                .SelectMany(c => c.Contracts)
                .Select(con => new FcsContract
                {
                    ID = con.Id,
                    ContractorID = con.ContractorId,
                    ContractNumber = con.ContractNumber,
                    MajorVersion = con.ContractVersionNumber, // MinorVersion = con.ContractSubVersionNumber,
                    StartDate = con.StartDate,
                    EndDate = con.EndDate,
                })
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves the contract allocations asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// a task running the collection builder
        /// </returns>
        public async Task<IReadOnlyCollection<IFcsContractAllocation>> RetrieveContractAllocationsAsync(CancellationToken cancellationToken)
        {
            var ukprn = UKPRNFromMessage(_messageCache.Item);

            return await _fcs.ContractAllocations
                .Where(ca => ca.DeliveryUKPRN == ukprn)
                .Select(ca => new FcsContractAllocation
                {
                    ContractAllocationNumber = ca.ContractAllocationNumber,
                    FundingStreamCode = ca.FundingStreamCode,
                    FundingStreamPeriodCode = ca.FundingStreamPeriodCode,
                    Period = ca.Period,
                    StartDate = ca.StartDate,
                    EndDate = ca.EndDate,
                    ID = ca.Id,
                    ContractID = ca.ContractId,
                    DeliveryOrganisation = ca.DeliveryOrganisation,
                    DeliveryUKPRN = ca.DeliveryUKPRN,
                    LearningRatePremiumFactor = ca.LearningRatePremiumFactor,
                    LotReference = ca.LotReference,
                    PeriodTypeCode = ca.PeriodTypeCode,
                    StopNewStartsFromDate = ca.StopNewStartsFromDate,
                    TenderSpecReference = ca.TenderSpecReference,
                    TerminationDate = ca.TerminationDate,
                    UoPCode = ca.UoPCode
                })
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves the eligibility rule employment statuses asynchronous.
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
                    EligibilityRuleID = 0, // not yet offered by the class EsfEligibilityRule = er.EsfEligibilityRule,
                    LotReference = er.LotReference,
                    TenderSpecReference = er.TenderSpecReference
                })
                .ToListAsync(cancellationToken);
        }

        public int UKPRNFromMessage(IMessage message) =>
            message.LearningProviderEntity.UKPRN;
    }
}
