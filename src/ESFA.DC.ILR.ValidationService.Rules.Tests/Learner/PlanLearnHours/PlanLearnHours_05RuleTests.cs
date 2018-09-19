using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PlanLearnHours
{
    public class PlanLearnHours_05RuleTests : PlanLearnHoursTestsBase
    {
        [Theory]
        [InlineData(4001, null)]
        [InlineData(4001, 0)]
        [InlineData(4311, 900)]
        public void ConditionMet_True(long? planLearnHours, long? planEeepHours)
        {
            var rule = new PlanLearnHours_05Rule(null);
            rule.ConditionMet(planLearnHours, planEeepHours).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, 4500)]
        [InlineData(3900, 100)]
        public void ConditionMet_False(long? planLearnHours, long? planEeepHours)
        {
            var rule = new PlanLearnHours_05Rule(null);
            rule.ConditionMet(planLearnHours, planEeepHours).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner(4000, 1, null);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PlanLearnHours_05", null, null, null);

            var rule = new PlanLearnHours_05Rule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner(3990, 10, null);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PlanLearnHours_05", null, null, null);

            var rule = new PlanLearnHours_05Rule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }
    }
}
