using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LLDDHealthProb
{
    public class LLDDHealthProb_01RuleTests
    {
        [Theory]
        [InlineData(100)]
        [InlineData(0)]
        public void ConditionMet_True(long? value)
        {
            var rule = NewRule();
            rule.ConditionMet(value).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(9)]
        public void ConditionMet_False(long? value)
        {
            var rule = NewRule();
            rule.ConditionMet(value).Should().BeFalse();
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                LLDDHealthProbNullable = 0
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LLDDHealthProb_01", null, null, null);
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
                LLDDHealthProbNullable = 1
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LLDDHealthProb_01", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private LLDDHealthProb_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LLDDHealthProb_01Rule(validationErrorHandler);
        }
    }
}
