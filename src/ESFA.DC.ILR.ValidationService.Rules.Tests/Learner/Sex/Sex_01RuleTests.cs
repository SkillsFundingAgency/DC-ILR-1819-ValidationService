using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Sex;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.Sex
{
    public class Sex_01RuleTests
    {
        [Theory]
        [InlineData("f")]
        [InlineData("m")]
        [InlineData("A")]
        public void ConditionMet_True(string sex)
        {
            var rule = NewRule();
            rule.ConditionMet(sex).Should().BeTrue();
        }

        [Theory]
        [InlineData("F")]
        [InlineData("M")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void ConditionMet_False(string sex)
        {
            var rule = NewRule();
            rule.ConditionMet(sex).Should().BeFalse();
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                Sex = "X"
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("Sex_01", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_True()
        {
            var learner = new TestLearner()
            {
                Sex = "F"
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("Sex_01", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private Sex_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new Sex_01Rule(validationErrorHandler);
        }
    }
}