using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class ExternalDataCachePopulationService : IExternalDataCachePopulationService
    {
        private readonly IExternalDataCache _externalDataCache;
        private readonly ILARSStandardDataRetrievalService _larsStandardDataRetrievalService;
        private readonly ILARSStandardValidityDataRetrievalService _larsStandardValidityDataRetrievalService;
        private readonly ILARSLearningDeliveryDataRetrievalService _larsLearningDeliveryDataRetrievalService;
        private readonly ILARSFrameworkDataRetrievalService _larsFrameworkDataRetrievalService;
        private readonly IULNDataRetrievalService _ulnDataRetrievalService;
        private readonly IPostcodesDataRetrievalService _postcodesDataRetrievalService;
        private readonly IOrganisationsDataRetrievalService _organisationsDataRetrievalService;
        private readonly IEPAOrganisationsDataRetrievalService _epaOrganisationsDataRetrievalService;
        private readonly ICampusIdentifierDataRetrievalService _campusIdentifierDataRetrievalService;
        private readonly IFCSDataRetrievalService _fcsDataRetrievalService;
        private readonly IEmployersDataRetrievalService _employersDataRetrievalService;

        public ExternalDataCachePopulationService(
            IExternalDataCache externalDataCache,
            ILARSStandardDataRetrievalService larsStandardDataRetrievalService,
            ILARSStandardValidityDataRetrievalService larsStandardValidityDataRetrievalService,
            ILARSLearningDeliveryDataRetrievalService larsLearningDeliveryDataRetrievalService,
            ILARSFrameworkDataRetrievalService larsFrameworkDataRetrievalService,
            IULNDataRetrievalService ulnDataRetrievalService,
            IPostcodesDataRetrievalService postcodesDataRetrievalService,
            IOrganisationsDataRetrievalService organisationsDataRetrievalService,
            IEPAOrganisationsDataRetrievalService epaOrganisationsDataRetrievalService,
            ICampusIdentifierDataRetrievalService campusIdentifierDataRetrievalService,
            IFCSDataRetrievalService fcsDataRetrievalService,
            IEmployersDataRetrievalService employersDataRetrievalService)
        {
            _externalDataCache = externalDataCache;
            _larsStandardDataRetrievalService = larsStandardDataRetrievalService;
            _larsStandardValidityDataRetrievalService = larsStandardValidityDataRetrievalService;
            _larsLearningDeliveryDataRetrievalService = larsLearningDeliveryDataRetrievalService;
            _larsFrameworkDataRetrievalService = larsFrameworkDataRetrievalService;
            _ulnDataRetrievalService = ulnDataRetrievalService;
            _postcodesDataRetrievalService = postcodesDataRetrievalService;
            _organisationsDataRetrievalService = organisationsDataRetrievalService;
            _epaOrganisationsDataRetrievalService = epaOrganisationsDataRetrievalService;
            _campusIdentifierDataRetrievalService = campusIdentifierDataRetrievalService;
            _fcsDataRetrievalService = fcsDataRetrievalService;
            _employersDataRetrievalService = employersDataRetrievalService;
        }

        public async Task PopulateAsync(CancellationToken cancellationToken)
        {
            var externalDataCache = (ExternalDataCache)_externalDataCache;

            externalDataCache.Standards = await _larsStandardDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.StandardValidities = await _larsStandardValidityDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.LearningDeliveries = await _larsLearningDeliveryDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.Frameworks = await _larsFrameworkDataRetrievalService.RetrieveAsync(cancellationToken);

            externalDataCache.ULNs = new HashSet<long>(await _ulnDataRetrievalService.RetrieveAsync(cancellationToken));

            externalDataCache.Postcodes = (await _postcodesDataRetrievalService.RetrieveAsync(cancellationToken)).ToCaseInsensitiveHashSet();
            externalDataCache.ONSPostcodes = await _postcodesDataRetrievalService.RetrieveONSPostcodesAsync(cancellationToken);

            externalDataCache.Organisations = await _organisationsDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.CampusIdentifiers = await _campusIdentifierDataRetrievalService.RetrieveAsync(cancellationToken);

            externalDataCache.EPAOrganisations = await _epaOrganisationsDataRetrievalService.RetrieveAsync(cancellationToken);

            externalDataCache.FCSContractAllocations = await _fcsDataRetrievalService.RetrieveAsync(cancellationToken);

            externalDataCache.ERNs = new HashSet<int>(await _employersDataRetrievalService.RetrieveAsync(cancellationToken));
        }
    }
}
