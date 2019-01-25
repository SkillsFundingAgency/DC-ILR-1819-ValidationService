using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.SSN;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.SSN
{
    public class SSN_02RuleTests : AbstractRuleTests<SSN_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("SSN_02");
        }

        [Fact]
        public void ConditionMet_True_RegexInvalid()
        {
            var ssn = "ABCD123456789";

            NewRule().ConditionMet(ssn).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_CheckSumInvalid()
        {
            var ssn = "ABCD12345678A";

            NewRule().ConditionMet(ssn).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var ssn = "WADM46891352A";

            NewRule().ConditionMet(ssn).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            SSN = "ABCD12345678A"
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            SSN = "WADM46891352A"
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_NullLearningDeliveryHE()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_NullSSN()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            SSN = null
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void RegexIsValid_True_FirstCharacter()
        {
            var notAllowed = new List<char>() { 'I', 'O', 'Q', 'Z' };
            char[] allowed = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).Where(x => !notAllowed.Contains(x)).ToArray();

            foreach (char letter in allowed)
            {
                var ssn = $"{letter}AAA12345678A";
                NewRule().RegexIsValid(ssn).Should().BeTrue();
            }
        }

        [Fact]
        public void RegexIsValid_True_SecondCharacter()
        {
            var notAllowed = new List<char>() { 'I', 'O', 'Q', 'Z' };
            char[] allowed = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).Where(x => !notAllowed.Contains(x)).ToArray();

            foreach (char letter in allowed)
            {
                var ssn = $"A{letter}AA12345678A";
                NewRule().RegexIsValid(ssn).Should().BeTrue();
            }
        }

        [Fact]
        public void RegexIsValid_True_ThirdCharacter()
        {
            var notAllowed = new List<char>() { 'I', 'O', 'Q', 'Z' };
            char[] allowed = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).Where(x => !notAllowed.Contains(x)).ToArray();

            foreach (char letter in allowed)
            {
                var ssn = $"AA{letter}A12345678A";
                NewRule().RegexIsValid(ssn).Should().BeTrue();
            }
        }

        [Fact]
        public void RegexNotValid_True_FourthCharacter()
        {
            var notAllowed = new List<char>() { 'I', 'O', 'Q', 'Z' };
            char[] allowed = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).Where(x => !notAllowed.Contains(x)).ToArray();

            foreach (char letter in allowed)
            {
                var ssn = $"AAA{letter}12345678A";
                NewRule().RegexIsValid(ssn).Should().BeTrue();
            }
        }

        [Fact]
        public void RegexIsValid_True_Characters5To12()
        {
            var ssn = "AAAA12345678A";
            NewRule().RegexIsValid(ssn).Should().BeTrue();
        }

        [Fact]
        public void RegexIsValid_True_LastCharacter()
        {
            var notAllowed = new List<char>() { 'I', 'O', 'Q' };
            char[] allowed = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).Where(x => !notAllowed.Contains(x)).ToArray();

            foreach (char letter in allowed)
            {
                var ssn = $"AAAA12345678{letter}";
                NewRule().RegexIsValid(ssn).Should().BeTrue();
            }
        }

        [Fact]
        public void RegexIsValid_False_FirstCharacter()
        {
            var notAllowed = new List<char>() { 'I', 'O', 'Q', 'Z' };

            foreach (char letter in notAllowed)
            {
                var ssn = $"{letter}AAA12345678A";
                NewRule().RegexIsValid(ssn).Should().BeFalse();
            }
        }

        [Fact]
        public void RegexIsValid_False_SecondCharacter()
        {
            var notAllowed = new List<char>() { 'I', 'O', 'Q', 'Z' };

            foreach (char letter in notAllowed)
            {
                var ssn = $"A{letter}AA12345678A";
                NewRule().RegexIsValid(ssn).Should().BeFalse();
            }
        }

        [Fact]
        public void RegexIsValid_False_ThirdCharacter()
        {
            var notAllowed = new List<char>() { 'I', 'O', 'Q', 'Z' };

            foreach (char letter in notAllowed)
            {
                var ssn = $"AA{letter}A12345678A";
                NewRule().RegexIsValid(ssn).Should().BeFalse();
            }
        }

        [Fact]
        public void RegexIsValid_False_FourthCharacter()
        {
            var notAllowed = new List<char>() { 'I', 'O', 'Q', 'Z' };

            foreach (char letter in notAllowed)
            {
                var ssn = $"AAA{letter}12345678A";
                NewRule().RegexIsValid(ssn).Should().BeFalse();
            }
        }

        [Theory]
        [InlineData("AAAAX2345678A")]
        [InlineData("AAAA1X345678A")]
        [InlineData("AAAA12X45678A")]
        [InlineData("AAAA123X5678A")]
        [InlineData("AAAA1234X678A")]
        [InlineData("AAAA12345X78A")]
        [InlineData("AAAA123456X8A")]
        [InlineData("AAAA1234567XA")]
        public void RegexIsValid_False_Characters5To12(string ssn)
        {
            NewRule().RegexIsValid(ssn).Should().BeFalse();
        }

        [Fact]
        public void RegexIsValid_False_LastCharacter()
        {
            var notAllowed = new List<char>() { 'I', 'O', 'Q' };

            foreach (char letter in notAllowed)
            {
                var ssn = $"AAAA12345678{letter}";
                NewRule().RegexIsValid(ssn).Should().BeFalse();
            }
        }

        [Fact]
        public void ActualCheckSumMatchesExpected_True()
        {
            var ssn = "WADM46891352A";

            NewRule().CheckSumMatchesExpected(ssn).Should().BeTrue();
        }

        [Fact]
        public void ActualCheckSumMatchesExpected_False()
        {
            var ssn = "WADM46891352D";

            NewRule().CheckSumMatchesExpected(ssn).Should().BeFalse();
        }

        [Fact]
        public void GetCalculatedCheckSumValue_Match()
        {
            var ssn = "WADM46891352A";
            var expectedCheckSumValue = 1;

            var actualCheckSumValue = NewRule().GetCalculatedCheckSumValue(ssn);

            expectedCheckSumValue.Should().Be(actualCheckSumValue);
        }

        [Fact]
        public void GetCalculatedCheckSumValue_NoMatch()
        {
            var ssn = "WADM46891352D";
            var expectedCheckSumValue = 3;

            var actualCheckSumValue = NewRule().GetCalculatedCheckSumValue(ssn);

            expectedCheckSumValue.Should().NotBe(actualCheckSumValue);
        }

        [Theory]
        [InlineData(1, "A")]
        [InlineData(2, "B")]
        [InlineData(3, "C")]
        [InlineData(4, "D")]
        [InlineData(5, "E")]
        [InlineData(6, "F")]
        [InlineData(7, "G")]
        [InlineData(8, "H")]
        [InlineData(9, "J")]
        [InlineData(10, "K")]
        [InlineData(11, "L")]
        [InlineData(12, "M")]
        [InlineData(13, "N")]
        [InlineData(14, "P")]
        [InlineData(15, "R")]
        [InlineData(16, "S")]
        [InlineData(17, "T")]
        [InlineData(18, "U")]
        [InlineData(19, "V")]
        [InlineData(20, "W")]
        [InlineData(21, "X")]
        [InlineData(22, "Y")]
        [InlineData(23, "Z")]
        public void GetLetter_Match(int input, string expectedLetter)
        {
            var result = NewRule().GetLetter(input);

            result.Should().Be(expectedLetter);
        }

        [Theory]
        [InlineData(24)]
        [InlineData(100)]
        public void GetLetter_NoMatch(int input)
        {
            var result = NewRule().GetLetter(input);

            result.Should().Be(null);
        }

        [Theory]
        [InlineData(0, "A")]
        [InlineData(1, "B")]
        [InlineData(2, "C")]
        [InlineData(3, "D")]
        [InlineData(4, "E")]
        [InlineData(5, "F")]
        [InlineData(6, "G")]
        [InlineData(7, "H")]
        [InlineData(8, "J")]
        [InlineData(9, "K")]
        [InlineData(10, "L")]
        [InlineData(11, "M")]
        [InlineData(12, "N")]
        [InlineData(13, "P")]
        [InlineData(14, "R")]
        [InlineData(15, "S")]
        [InlineData(16, "T")]
        [InlineData(17, "U")]
        [InlineData(18, "V")]
        [InlineData(19, "W")]
        [InlineData(20, "X")]
        [InlineData(21, "Y")]
        public void GetLetterValue_Match(int expectedValue, string letter)
        {
            var result = NewRule().GetLetterValue(letter);

            result.Should().Be(expectedValue);
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.SSN, "123A")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("123A");

            validationErrorHandlerMock.Verify();
        }

        private SSN_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new SSN_02Rule(validationErrorHandler);
        }
    }
}
