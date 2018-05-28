using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class ActorPreValidationPopulationService : IPreValidationPopulationService<IValidationContext>
    {
        private readonly IMessageCacheWithDataPopulationService _messageCacheWithDataPopulationService;
        private readonly IFileDataCachePopulationService _fileDataCachePopulationService;
        private readonly IInternalDataCacheWithDataPopulationService _internalDataCacheWithDataPopulationService;
        private readonly IExternalDataCachePopulationService _externalDataCachePopulationService;

        public ActorPreValidationPopulationService(
            IMessageCacheWithDataPopulationService messageCacheWithDataPopulationService,
            IFileDataCachePopulationService fileDataCachePopulationService,
            IInternalDataCacheWithDataPopulationService internalDataCacheWithDataPopulationService,
            IExternalDataCachePopulationService externalDataCachePopulationService)
        {
            _messageCacheWithDataPopulationService = messageCacheWithDataPopulationService;
            _fileDataCachePopulationService = fileDataCachePopulationService;
            _internalDataCacheWithDataPopulationService = internalDataCacheWithDataPopulationService;
            _externalDataCachePopulationService = externalDataCachePopulationService;
        }

        public void Populate(IValidationContext validationContext)
        {
            _messageCacheWithDataPopulationService.Populate(validationContext.Input);
            _fileDataCachePopulationService.Populate();
            _internalDataCacheWithDataPopulationService.Populate(validationContext.InternalDataCache);
            _externalDataCachePopulationService.Populate();
        }
    }
}
