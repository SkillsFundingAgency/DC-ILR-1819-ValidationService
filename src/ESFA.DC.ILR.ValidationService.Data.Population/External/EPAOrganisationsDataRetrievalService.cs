using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ReferenceData.EPA.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class EPAOrganisationsDataRetrievalService : AbstractEpaOrganisationsDataRetrievalService, IEPAOrganisationsDataRetrievalService
    {
        private readonly IEpaContext _epaContext;

        public EPAOrganisationsDataRetrievalService()
            : base(null)
        {
        }

        public EPAOrganisationsDataRetrievalService(IEpaContext epaContext, ICache<IMessage> messageCache)
            : base(messageCache)
        {
            _epaContext = epaContext;
        }

        public async Task<IReadOnlyDictionary<string, List<EPAOrganisations>>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var epaOrgIds = UniqueEpaOrgIdsFromMessage(_messageCache.Item).ToCaseInsensitiveHashSet();

            return await _epaContext?
                        .Periods?.Where(o => epaOrgIds.Contains(o.OrganisationId))
                        .GroupBy(o => o.OrganisationId)
                        .ToCaseInsensitiveAsyncDictionary(
                           k => k.Key,
                           v => v.Select(epa => new EPAOrganisations
                           {
                               ID = epa.OrganisationId,
                               Standard = epa.StandardCode,
                               EffectiveFrom = epa.EffectiveFrom,
                               EffectiveTo = epa.EffectiveTo
                           }).ToList(), cancellationToken);
        }
    }
}
