using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.LLDDCat;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Internal
{
    public class LLDDCatDataServiceTests
    {
        [Fact]
        public void Exists_True()
        {
            NewService().Exists(1).Should().BeTrue();
        }

        [Fact]
        public void Exists_False()
        {
            NewService().Exists(2).Should().BeFalse();
        }

        [Fact]
        public void IsDateValidForLLDDCat_True()
        {
            NewService().IsDateValidForLLDDCat(1, new DateTime(2001, 01, 01)).Should().BeTrue();
        }

        [Fact]
        public void IsDateValidForLLDDCat_False()
        {
            NewService().IsDateValidForLLDDCat(1, new DateTime(2020, 01, 01)).Should().BeFalse();
        }

        [Fact]
        public void IsDateValidForLLDDCat_False_NotValidKey()
        {
            NewService().IsDateValidForLLDDCat(2, new DateTime(2005, 01, 01)).Should().BeFalse();
        }

        private LLDDCatDataService NewService()
        {
            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.LLDDCats)
                .Returns(new Dictionary<int, ValidityPeriods>()
                {
                    { 1, new ValidityPeriods
                        {
                            ValidFrom = new DateTime(2000, 01, 01),
                            ValidTo = new DateTime(2019, 01, 01)
                        }
                    }
                });

            return new LLDDCatDataService(internalDataCacheMock.Object);
        }
    }
}
