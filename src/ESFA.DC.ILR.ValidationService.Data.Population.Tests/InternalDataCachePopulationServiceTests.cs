using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class InternalDataCachePopulationServiceTests
    {
        [Fact]
        public void Populate()
        {
            var internalDataCache = new InternalDataCache();

            NewService(internalDataCache).Populate();

            var yearStart = new DateTime(2018, 8, 1);
            var fundModels = new List<int> { 10, 25, 35, 36, 70, 81, 82, 99 };

            internalDataCache.AcademicYear.Start.Should().BeSameDateAs(yearStart);
            internalDataCache.FundModels.Should().BeEquivalentTo(fundModels);
            internalDataCache.QUALENT3s.Count().Should().Be(61);
        }

        private InternalDataCachePopulationService NewService(IInternalDataCache internalDataCache = null)
        {
            return new InternalDataCachePopulationService(internalDataCache);
        }
    }
}
