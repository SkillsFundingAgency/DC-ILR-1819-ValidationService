using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.Outcome
{
    public class Outcome_01RuleTests : AbstractRuleTests<Outcome_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Outcome_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(0).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(OutcomeConstants.Achieved)]
        [InlineData(OutcomeConstants.PartialAchievement)]
        [InlineData(OutcomeConstants.NoAchievement)]
        [InlineData(OutcomeConstants.LearningActivitiesCompleteButOutcomeNotKnown)]
        public void ConditionMet_False(int? outcome)
        {
            NewRule().ConditionMet(outcome).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutcomeNullable = 0,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(OutcomeConstants.Achieved)]
        [InlineData(OutcomeConstants.PartialAchievement)]
        [InlineData(OutcomeConstants.NoAchievement)]
        [InlineData(OutcomeConstants.LearningActivitiesCompleteButOutcomeNotKnown)]
        public void ValidateNoError(int? outcome)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutcomeNullable = outcome,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("Outcome", OutcomeConstants.Achieved)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(OutcomeConstants.Achieved);

            validationErrorHandlerMock.Verify();
        }

        private Outcome_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new Outcome_01Rule(validationErrorHandler);
        }
    }
}
