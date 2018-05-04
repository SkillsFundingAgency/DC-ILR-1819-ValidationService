using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.FileData.Tests
{
    public class FileDataCachePopulationServiceTests
    {
        [Fact]
        public void Populate()
        {
            var filePreparationDate = new DateTime(2018, 1, 5);
            var ukprn = 1;

            var message = new TestMessage()
            {
                HeaderEntity = new TestHeader()
                {
                    CollectionDetailsEntity = new TestCollectionDetails()
                    {
                        FilePreparationDate = filePreparationDate
                    }
                },
                LearningProviderEntity = new TestLearningProvider()
                {
                    UKPRN = ukprn,
                },
            };

            var messageCacheMock = new Mock<ICache<IMessage>>();

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var fileDataCache = new FileDataCache();

            var fileDataCachePopulationService = new FileDataCachePopulationService(fileDataCache, messageCacheMock.Object);

            fileDataCachePopulationService.Populate();

            fileDataCache.FilePreparationDate.Should().Be(filePreparationDate);
            fileDataCache.UKPRN.Should().Be(ukprn);
        }
    }
}
