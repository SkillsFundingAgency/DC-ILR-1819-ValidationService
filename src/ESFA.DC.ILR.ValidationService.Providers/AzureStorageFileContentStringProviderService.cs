using System;
using System.Diagnostics;
using System.Text;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class AzureStorageFileContentStringProviderService : IMessageStringProviderService
    {
        private readonly IPreValidationContext _preValidationContext;
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _keyValuePersistenceService;

        public AzureStorageFileContentStringProviderService(
            IPreValidationContext preValidationContext,
            [KeyFilter(PersistenceStorageKeys.AzureStorage)]IKeyValuePersistenceService keyValuePersistenceService,
            ILogger logger)
        {
            _preValidationContext = preValidationContext;
            _logger = logger;
            _keyValuePersistenceService = keyValuePersistenceService;
        }

        public string Provide()
        {
            var startDateTime = DateTime.UtcNow;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var fileContentString = _keyValuePersistenceService.GetAsync(_preValidationContext.Input).GetAwaiter().GetResult();

            var processTimes = new StringBuilder();

            processTimes.AppendLine($"Start Time : {startDateTime}");
            processTimes.AppendLine($"Total Time : {(DateTime.UtcNow - startDateTime).TotalMilliseconds}");

            _logger.LogDebug($"Blob download :{processTimes} ");

            return fileContentString;
        }
    }
}
