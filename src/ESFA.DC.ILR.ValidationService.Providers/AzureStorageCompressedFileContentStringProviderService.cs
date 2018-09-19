using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class AzureStorageCompressedFileContentStringProviderService : IMessageStreamProviderService
    {
        private readonly IPreValidationContext _preValidationContext;
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AzureStorageCompressedFileContentStringProviderService(
            IPreValidationContext preValidationContext,
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IDateTimeProvider dateTimeProvider)
        {
            _preValidationContext = preValidationContext;
            _logger = logger;
            _streamableKeyValuePersistenceService = streamableKeyValuePersistenceService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Stream> Provide(CancellationToken cancellationToken)
        {
            var startDateTime = _dateTimeProvider.GetNowUtc();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            MemoryStream outputStream = new MemoryStream();

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await _streamableKeyValuePersistenceService.GetAsync(_preValidationContext.Input, memoryStream, cancellationToken);

                    using (ZipArchive archive = new ZipArchive(memoryStream))
                    {
                        List<ZipArchiveEntry> xmlFiles = archive.Entries.Where(x =>
                            x.Name.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase)).ToList();

                        if (xmlFiles.Count == 1)
                        {
                            ZipArchiveEntry zippedFile = xmlFiles.First();
                            using (Stream stream = zippedFile.Open())
                            {
                                await stream.CopyToAsync(outputStream, 81920, cancellationToken);
                            }

                            string xmlFileName = $"{ExtractUkrpn(_preValidationContext.Input)}/{zippedFile.Name}";
                            _preValidationContext.Input = xmlFileName;
                            await _streamableKeyValuePersistenceService.SaveAsync(
                                xmlFileName,
                                outputStream,
                                cancellationToken);
                        }
                        else
                        {
                            _logger.LogWarning(
                                $"Zip file contains either more than one file will or no xml file, returning empty stream: jobId: {_preValidationContext.JobId}, file name: {_preValidationContext.Input}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Failed to extract the zip file from storage: jobId: {_preValidationContext.JobId}, file name: {_preValidationContext.Input}",
                    ex);
            }

            stopwatch.Stop();

            var processTimes = new StringBuilder();

            processTimes.Append("Start Time: ");
            processTimes.AppendLine(startDateTime.ToString(CultureInfo.InvariantCulture));
            processTimes.Append("Total Time: ");
            processTimes.AppendLine((DateTime.UtcNow - startDateTime).TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            _logger.LogDebug($"Blob download: {processTimes}");

            return outputStream;
        }

        private string ExtractUkrpn(string fileName)
        {
            if (fileName.Contains("/"))
            {
                return fileName.Split('/')[0];
            }

            return string.Empty;
        }
    }
}