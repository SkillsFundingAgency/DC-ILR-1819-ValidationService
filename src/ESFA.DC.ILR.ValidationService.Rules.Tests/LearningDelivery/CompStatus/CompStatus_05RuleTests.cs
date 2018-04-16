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
    public class CompStatus_05RuleTests : AbstractRuleTests<CompStatus_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("CompStatus_05");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(1, 1).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Outcome()
        {
            NewRule().ConditionMet(null, 1).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_CompStatus()
        {
            NewRule().ConditionMet(1, 2).Should().BeFalse();
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
                        CompStatus = 1,
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

        private CompStatus_05Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new CompStatus_05Rule(validationErrorHandler);
        }
    }
}
