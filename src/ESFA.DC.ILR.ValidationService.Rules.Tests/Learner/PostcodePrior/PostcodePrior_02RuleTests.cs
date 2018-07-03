using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PostcodePrior;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PostcodePrior
{
    public class PostcodePrior_02RuleTests
    {
        [Theory]
        [InlineData("b01 1WX")]
        [InlineData("bc1 1WX")]
        [InlineData("1x 1WX")]
        [InlineData("1x 1WXTT")]
        [InlineData("G11 1")]
        [InlineData("HHV XXXX")]
        public void ConditionMet_True_FirstTwoCharacters(string postcode)
        {
            var rule = new PostcodePrior_02Rule(null);

            rule.ConditionMet(postcode).Should().BeTrue();
        }

        [Theory]
        [InlineData("A1 1WX")]
        [InlineData("BV1 1WX")]
        [InlineData("")]
        [InlineData(null)]
        public void ConditionMet_False_FirstTwoCharacters(string postcode)
        {
            var rule = new PostcodePrior_02Rule(null);

            rule.ConditionMet(postcode).Should().BeFalse();
        }

        [Theory]
        [InlineData("NZ 1WX")]
        [InlineData("N1c 1WX")]
        [InlineData("NFV 1WX")]
        public void ConditionMet_True_MiddleTwoCharacters(string postcode)
        {
            var rule = new PostcodePrior_02Rule(null);

            rule.ConditionMet(postcode).Should().BeTrue();
        }

        [Theory]
        [InlineData("B20 1WX")]
        [InlineData("C11 1WX")]
        [InlineData("CV22 1WX")]
        [InlineData("N1 1WX")]
        public void ConditionMet_False_MiddleTwoCharacters(string postcode)
        {
            var rule = new PostcodePrior_02Rule(null);

            rule.ConditionMet(postcode).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True_Space()
        {
            var rule = new PostcodePrior_02Rule(null);
            rule.ConditionMet("B111EW").Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Space()
        {
            var rule = new PostcodePrior_02Rule(null);
            rule.ConditionMet("B11 1EW").Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True_AfterSpaceNumbers()
        {
            var rule = new PostcodePrior_02Rule(null);
            rule.ConditionMet("B11 EEW").Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_AfterSpaceNumbers()
        {
            var rule = new PostcodePrior_02Rule(null);
            rule.ConditionMet("B11 9XX").Should().BeFalse();
        }

        [Theory]
        [InlineData("B11 0{0}X")]
        [InlineData("B11 0X{0}")]
        public void ConditionMet_True_AfterSpaceNotAllowedCharacters(string postCodeTemplate)
        {
            var notAllowed = new List<char>() { 'C', 'I', 'K', 'M', 'O', 'V' };
            var rule = new PostcodePrior_02Rule(null);

            foreach (char letter in notAllowed)
            {
                var niNumber = string.Format(postCodeTemplate, letter);
                rule.ConditionMet(niNumber).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData("B11 0{0}X")]
        [InlineData("B11 0X{0}")]
        public void ConditionMet_False_AfterSpaceNotAllowedCharacters(string postCodeTemplate)
        {
            var notAllowed = new List<char>() { 'C', 'I', 'K', 'M', 'O', 'V' };
            char[] allowedCharacters = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).Where(x => !notAllowed.Contains(x)).ToArray();

            var rule = new PostcodePrior_02Rule(null);

            foreach (char letter in allowedCharacters)
            {
                var niNumber = string.Format(postCodeTemplate, letter);
                rule.ConditionMet(niNumber).Should().BeFalse();
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new Mock<ILearner>();
            learner.SetupGet(x => x.PostcodePrior).Returns("B11 1WX");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PostCodePrior_02", null, null, null);

            var rule = new PostcodePrior_02Rule(validationErrorHandlerMock.Object);

            rule.Validate(learner.Object);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new Mock<ILearner>();
            learner.SetupGet(x => x.PostcodePrior).Returns("B1 XXX");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PostCodePrior_02", null, null, null);

            var rule = new PostcodePrior_02Rule(validationErrorHandlerMock.Object);

            rule.Validate(learner.Object);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }
    }
}
