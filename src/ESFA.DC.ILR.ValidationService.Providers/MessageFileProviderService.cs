using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class MessageFileProviderService : IValidationItemProviderService<IMessage>
    {
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IMessageStreamProviderService _streamProvider;
        private readonly ICache<string> _messageCache;

        public MessageFileProviderService(
            IXmlSerializationService xmlSerializationService,
            IMessageStreamProviderService streamProvider,
            ICache<string> messageCache)
        {
            _xmlSerializationService = xmlSerializationService;
            _streamProvider = streamProvider;
            _messageCache = messageCache;
        }

        public async Task<IMessage> ProvideAsync(CancellationToken cancellationToken)
        {
            var fileContentCache = (Cache<string>)_messageCache;

            Stream fileStream = await _streamProvider.Provide(cancellationToken);

            if (fileStream == null)
            {
                return null;
            }

            fileStream.Seek(0, SeekOrigin.Begin);
            UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
            using (StreamReader reader = new StreamReader(fileStream, utF8Encoding, true, 1024, true))
            {
                fileContentCache.Item = reader.ReadToEnd();
            }

            fileStream.Seek(0, SeekOrigin.Begin);
            return _xmlSerializationService.Deserialize<Message>(fileStream);
        }
    }
}
