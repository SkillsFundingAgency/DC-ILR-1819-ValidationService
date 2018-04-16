using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.CompStatus
{
    public class CompStatus_06RuleTests : AbstractRuleTests<CompStatus_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("CompStatus_06");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(1, 3).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Outcome()
        {
            NewRule().ConditionMet(2, 3).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_CompStatus()
        {
            NewRule().ConditionMet(1, 1).Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(8)]
        public void OutcomeConditionMet_True(int outcome)
        {
            NewRule().OutcomeConditionMet(outcome).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2)]
        public void OutcomeConditionMet_False(int? outcome)
        {
            NewRule().OutcomeConditionMet(outcome).Should().BeFalse();
        }

        [Theory]
        [InlineData(3)]
        [InlineData(6)]
        public void CompStatusConditionMet_True(int compStatus)
        {
            NewRule().CompStatusConditionMet(compStatus).Should().BeTrue();
        }

        [Fact]
        public void CompStatusConditionMet_False()
        {
            NewRule().CompStatusConditionMet(2).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutcomeNullable = 1,
                        CompStatus = 3,
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
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutcomeNullable = null,
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("CompStatus", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("Outcome", 1)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, 1);

            validationErrorHandlerMock.Verify();
        }

        private CompStatus_06Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new CompStatus_06Rule(validationErrorHandler);
        }
    }
}
