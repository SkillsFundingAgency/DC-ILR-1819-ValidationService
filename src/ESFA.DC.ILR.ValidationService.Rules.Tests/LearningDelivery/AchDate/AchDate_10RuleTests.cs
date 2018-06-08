using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AchDate
{
    public class AchDate_10RuleTests : AbstractRuleTests<AchDate_10Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AchDate_10");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var achDate = new DateTime(2017, 1, 1);
            var learnActEndDate = new DateTime(2017, 1, 1);
            var aimType = 1;
            var progType = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.AchDateConditionMet(achDate, learnActEndDate)).Returns(true);
            rule.Setup(r => r.TraineeshipConditionMet(aimType, progType)).Returns(true);

            rule.Object.ConditionMet(achDate, learnActEndDate, aimType, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_AchDate()
        {
            var achDate = new DateTime(2017, 1, 1);
            var learnActEndDate = new DateTime(2017, 1, 1);
            var aimType = 1;
            var progType = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.AchDateConditionMet(achDate, learnActEndDate)).Returns(false);

            rule.Object.ConditionMet(achDate, learnActEndDate, aimType, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Traineeship()
        {
            var achDate = new DateTime(2017, 1, 1);
            var learnActEndDate = new DateTime(2017, 1, 1);
            var aimType = 1;
            var progType = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.AchDateConditionMet(achDate, learnActEndDate)).Returns(true);
            rule.Setup(r => r.TraineeshipConditionMet(aimType, progType)).Returns(false);

            rule.Object.ConditionMet(achDate, learnActEndDate, aimType, progType).Should().BeFalse();
        }

        [Fact]
        public void AchDateConditionMet_False_AchDateNull()
        {
            NewRule().AchDateConditionMet(null, new DateTime(2017, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void AchDateConditionMet_False_LearnActEndDateNull()
        {
            NewRule().AchDateConditionMet(new DateTime(2017, 1, 1), null).Should().BeFalse();
        }

        [Fact]
        public void AchDateConditionMet_False()
        {
            NewRule().AchDateConditionMet(new DateTime(2017, 1, 1), new DateTime(2017, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void AchDateConditionMet_True()
        {
            NewRule().AchDateConditionMet(new DateTime(2017, 7, 1), new DateTime(2017, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void TraineehipConditionMet_True()
        {
            NewRule().TraineeshipConditionMet(1, 24).Should().BeTrue();
        }

        [Fact]
        public void TraineeshipConditionMet_False_AimType()
        {
            NewRule().TraineeshipConditionMet(2, 24).Should().BeFalse();
        }

        [Fact]
        public void TraineeshipConditionMet_False_ProgType()
        {
            NewRule().TraineeshipConditionMet(1, 1).Should().BeFalse();
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
                        AchDateNullable = new DateTime(2019, 1, 1),
                        LearnActEndDateNullable = new DateTime(2016, 1, 1),
                        AimType = 1,
                        ProgTypeNullable = 24
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AchDate", "01/01/2016")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, 1, new DateTime(2017, 1, 1), new DateTime(2016, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private AchDate_10Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AchDate_10Rule(validationErrorHandler);
        }
    }
}
