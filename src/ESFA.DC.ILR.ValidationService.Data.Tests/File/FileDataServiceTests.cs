using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.File
{
    public class FileDataServiceTests
    {
        [Fact]
        public void UKPRN()
        {
            var ukprn = 99999999;

            var fileDataCacheMock = new Mock<IFileDataCache>();

            fileDataCacheMock.SetupGet(c => c.UKPRN).Returns(ukprn);

            NewService(fileDataCacheMock.Object).UKPRN().Should().Be(ukprn);
        }

        [Fact]
        public void FilePreparationDate()
        {
            var date = new DateTime(2018, 8, 31);

            var fileDataCacheMock = new Mock<IFileDataCache>();

            fileDataCacheMock.SetupGet(c => c.FilePreparationDate).Returns(date);

            NewService(fileDataCacheMock.Object).FilePreparationDate().Should().Be(date);
        }

        [Fact]
        public void LearnerDestinationAndProgressions()
        {
            var learnerDestinationAndProgressions = new List<TestLearnerDestinationAndProgression>
            {
                new TestLearnerDestinationAndProgression
                {
                    LearnRefNumber = "111111"
                }
            };

            var fileDataCacheMock = new Mock<IFileDataCache>();

            fileDataCacheMock.SetupGet(c => c.LearnerDestinationAndProgressions).Returns(learnerDestinationAndProgressions);

            NewService(fileDataCacheMock.Object).LearnerDestinationAndProgressions().Should().BeEquivalentTo(learnerDestinationAndProgressions);
        }

        [Fact]
        public void LearnerDestinationAndProgressionsForLearnRefNumber()
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                LearnRefNumber = "111111"
            };

            var learnerDestinationAndProgressions = new List<TestLearnerDestinationAndProgression>
            {
                learnerDP
            };

            var fileDataCacheMock = new Mock<IFileDataCache>();

            fileDataCacheMock.SetupGet(c => c.LearnerDestinationAndProgressions).Returns(learnerDestinationAndProgressions);

            NewService(fileDataCacheMock.Object).LearnerDestinationAndProgressionsForLearnRefNumber("111111").Should().BeEquivalentTo(learnerDP);
        }

        [Fact]
        public void FileName()
        {
            var fileName = "ILR_11223344";

            var fileDataCacheMock = new Mock<IFileDataCache>();
            fileDataCacheMock.SetupGet(fdc => fdc.FileName).Returns(fileName);

            NewService(fileDataCacheMock.Object).FileName().Should().Be(fileName);
        }

        [Fact]
        public void FileNameUKPRN()
        {
            var fileNameUKPRN = 11223344;

            var fileDataCacheMock = new Mock<IFileDataCache>();
            fileDataCacheMock.SetupGet(fdc => fdc.FileNameUKPRN).Returns(fileNameUKPRN);

            NewService(fileDataCache: fileDataCacheMock.Object).FileNameUKPRN().Should().Be(fileNameUKPRN);
        }

        public FileDataService NewService(IFileDataCache fileDataCache = null)
        {
            return new FileDataService(fileDataCache);
        }
    }
}
