    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
    using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
    using ESFA.DC.ILR.ValidationService.Data.Interface;
    using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
    using ESFA.DC.ReferenceData.Organisations.Model.Interface;
    using Microsoft.EntityFrameworkCore;

    namespace ESFA.DC.ILR.ValidationService.Data.Population.External
    {
    public class CampusIdentiferDataRetrievalService : AbstractOrganisationsDataRetrievalService, ICampusIdentifierDataRetrievalService
    {
        private readonly IOrganisationsContext _organisations;

        public CampusIdentiferDataRetrievalService()
              : base(null)
        {
        }

        public CampusIdentiferDataRetrievalService(IOrganisationsContext organisations, ICache<IMessage> messageCache)
              : base(messageCache)
        {
            _organisations = organisations;
        }

        public async Task<IReadOnlyCollection<ICampusIdentifier>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var ukprns = UniqueUKPRNsFromMessage(_messageCache.Item).ToList();

            return await _organisations
                .CampusIdentifiers
                .Where(o => ukprns.Contains(o.MasterUkprn))
                .Select(ci =>
                    new CampusIdentifier()
                    {
                        MasterUKPRN = ci.MasterUkprn,
                        CampusIdentifer = ci.CampusIdentifier1,
                    }).ToListAsync(cancellationToken);
        }
    }
}
