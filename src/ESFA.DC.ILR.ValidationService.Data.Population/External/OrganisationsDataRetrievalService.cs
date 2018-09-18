using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.Organisatons.Model.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class OrganisationsDataRetrievalService : AbstractOrganisationsDataRetrievalService, IOrganisationsDataRetrievalService
    {
        private readonly IOrganisations _organisations;

        public OrganisationsDataRetrievalService()
              : base(null)
        {
        }

        public OrganisationsDataRetrievalService(IOrganisations organisations, ICache<IMessage> messageCache)
              : base(messageCache)
        {
            _organisations = organisations;
        }

        public async Task<IReadOnlyDictionary<long, Organisation>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var ukprns = UniqueUKPRNsFromMessage(_messageCache.Item).ToList();

            return await _organisations
                .MasterOrganisations
                .Where(o => ukprns.Contains(o.UKPRN))
                .ToDictionaryAsync(
                    o => o.UKPRN,
                    o => new Organisation
                    {
                        UKPRN = o.Org_Details?.UKPRN,
                        LegalOrgType = o.Org_Details?.LegalOrgType,
                        PartnerUKPRN = o.Org_PartnerUKPRN.Any(op => op.UKPRN == o.UKPRN)
                    }, cancellationToken);
        }
    }
}
