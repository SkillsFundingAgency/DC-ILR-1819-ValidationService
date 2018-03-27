using System;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.ExternalData.Tests.FileDataService
{
    public class FileDataServiceTests
    {
        [Fact]
        public void Populate_FilePreparationDate()
        {
            var fileData = new ExternalData.FileDataService.FileDataService();

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

            fileData.Populate(message);

            fileData.FilePreparationDate.Should().Be(filePreparationDate);
        }
    }
}
