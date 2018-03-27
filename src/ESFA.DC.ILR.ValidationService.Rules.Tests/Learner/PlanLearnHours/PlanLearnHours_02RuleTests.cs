using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PlanLearnHours
{
    public class PlanLearnHours_02RuleTests : PlanLearnHoursTestsBase
    {
        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(81)]
        [InlineData(10)]
        [InlineData(99)]
        public void ConditionMet_True(long? fundModel)
        {
            var rule = new PlanLearnHours_02Rule(null);
            rule.ConditionMet(0, fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, 35)]
        [InlineData(0, 1000)]
        [InlineData(10, 35)]
        public void ConditionMet_False(long? planHours, long? fundModel)
        {
            var rule = new PlanLearnHours_02Rule(null);
            rule.ConditionMet(planHours, fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(333)]
        public void ConditionMet_FundModel_False(long? fundModel)
        {
            var rule = new PlanLearnHours_02Rule(null);
            rule.FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(81)]
        [InlineData(10)]
        [InlineData(99)]
        public void ConditionMet_FundModel_True(long? fundModel)
        {
            var rule = new PlanLearnHours_02Rule(null);
            rule.FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner(0, null, 35);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PlanLearnHours_02", null, null, null);

            var rule = new PlanLearnHours_02Rule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner(10, null, 35);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PlanLearnHours_02", null, null, null);

            var rule = new PlanLearnHours_02Rule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }
    }
}
