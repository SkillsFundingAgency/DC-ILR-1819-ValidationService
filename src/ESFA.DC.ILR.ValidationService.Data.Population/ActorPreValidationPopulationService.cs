using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class ActorPreValidationPopulationService : IPreValidationPopulationService<IValidationContext>
    {
        private readonly IFileDataCachePopulationService _fileDataCachePopulationService;

        public ActorPreValidationPopulationService(IFileDataCachePopulationService fileDataCachePopulationService)
        {
            _fileDataCachePopulationService = fileDataCachePopulationService;
        }

        public void Populate(IValidationContext validationContext)
        {
            _fileDataCachePopulationService.Populate();
        }
    }
}
