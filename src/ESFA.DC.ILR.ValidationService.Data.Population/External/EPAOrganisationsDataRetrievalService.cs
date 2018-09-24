using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    /// <summary>
    /// the EPA organisations data retribval service
    /// </summary>
    /// <seealso cref="AbstractDataRetrievalService" />
    /// <seealso cref="IEPAOrganisationsDataRetrievalService" />
    public class EPAOrganisationsDataRetrievalService :
        AbstractDataRetrievalService,
        IEPAOrganisationsDataRetrievalService
    {
        // TODO: remove comment once code integrated
        // private readonly IEPAOrganisations _organisations;
        public EPAOrganisationsDataRetrievalService(ICache<IMessage> messageCache)
              : base(messageCache)
        {
        }

        /*
        // TODO: candidate constructor
        public EPAOrganisationsDataRetrievalService(IEPAOrganisations organisations, ICache<IMessage> messageCache)
            : base(messageCache)
        {
            _organisations = organisations;
        }
         */

        public async Task<IReadOnlyCollection<IEPAOrganisation>> RetrieveAsync(CancellationToken cancellationToken)
        {
            return await Task.Run(() => Collection.EmptyAndReadOnly<IEPAOrganisation>(), cancellationToken);
        }
    }
}
