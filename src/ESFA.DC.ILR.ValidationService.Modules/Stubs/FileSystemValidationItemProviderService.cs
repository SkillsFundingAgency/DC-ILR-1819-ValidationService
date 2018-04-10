using System.Collections.Generic;
using System.IO;
using System.Linq;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Modules.Stubs
{
    public class FileSystemValidationItemProviderService : IValidationItemProviderService<ILearner>
    {
        private readonly IFileDataCachePopulationService _fileDataCachePopulationService;
        private readonly ISerializationService _serializationService;

        public FileSystemValidationItemProviderService(IFileDataCachePopulationService fileDataCachePopulationService, ISerializationService serializationService)
        {
            _fileDataCachePopulationService = fileDataCachePopulationService;
            _serializationService = serializationService;
        }

        public IEnumerable<ILearner> Provide(IValidationContext validationContext)
        {
            var message = _serializationService.Deserialize<Message>(File.ReadAllText(validationContext.Input));

            _fileDataCachePopulationService.Populate(message);

            List<MessageLearner> learners = message.Learner.ToList();

            return learners;
        }
    }
}
