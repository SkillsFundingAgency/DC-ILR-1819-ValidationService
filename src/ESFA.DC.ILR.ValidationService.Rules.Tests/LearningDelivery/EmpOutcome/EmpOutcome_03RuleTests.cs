using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EmpOutcome;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.EmpOutcome
{
    public class EmpOutcome_03RuleTests : AbstractRuleTests<EmpOutcome_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpOutcome_03");
        }

        [Fact]
        public void ConditionMet_True()
        {
            int? empOutcome = 1;
            int? progType = 1;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.EmpOutcomeConditionMet(empOutcome)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);

            ruleMock.Object.ConditionMet(empOutcome, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Apprenticeship()
        {
            var empOutcome = 1;
            var progType = 1;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.EmpOutcomeConditionMet(empOutcome)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(false);

            ruleMock.Object.ConditionMet(empOutcome, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_EmpOutcome()
        {
            var empOutcome = 1;
            var progType = 1;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.EmpOutcomeConditionMet(empOutcome)).Returns(false);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);

            ruleMock.Object.ConditionMet(empOutcome, progType).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipConditionMet_True()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipConditionMet_False()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void EmpOutcomeConditionMet_True()
        {
            NewRule().EmpOutcomeConditionMet(1).Should().BeTrue();
        }

        public void EmpOutcomeConditionMet_False()
        {
            NewRule().EmpOutcomeConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            int? progType = 1;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EmpOutcomeNullable = 1,
                        ProgTypeNullable = progType,
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
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
                        EmpOutcomeNullable = null
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("EmpOutcome", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private EmpOutcome_03Rule NewRule(IDD07 dd07 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpOutcome_03Rule(dd07, validationErrorHandler);
        }
    }
}
