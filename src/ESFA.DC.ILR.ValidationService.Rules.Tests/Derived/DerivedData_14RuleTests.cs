using ESFA.DC.ILR.ValidationService.Rules.Derived;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_14RuleTests
    {
        [Theory]
        [InlineData(123456789, '9')]
        [InlineData(100000002, '2')]
        [InlineData(100000010, '0')]
        [InlineData(100000134, '4')]
        [InlineData(100000142, '2')]
        [InlineData(100000150, '0')]
        [InlineData(100000169, '9')]
        [InlineData(100000177, '7')]
        [InlineData(100000185, '5')]
        [InlineData(100000193, '3')]
        [InlineData(100000207, '7')]
        [InlineData(10000020, 'X')]
        [InlineData(1000002, 'X')]
        [InlineData(100000, 'X')]
        public void GetWorkPlaceEmpIdChecksumMeetsExpectation(int candidate, char expectation)
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.GetWorkPlaceEmpIdChecksum(candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        public DerivedData_14Rule NewRule()
        {
            return new DerivedData_14Rule();
        }
    }
}
