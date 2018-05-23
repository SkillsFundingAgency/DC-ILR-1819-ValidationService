using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.External
{
    public class PostcodesDataServiceTests
    {
        [Fact]
        public void Exists_True()
        {
            var referenceDataCacheMock = new Mock<IExternalDataCache>();

            referenceDataCacheMock.SetupGet(rdc => rdc.Postcodes).Returns(new HashSet<string>() { "abc", "def", "ghi" });

            NewService(referenceDataCacheMock.Object).PostcodeExists("abc").Should().BeTrue();
        }

        [Fact]
        public void Exists_False()
        {
            var referenceDataCacheMock = new Mock<IExternalDataCache>();

            referenceDataCacheMock.SetupGet(rdc => rdc.Postcodes).Returns(new HashSet<string>() { "abc", "def", "ghi" });

            NewService(referenceDataCacheMock.Object).PostcodeExists("jkl").Should().BeFalse();
        }

        [Fact]
        public void Exists_False_Null()
        {
            NewService().PostcodeExists(null).Should().BeFalse();
        }

        [Fact]
        public void Exists_False_WhiteSpace()
        {
            NewService().PostcodeExists(" ").Should().BeFalse();
        }

        private PostcodesDataService NewService(IExternalDataCache externalDataCache = null)
        {
            return new PostcodesDataService(externalDataCache);
        }
    }
}
