using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.QUALENT3;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Internal
{
    public class QUALENT3DataServiceTests
    {
        [Fact]
        public void Exists_True()
        {
            NewService().Exists("A2").Should().BeTrue();
        }

        [Fact]
        public void Exists_False()
        {
            NewService().Exists("C1").Should().BeFalse();
        }

        private QUALENT3DataService NewService()
        {
            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.QUALENT3s).Returns(new HashSet<string>() { "A1", "A2", "B1" });

            return new QUALENT3DataService(internalDataCacheMock.Object);
        }
    }
}
