using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.TTAccom;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Internal
{
    /// <summary>
    /// the term time accomodation details provider test fixture
    /// </summary>
    public class TermTimeAccomodationDetailsProviderTests
    {
        /// <summary>
        /// Provider 'is current' values match expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="testCaseDate">The test case date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(1, "2013-06-14", true)]
        [InlineData(1, "2013-06-13", false)]
        [InlineData(2, "2015-09-03", true)]
        [InlineData(2, "2007-09-03", false)]
        [InlineData(3, "2012-06-18", false)]
        [InlineData(5, "2013-06-14", true)]
        [InlineData(5, "2010-10-14", false)]
        [InlineData(9, "2004-05-01", true)]
        [InlineData(9, "2008-08-27", false)]
        public void ProviderIsCurrentValuesMatchExpectation(int candidate, string testCaseDate, bool expectation)
        {
            // arrange
            var sut = NewService();
            var testDate = DateTime.Parse(testCaseDate);

            // act
            var result = sut.IsCurrent(candidate, testDate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// New service.
        /// </summary>
        /// <returns>a <seealso cref="TermTimeAccomodationDetailsProvider"/></returns>
        public TermTimeAccomodationDetailsProvider NewService()
        {
            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock
                .SetupGet(c => c.TTAccoms)
                .Returns(new Dictionary<int, ValidityPeriods>()
                {
                    [1] = new ValidityPeriods(validFrom: DateTime.Parse("2013-06-14"), validTo: DateTime.Parse("2020-06-14")),
                    [2] = new ValidityPeriods(validFrom: DateTime.Parse("2009-04-28"), validTo: DateTime.Parse("2020-06-14")),
                    [4] = new ValidityPeriods(validFrom: DateTime.Parse("2012-09-06"), validTo: DateTime.Parse("2015-02-28")),
                    [5] = new ValidityPeriods(validFrom: DateTime.Parse("2010-11-21"), validTo: DateTime.Parse("2020-06-14")),
                    [6] = new ValidityPeriods(validFrom: DateTime.Parse("2018-07-02"), validTo: DateTime.Parse("2020-06-14")),
                    [9] = new ValidityPeriods(validFrom: DateTime.Parse("2000-02-01"), validTo: DateTime.Parse("2008-08-26")),
                });

            return new TermTimeAccomodationDetailsProvider(internalDataCacheMock.Object);
        }
    }
}
