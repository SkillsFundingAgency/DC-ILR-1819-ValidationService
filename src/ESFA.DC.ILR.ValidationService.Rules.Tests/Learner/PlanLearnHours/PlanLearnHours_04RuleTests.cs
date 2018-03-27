using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PlanLearnHours
{
    public class PlanLearnHours_04RuleTests : PlanLearnHoursTestsBase
    {
        [Theory]
        [InlineData(1001, null)]
        [InlineData(1001, 0)]
        [InlineData(101, 900)]
        public void ConditionMet_True(long? planLearnHours, long? planEeepHours)
        {
            var rule = new PlanLearnHours_04Rule(null);
            rule.ConditionMet(planLearnHours, planEeepHours).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, 35)]
        [InlineData(990, 10)]
        public void ConditionMet_False(long? planLearnHours, long? planEeepHours)
        {
            var rule = new PlanLearnHours_04Rule(null);
            rule.ConditionMet(planLearnHours, planEeepHours).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner(1000, 10, null);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PlanLearnHours_04", null, null, null);

            var rule = new PlanLearnHours_04Rule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner(999, 1, null);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PlanLearnHours_04", null, null, null);

            var rule = new PlanLearnHours_04Rule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }
    }
}
