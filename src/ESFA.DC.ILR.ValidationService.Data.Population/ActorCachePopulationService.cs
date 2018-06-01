using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class ActorCachePopulationService : IPopulationService
    {
        private readonly IFileDataCachePopulationService _fileDataCachePopulationService;

        public ActorCachePopulationService(IFileDataCachePopulationService fileDataCachePopulationService)
        {
            _fileDataCachePopulationService = fileDataCachePopulationService;
        }

        public void Populate()
        {
            _fileDataCachePopulationService.Populate();
        }
    }
}
