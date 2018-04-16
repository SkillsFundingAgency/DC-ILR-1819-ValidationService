using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AddHours
{
    public class AddHours_02RuleTests : AbstractRuleTests<AddHours_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AddHours_02");
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        [InlineData(10)]
        [InlineData(99)]
        public void ConditionMet_True(int fundModel)
        {
            NewRule().ConditionMet(fundModel, 1).Should().BeTrue();
        }

        [Theory]
        [InlineData(25, null)]
        [InlineData(24, 1)]
        public void ConditionMet_False(int fundModel, int? addHours)
        {
            NewRule().ConditionMet(fundModel, addHours).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                        AddHoursNullable = 1,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 24
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AddHours_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AddHours_02Rule(validationErrorHandler);
        }
    }
}
