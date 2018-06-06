using System.IO;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class MessageFileSystemProviderServiceStub : IValidationItemProviderService<IMessage>
    {
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IPreValidationContext _preValidationContext;

        private IMessage _message;

        public MessageFileSystemProviderServiceStub(IXmlSerializationService xmlSerializationService, IPreValidationContext preValidationContext)
        {
            _xmlSerializationService = xmlSerializationService;
            _preValidationContext = preValidationContext;
        }

        public IMessage Provide()
        {
            if (_message == null)
            {
                _message = _xmlSerializationService.Deserialize<Message>(File.ReadAllText(_preValidationContext.Input));
            }

            return _message;
        }
    }
}
