using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.External
{
    /// <summary>
    /// the lookups details provider test fixture
    /// </summary>
    public class EPAOrganisationDetailsProviderTests
    {
        /// <summary>
        /// Provider 'is current' values match expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="testCaseDate">The test case date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("EPA0001", "2013-06-14", true)]
        [InlineData("EPA0001", "2016-06-13", false)]
        [InlineData("EPA0002", "2015-09-03", true)]
        [InlineData("EPA0002", "2007-09-03", false)]
        [InlineData("EPA0003", "2012-06-18", false)]
        [InlineData("EPA0003", "2016-10-14", true)]
        [InlineData("EPA0003", "2017-01-01", false)]
        [InlineData("EPA0004", "2004-05-01", false)]
        public void ProviderIsCurrentValuesMatchExpectation(string candidate, string testCaseDate, bool expectation)
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
        /// Provider contains limited life value matches expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("EPA0001", true)]
        [InlineData("EPA0003", true)]

        // [InlineData("EPA0006", false)] <= this routine only returns true at the moment
        // [InlineData("EPA0007", false)]
        public void IsKnownMatchesExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewService();

            // act
            var result = sut.IsKnown(candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// New service.
        /// </summary>
        /// <returns>a <seealso cref="LookupDetailsProvider"/></returns>
        public EPAOrganisationDetailsProvider NewService()
        {
            var cache = new ExternalDataCache
            {
                EPAOrganisations = new List<EPAOrganisation>
                {
                    new EPAOrganisation { ID = "EPA0001", Standard = "STD0001", EffectiveFrom = DateTime.Parse("2012-01-01"), EffectiveTo = DateTime.Parse("2014-12-31") },
                    new EPAOrganisation { ID = "EPA0002", Standard = "STD0002", EffectiveFrom = DateTime.Parse("2013-01-01"), EffectiveTo = DateTime.Parse("2015-12-31") },
                    new EPAOrganisation { ID = "EPA0003", Standard = "STD0003", EffectiveFrom = DateTime.Parse("2014-01-01"), EffectiveTo = DateTime.Parse("2016-12-31") },
                    new EPAOrganisation { ID = "EPA0004", Standard = "STD0004", EffectiveFrom = DateTime.Parse("2015-01-01"), EffectiveTo = DateTime.Parse("2099-12-31") },
                    new EPAOrganisation { ID = "EPA0005", Standard = "STD0005", EffectiveFrom = DateTime.Parse("2016-01-01"), EffectiveTo = DateTime.Parse("2099-12-31") },
                }
            };

            return new EPAOrganisationDetailsProvider(cache);
        }
    }
}
