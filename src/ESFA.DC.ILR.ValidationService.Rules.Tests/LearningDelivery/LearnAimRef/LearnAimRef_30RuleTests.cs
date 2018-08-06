using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_30RuleTests : AbstractRuleTests<LearnAimRef_30Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_30");
        }

        [Fact]
        public void ConditionMet_True_LearnAimRef()
        {
            NewRule().ConditionMet("not ZPROG001", 1).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_AimType()
        {
            NewRule().ConditionMet("ZPROG001", 2).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet("ZPROG001", 1).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Mismatch()
        {
            NewRule().ConditionMet("Not ZPROG001", 2).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZPROG001",
                        AimType = 2,
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
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZESF0001",
                        AimType = 2,
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
            var learnAimRef = "LearnAimRef";
            var aimType = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", learnAimRef)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", aimType)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnAimRef, aimType);

            validationErrorHandlerMock.Verify();
        }

        private LearnAimRef_30Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnAimRef_30Rule(validationErrorHandler);
        }
    }
}
