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
    public class LearnAimRef_55RuleTests : AbstractRuleTests<LearnAimRef_55Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_55");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var aimType = 1;
            var fundModel = 1;
            var progType = 1;
            var learnAimRef = "LearnAimRef";

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.TraineeshipConditionMet(fundModel, progType)).Returns(true);
            ruleMock.Setup(r => r.WorkExperienceConditionMet(learnAimRef)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, progType, learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var aimType = 1;
            var fundModel = 1;
            var progType = 1;
            var learnAimRef = "LearnAimRef";

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(false);
            ruleMock.Setup(r => r.TraineeshipConditionMet(fundModel, progType)).Returns(true);
            ruleMock.Setup(r => r.WorkExperienceConditionMet(learnAimRef)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, progType, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Traineeship()
        {
            var aimType = 1;
            var fundModel = 1;
            var progType = 1;
            var learnAimRef = "LearnAimRef";

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.TraineeshipConditionMet(fundModel, progType)).Returns(false);
            ruleMock.Setup(r => r.WorkExperienceConditionMet(learnAimRef)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, progType, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_WorkExperience()
        {
            var aimType = 1;
            var fundModel = 1;
            var progType = 1;
            var learnAimRef = "LearnAimRef";

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.TraineeshipConditionMet(fundModel, progType)).Returns(true);
            ruleMock.Setup(r => r.WorkExperienceConditionMet(learnAimRef)).Returns(false);

            ruleMock.Object.ConditionMet(aimType, fundModel, progType, learnAimRef).Should().BeFalse();
        }

        [Theory]
        [InlineData("Z0007834")]
        [InlineData("Z0007835")]
        [InlineData("Z0007836")]
        [InlineData("Z0007837")]
        [InlineData("Z0007838")]
        [InlineData("ZWRKX001")]
        public void WorkExperienceConditionMet_True(string learnAimRef)
        {
            NewRule().WorkExperienceConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void WorkExperienceConditionMet_False()
        {
            NewRule().WorkExperienceConditionMet("LearnAimRef").Should().BeFalse();
        }

        [Fact]
        public void TraineeshipConditionMet_True()
        {
            NewRule().TraineeshipConditionMet(25, 24).Should().BeTrue();
        }

        [Fact]
        public void TraineeshipConditionMet_False_FundModel()
        {
            NewRule().TraineeshipConditionMet(1, 24).Should().BeFalse();
        }

        [Fact]
        public void TraineeshipConditionMet_False_ProgType()
        {
            NewRule().TraineeshipConditionMet(25, 1).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(5).Should().BeFalse();
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
                        LearnAimRef = "Z0007834",
                        FundModel = 25,
                        ProgTypeNullable = 24,
                        AimType = 1
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
                        LearnAimRef = "Z0007834",
                        FundModel = 25,
                        ProgTypeNullable = 24,
                        AimType = 5
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

        private LearnAimRef_55Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnAimRef_55Rule(validationErrorHandler);
        }
    }
}
