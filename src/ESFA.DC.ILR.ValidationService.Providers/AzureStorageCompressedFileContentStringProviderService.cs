using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
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
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;

        public AzureStorageCompressedFileContentStringProviderService(
            IPreValidationContext preValidationContext,
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService)
        {
            _preValidationContext = preValidationContext;
            _logger = logger;
            _streamableKeyValuePersistenceService = streamableKeyValuePersistenceService;
        }

        public string Provide()
        {
            var startDateTime = DateTime.UtcNow;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var fileContentString = string.Empty;

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    _streamableKeyValuePersistenceService.GetAsync(_preValidationContext.Input, memoryStream)
                        .GetAwaiter().GetResult();

                    var archive = new ZipArchive(memoryStream);
                    var xmlFiles = archive.Entries.Where(x =>
                        x.Name.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase)).ToList();

                    if (xmlFiles.Count == 1)
                    {
                        var zippedFile = xmlFiles.First();
                        using (var stream = zippedFile.Open())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                fileContentString = reader.ReadToEnd();
                            }

                            var xmlFileName = $"{ExtractUkrpn(_preValidationContext.Input)}/{zippedFile.Name}";
                            _preValidationContext.Input = xmlFileName;
                            _streamableKeyValuePersistenceService.SaveAsync(xmlFileName, stream);
                        }
                    }
                    else
                    {
                        _logger.LogWarning(
                            $"Zip file contains either more than one file will or no xml file, return empty string: jobId : {_preValidationContext.JobId}, file name :{_preValidationContext.Input}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Failed to extract the zip file from storage : jobId : {_preValidationContext.JobId}, file name :{_preValidationContext.Input}",
                    ex);
            }

            stopwatch.Stop();

            var processTimes = new StringBuilder();

            processTimes.AppendLine($"Start Time : {startDateTime}");
            processTimes.AppendLine($"Total Time : {(DateTime.UtcNow - startDateTime).TotalMilliseconds}");

            _logger.LogDebug($"Blob download :{processTimes} ");

            return fileContentString;
        }

        public string ExtractUkrpn(string fileName)
        {
            if (fileName.Contains("/"))
            {
                return fileName.Split('/')[0];
            }

            return string.Empty;
        }
    }
}