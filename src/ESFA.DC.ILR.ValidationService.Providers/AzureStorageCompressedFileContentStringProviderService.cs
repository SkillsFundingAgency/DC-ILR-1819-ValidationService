using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class AzureStorageCompressedFileContentStringProviderService : IMessageStringProviderService
    {
        private readonly IPreValidationContext _preValidationContext;
        private readonly AzureStorageModel _azureStorageModel;
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;

        public AzureStorageCompressedFileContentStringProviderService(IPreValidationContext preValidationContext, AzureStorageModel azureStorageModel, ILogger logger, IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService)
        {
            _preValidationContext = preValidationContext;
            _azureStorageModel = azureStorageModel;
            _logger = logger;
            _streamableKeyValuePersistenceService = streamableKeyValuePersistenceService;
        }

        public string Provide()
        {
            var startDateTime = DateTime.UtcNow;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            string fileContentString = string.Empty;

            using (var memoryStream = new MemoryStream())
            {
                _streamableKeyValuePersistenceService.GetAsync(_preValidationContext.Input, memoryStream).GetAwaiter().GetResult();

                 var archive = new ZipArchive(memoryStream);
                var xmlFiles = archive.Entries.Where(x =>
                    x.Name.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase));

                if (xmlFiles.Count() == 1)
                {
                    var zippedFile = xmlFiles.First();
                    using (var stream = zippedFile.Open())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            fileContentString = reader.ReadToEnd();
                        }

                        stream.Position = 0;
                         _streamableKeyValuePersistenceService.SaveAsync(
                            _preValidationContext.Input.Replace(".zip", ".xml"), stream);
                    }
                }
            }

            stopwatch.Stop();

            var processTimes = new StringBuilder();

            processTimes.AppendLine($"Start Time : {startDateTime}");
            processTimes.AppendLine($"Total Time : {(DateTime.UtcNow - startDateTime).TotalMilliseconds}");

            _logger.LogDebug($"Blob download :{processTimes} ");

            return fileContentString;
        }
    }
}
