using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PlanLearnHours
{
    public class PlanLearnHours_03RuleTests : PlanLearnHoursTestsBase
    {
        [Theory]
        [InlineData(25)]
        [InlineData(82)]

        public void ConditionMet_True(long? fundModel)
        {
            var rule = new PlanLearnHours_03Rule(null);
            rule.ConditionMet(0, 0, fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData(null, 35, null)]
        [InlineData(0, 10, 1000)]
        [InlineData(0, 10, 35)]
        public void ConditionMet_False(long? planLearnHours, long? planEeepHours, long? fundModel)
        {
            var rule = new PlanLearnHours_03Rule(null);
            rule.ConditionMet(planLearnHours, planEeepHours, fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(999)]
        public void ConditionMet_FundModel_False(long? fundModel)
        {
            var rule = new PlanLearnHours_03Rule(null);
            rule.FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]

        public void ConditionMet_FundModel_True(long? fundModel)
        {
            var rule = new PlanLearnHours_03Rule(null);
            rule.FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner(0, 0, 25);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PlanLearnHours_03", null, null, null);

            var rule = new PlanLearnHours_03Rule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner(0, 10, 25);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PlanLearnHours_03", null, null, null);

            var rule = new PlanLearnHours_03Rule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }
    }
}
