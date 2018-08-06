using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Accom;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.Accom
{
    public class Accom_01RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(4).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        public void ConditionMet_False(long? accomValue)
        {
            NewRule().ConditionMet(accomValue).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                AccomNullable = 11
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("Accom_01", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                AccomNullable = 5
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("Accom_01", null, null, null);

            var rule = new Accom_01Rule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private Accom_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new Accom_01Rule(validationErrorHandler);
        }
    }
}
