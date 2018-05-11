using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class ActorPreValidationPopulationService : IPreValidationPopulationService<IMessage>
    {
        private readonly IMessageCachePopulationService _messageCachePopulationService;
        private readonly IFileDataCachePopulationService _fileDataCachePopulationService;
        private readonly IInternalDataCachePopulationService _internalDataCachePopulationService;
        private readonly IExternalDataCachePopulationService _externalDataCachePopulationService;

        public ActorPreValidationPopulationService(
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
      

        public void Populate(IMessage data)
        {
            _messageCachePopulationService.Populate(data);
            _fileDataCachePopulationService.Populate();
            _internalDataCachePopulationService.Populate();
            _externalDataCachePopulationService.Populate();
        }
    }
}
