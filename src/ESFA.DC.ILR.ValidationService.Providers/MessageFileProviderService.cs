using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class MessageFileProviderService : IValidationItemProviderService<IMessage>
    {
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IMessageStreamProviderService _streamProvider;

        public MessageFileProviderService(
            IXmlSerializationService xmlSerializationService,
            IMessageStreamProviderService streamProvider)
        {
            _xmlSerializationService = xmlSerializationService;
            _streamProvider = streamProvider;
        }

        public async Task<IMessage> ProvideAsync(CancellationToken cancellationToken)
        {
            using (var stream = await _streamProvider.Provide(cancellationToken))
            {
                stream.Seek(0, SeekOrigin.Begin);

                return _xmlSerializationService.Deserialize<Message>(stream);
            }
        }
    }
}
