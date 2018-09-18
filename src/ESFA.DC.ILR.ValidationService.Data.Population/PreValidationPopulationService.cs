using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class PreValidationPopulationService : IPopulationService
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

        public async Task PopulateAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(
                _messageCachePopulationService.PopulateAsync(cancellationToken),
                _fileDataCachePopulationService.PopulateAsync(cancellationToken),
                _internalDataCachePopulationService.PopulateAsync(cancellationToken),
                _externalDataCachePopulationService.PopulateAsync(cancellationToken));
        }
    }
}
