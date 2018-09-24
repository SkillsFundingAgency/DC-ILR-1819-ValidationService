using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class AzureStorageFileContentStringProviderService : IMessageStreamProviderService
    {
        private readonly IPreValidationContext _preValidationContext;
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _keyValuePersistenceService;

        public AzureStorageFileContentStringProviderService(
            IPreValidationContext preValidationContext,
            IStreamableKeyValuePersistenceService keyValuePersistenceService,
            ILogger logger)
        {
            _preValidationContext = preValidationContext;
            _logger = logger;
            _keyValuePersistenceService = keyValuePersistenceService;
        }

        public async Task<Stream> Provide(CancellationToken cancellationToken)
        {
            var startDateTime = DateTime.UtcNow;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            MemoryStream memoryStream = new MemoryStream();
            await _keyValuePersistenceService.GetAsync(_preValidationContext.Input, memoryStream, cancellationToken);

            var processTimes = new StringBuilder();

            processTimes.AppendLine($"Start Time : {startDateTime}");
            processTimes.AppendLine($"Total Time : {(DateTime.UtcNow - startDateTime).TotalMilliseconds}");

            _logger.LogDebug($"Blob download :{processTimes} ");

            return memoryStream;
        }
    }
}
