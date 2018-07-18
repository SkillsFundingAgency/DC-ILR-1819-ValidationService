using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class AzureInternalDataCachePopulationServiceStub : IInternalDataCachePopulationService
    {
        private readonly IInternalDataCache _internalDataCache;
        private readonly AzureStorageModel _azureStorageModel;

        public AzureInternalDataCachePopulationServiceStub(IInternalDataCache internalDataCache, AzureStorageModel azureStorageModel)
        {
            _internalDataCache = internalDataCache;
            _azureStorageModel = azureStorageModel;
        }

        public void Populate()
        {
            var internalDataCache = (InternalDataCache)_internalDataCache;

            XElement lookups;

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_azureStorageModel.AzureBlobConnectionString);

            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_azureStorageModel.AzureContainerReference);

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference("Lookups.xml");

            var xmlData = cloudBlockBlob.DownloadText();

            // on downloading the file from Azure it adds BOM (Byte Order Mark) so remove it before parsing
            string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

            if (xmlData.StartsWith(byteOrderMarkUtf8))
            {
                xmlData = xmlData.Remove(0, byteOrderMarkUtf8.Length);
            }

            lookups = XDocument.Parse(xmlData).Root;

            internalDataCache.AcademicYear = BuildAcademicYear();

            internalDataCache.AimTypes = new HashSet<int>(BuildSimpleLookupEnumerable<int>(lookups, "AimType"));
            internalDataCache.CompStatuses = new HashSet<int>(BuildSimpleLookupEnumerable<int>(lookups, "CompStatus"));
            internalDataCache.EmpOutcomes = new HashSet<int>(BuildSimpleLookupEnumerable<int>(lookups, "EmpOutcome"));
            internalDataCache.FundModels = new HashSet<int>(BuildSimpleLookupEnumerable<int>(lookups, "FundModel"));
            internalDataCache.QUALENT3s = new HashSet<string>(BuildSimpleLookupEnumerable<string>(lookups, "QualEnt3"));
        }

        private AcademicYear BuildAcademicYear()
        {
            return new AcademicYear()
            {
                AugustThirtyFirst = new DateTime(2018, 8, 31),
                End = new DateTime(2019, 7, 31),
                JanuaryFirst = new DateTime(2019, 1, 1),
                JulyThirtyFirst = new DateTime(2019, 7, 31),
                Start = new DateTime(2018, 8, 1),
            };
        }

        private IEnumerable<T> BuildSimpleLookupEnumerable<T>(XElement lookups, string type)
        {
            return lookups
                .Descendants(type)
                .Descendants("option")
                .Attributes("code")
                .Select(c => (T)Convert.ChangeType(c.Value, typeof(T)));
        }
    }
}
