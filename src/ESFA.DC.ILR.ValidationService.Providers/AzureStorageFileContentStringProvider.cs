using System;
using System.Diagnostics;
using System.Text;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.Logging.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class AzureStorageFileContentStringProvider : IStringProviderService
    {
        private readonly IPreValidationContext _preValidationContext;
        private readonly AzureStorageModel _azureStorageModel;
        private readonly ILogger _logger;

        public AzureStorageFileContentStringProvider(IPreValidationContext preValidationContext, AzureStorageModel azureStorageModel, ILogger logger)
        {
            _preValidationContext = preValidationContext;
            _azureStorageModel = azureStorageModel;
            _logger = logger;
        }

        public string Provide()
        {
            var startDateTime = DateTime.UtcNow;

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_azureStorageModel.AzureBlobConnectionString);

            var cloudStorageAccountElapsed = stopwatch.ElapsedMilliseconds;

            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            var cloudBlobClientElapsed = stopwatch.ElapsedMilliseconds;

            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_azureStorageModel.AzureContainerReference);

            var cloudBlobContainerElapsed = stopwatch.ElapsedMilliseconds;

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(_preValidationContext.Input);

            var cloudBlockBlobElapsed = stopwatch.ElapsedMilliseconds;

            var xmlData = cloudBlockBlob.DownloadText();

            var blobDownloadElapsed = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            var processTimes = new StringBuilder();

            processTimes.AppendLine($"Start Time : {startDateTime}");
            processTimes.AppendLine($"Blob Client : {cloudBlobClientElapsed}");
            processTimes.AppendLine($"Blob Container : {cloudBlobContainerElapsed}");
            processTimes.AppendLine($"Blob Block Blob : {cloudBlockBlobElapsed}");
            processTimes.AppendLine($"Blob Download Text : {blobDownloadElapsed}");
            processTimes.AppendLine($"Total Time : {(DateTime.UtcNow - startDateTime).TotalMilliseconds}");

            _logger.LogDebug($"Blob download :{processTimes} ");

            return xmlData;
        }
    }
}
