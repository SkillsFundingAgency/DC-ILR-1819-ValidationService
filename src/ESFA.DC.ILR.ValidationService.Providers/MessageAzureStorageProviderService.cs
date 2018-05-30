using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class MessageAzureStorageProviderService : IValidationItemProviderService<IMessage>
    {
        private readonly ISerializationService _serializationService;
        private readonly IPreValidationContext _preValidationContext;
        private readonly AzureStorageModel _azureStorageModel;
        private readonly ILogger _logger;
        private IMessage _message;

        public MessageAzureStorageProviderService(
            IXmlSerializationService serializationService,
            IPreValidationContext prePreValidationContext,
            AzureStorageModel azureStorageModel,
            ILogger logger)
        {
            _serializationService = serializationService;
            _preValidationContext = prePreValidationContext;
            _azureStorageModel = azureStorageModel;
            _logger = logger;
        }

        public IMessage Provide()
        {
            if (_message == null)
            {
                // get message from azure storage
                var startDateTime = DateTime.UtcNow;

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                CloudStorageAccount cloudStorageAccount =
                    CloudStorageAccount.Parse(_azureStorageModel.AzureBlobConnectionString);

                var cloudStorageAccountElapsed = stopwatch.ElapsedMilliseconds;

                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

                var cloudBlobClientElapsed = stopwatch.ElapsedMilliseconds;

                CloudBlobContainer cloudBlobContainer =
                    cloudBlobClient.GetContainerReference(_azureStorageModel.AzureContainerReference);

                var cloudBlobContainerElapsed = stopwatch.ElapsedMilliseconds;

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(_preValidationContext.Input);

                var cloudBlockBlobElapsed = stopwatch.ElapsedMilliseconds;

                var xmlData = cloudBlockBlob.DownloadText();

                var blobDownloadElapsed = stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();

                _message = _serializationService.Deserialize<Message>(xmlData);
                var deserializedElapsed = stopwatch.ElapsedMilliseconds;

                var processTimes = new StringBuilder();
                processTimes.AppendLine($"Start Time : {startDateTime}");
                processTimes.AppendLine($"Learners : {_message.Learners.Count}");
                processTimes.AppendLine($"Blob Client : {cloudBlobClientElapsed}");
                processTimes.AppendLine($"Blob Container : {cloudBlobContainerElapsed}");
                processTimes.AppendLine($"Blob Block Blob : {cloudBlockBlobElapsed}");
                processTimes.AppendLine($"Blob Download Text : {blobDownloadElapsed}");
                processTimes.AppendLine($"Deserialize ms : {deserializedElapsed}");
                processTimes.AppendLine($"Total Time : {(DateTime.UtcNow - startDateTime).TotalMilliseconds}");

                _logger.LogInfo($"Blob download :{processTimes} ");
            }

            return _message;
        }
    }
}
