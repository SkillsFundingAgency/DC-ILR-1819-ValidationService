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
    public class CompStatus_04RuleTests : AbstractRuleTests<CompStatus_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("CompStatus_04");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(null, 2).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Outcome()
        {
            NewRule().ConditionMet(1, 1).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_CompStatus()
        {
            NewRule().ConditionMet(null, 1).Should().BeFalse();
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
                        OutcomeNullable = null,
                        CompStatus = 2,
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
                        OutcomeNullable = 1,
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

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private CompStatus_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new CompStatus_04Rule(validationErrorHandler);
        }
    }
}
