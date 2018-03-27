using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_24RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var rule = NewRule();

            rule.ConditionMet(1234, null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_ULN_Null()
        {
            var rule = NewRule();

            rule.ConditionMet(null, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ULN_Temporary()
        {
            var rule = NewRule();

            rule.ConditionMet(9999999999, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DateOfBirth()
        {
            var rule = NewRule();

            rule.ConditionMet(1234, new DateTime(1990, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var learner = new TestLearner()
            {
                ULNNullable = 1234
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("DateOfBirth_24", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                ULNNullable = 1234,
                DateOfBirthNullable = new DateTime(2112, 1, 1)
            };

            var rule = NewRule();

            rule.Validate(learner);
        }

        private DateOfBirth_24Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_24Rule(validationErrorHandler);
        }
    }
}
