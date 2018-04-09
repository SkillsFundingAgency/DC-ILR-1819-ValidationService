using System.IO;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Modules.Stubs
{
    public class MessageFileSystemProviderService : IValidationItemProviderService<IMessage>
    {
        private readonly IFileDataCachePopulationService _fileDataCachePopulationService;
        private readonly ISerializationService _serializationService;

        private IMessage _message;

        public MessageFileSystemProviderService(IFileDataCachePopulationService fileDataCachePopulationService, ISerializationService serializationService)
        {
            _fileDataCachePopulationService = fileDataCachePopulationService;
            _serializationService = serializationService;
        }

        public IMessage Provide(IValidationContext validationContext)
        {
            if (_message == null)
            {
                _message = _serializationService.Deserialize<Message>(File.ReadAllText(validationContext.Input));
                _fileDataCachePopulationService.Populate(_message);
            }

            return _message;
        }
    }
}
