using System.IO;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class MessageFileSystemProviderServiceStub : IValidationItemProviderService<IMessage>
    {
        private readonly ISerializationService _serializationService;
        private readonly IPreValidationContext _preValidationContext;

        private IMessage _message;

        public MessageFileSystemProviderServiceStub(IXmlSerializationService serializationService, IPreValidationContext preValidationContext)
        {
            _serializationService = serializationService;
            _preValidationContext = preValidationContext;
        }

        public IMessage Provide()
        {
            if (_message == null)
            {
                _message = _serializationService.Deserialize<Message>(File.ReadAllText(_preValidationContext.Input));
            }

            return _message;
        }
    }
}
