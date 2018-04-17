﻿using System.IO;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class MessageStringProviderService : IValidationItemProviderService<IMessage>
    {
        private readonly ISerializationService _serializationService;
        private readonly IValidationContext _validationContext;

        private IMessage _message;

        public MessageStringProviderService(ISerializationService serializationService, IValidationContext validationContext)
        {
            _serializationService = serializationService;
            _validationContext = validationContext;
        }

        public IMessage Provide()
        {
            if (_message == null)
            {
                _message = _serializationService.Deserialize<Message>(_validationContext.Input);
            }

            return _message;
        }
    }
}