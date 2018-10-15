using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ReferenceData.FCS.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class FCSDataRetrievalService : AbstractDataRetrievalService, IFCSDataRetrievalService
    {
        private readonly IFcsContext _fcs;

        public FCSDataRetrievalService()
            : base(null)
        {
        }

        public FCSDataRetrievalService(IFcsContext fcs, ICache<IMessage> messageCache)
            : base(messageCache)
        {
            _fcs = fcs;
        }

        public async Task<IReadOnlyCollection<FcsContract>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var ukprn = UKPRNFromMessage(_messageCache.Item);

            return await _fcs.Contractors
                .Where(c => c.Ukprn == ukprn)
                .SelectMany(c => c.Contracts
                .Select(con => new FcsContract
                {
                    ContractNumber = con.ContractNumber,
                    OragnisationIdentifier = c.OrganisationIdentifier,
                    StartDate = con.StartDate,
                    EndDate = con.EndDate,
                    FcsContractAllocations = con.ContractAllocations
                    .Select(ca => new FcsContractAllocation
                    {
                        ContractAllocationNumber = ca.ContractAllocationNumber,
                        FundingStreamCode = ca.FundingStreamCode,
                        FundingStreamPeriodCode = ca.FundingStreamPeriodCode,
                        Period = ca.Period,
                        StartDate = ca.StartDate,
                        EndDate = ca.EndDate
                    }).ToList()
                })).ToListAsync(cancellationToken);
        }

        public int UKPRNFromMessage(IMessage message)
        {
            return message.LearningProviderEntity.UKPRN;
        }
    }
}
