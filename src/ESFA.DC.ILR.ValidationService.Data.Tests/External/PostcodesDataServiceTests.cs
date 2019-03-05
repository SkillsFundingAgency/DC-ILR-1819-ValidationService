using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.External
{
    public class PostcodesDataServiceTests
    {
        /// <summary>
        /// Postcode exists meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, false)]
        [InlineData(" ", false)]
        [InlineData("jkl", false)]
        [InlineData("def", true)]
        [InlineData("ghi", true)]
        public void PostcodeExistsMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var externalDC = new Mock<IExternalDataCache>();
            externalDC
                .SetupGet(rdc => rdc.Postcodes)
                .Returns(new HashSet<string>() { "abc", "def", "ghi" });

            var sut = new PostcodesDataService(externalDC.Object);

            // act
            var result = sut.PostcodeExists(candidate);

            // assert
            Assert.Equal(expectation, result);
        }
    }
}
