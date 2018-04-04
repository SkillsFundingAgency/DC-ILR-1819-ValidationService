using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AddHours
{
    public class AddHours_04RuleTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(60)]
        public void AddHoursConditionMet_False(int? addHours)
        {
            NewRule().AddHoursConditionMet(addHours).Should().BeFalse();
        }

        [Fact]
        public void AddHoursConditionMet_True()
        {
            NewRule().AddHoursConditionMet(61).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(25).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0.0)]
        [InlineData(24.0)]
        public void ConditionMet_False(double? averageHours)
        {
            NewRule().ConditionMet(averageHours).Should().BeFalse();
        }

        private AddHours_04Rule NewRule(ILearningDeliveryQueryService learningDeliveryQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AddHours_04Rule(learningDeliveryQueryService, validationErrorHandler);
        }
    }
}
