using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class OrganisationsDataRetrievalService : AbstractOrganisationsDataRetrievalService, IOrganisationsDataRetrievalService
    {
        private readonly IOrganisationsContext _organisations;

        public OrganisationsDataRetrievalService()
              : base(null)
        {
        }

        public OrganisationsDataRetrievalService(IOrganisationsContext organisations, ICache<IMessage> messageCache)
              : base(messageCache)
        {
            _organisations = organisations;
        }

        public async Task<IReadOnlyDictionary<long, Organisation>> RetrieveAsync(CancellationToken cancellationToken)
        {
            return await Task.Run(
                () =>
                {
                    var ukprns = UniqueUKPRNsFromMessage(_messageCache.Item).ToList();

                    return _organisations
                        .MasterOrganisations
                        .Where(o => ukprns.Contains(o.Ukprn))
                        .ToDictionary(
                            o => o.Ukprn,
                            o => new Organisation
                            {
                                UKPRN = o.OrgDetail?.Ukprn,
                                LegalOrgType = o.OrgDetail?.LegalOrgType,
                                PartnerUKPRN = o.OrgPartnerUkprns.Any(op => op.Ukprn == o.Ukprn)
                            });
                }, cancellationToken);
        }
    }
}
