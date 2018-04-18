using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class PreValidationPopulationService : IPreValidationPopulationService
    {
        private readonly IMessageCachePopulationService _messageCachePopulationService;
        private readonly IFileDataCachePopulationService _fileDataCachePopulationService;
        private readonly IInternalDataCachePopulationService _internalDataCachePopulationService;
        private readonly IExternalDataCachePopulationService _externalDataCachePopulationService;

        public PreValidationPopulationService(
            IMessageCachePopulationService messageCachePopulationService,
            IFileDataCachePopulationService fileDataCachePopulationService,
            IInternalDataCachePopulationService internalDataCachePopulationService,
            IExternalDataCachePopulationService externalDataCachePopulationService)
        {
            _messageCachePopulationService = messageCachePopulationService;
            _fileDataCachePopulationService = fileDataCachePopulationService;
            _internalDataCachePopulationService = internalDataCachePopulationService;
            _externalDataCachePopulationService = externalDataCachePopulationService;
        }

        public void Populate()
        {
            _messageCachePopulationService.Populate();
            _fileDataCachePopulationService.Populate();
            _internalDataCachePopulationService.Populate();
            _externalDataCachePopulationService.Populate();
        }
    }
}
