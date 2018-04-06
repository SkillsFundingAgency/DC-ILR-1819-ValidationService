using System;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.FileData.Tests
{
    public class FileDataCachePopulationServiceTests
    {
        [Fact]
        public void Populate_FilePreparationDate()
        {
            var fileDataCache = new FileDataCache();
            var fileDataCachePopulationService = new FileDataCachePopulationService(fileDataCache);

            var filePreparationDate = new DateTime(2018, 1, 5);

            var message = new TestMessage()
            {
                HeaderEntity = new TestHeader()
                {
                    CollectionDetailsEntity = new TestCollectionDetails()
                    {
                        FilePreparationDate = filePreparationDate
                    }
                }
            };

            fileDataCachePopulationService.Populate(message);

            fileDataCache.FilePreparationDate.Should().Be(filePreparationDate);
        }
    }
}
