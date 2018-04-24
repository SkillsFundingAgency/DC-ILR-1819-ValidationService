using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FworkCode;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.FworkCode
{
    public class FworkCode_01RuleTests : AbstractRuleTests<FworkCode_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FworkCode_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = 1;
            var fworkCode = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            rule.Setup(r => r.FworkCodeConditionMet(fworkCode)).Returns(true);

            rule.Object.ConditionMet(fworkCode, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Apprenticeship()
        {
            var progType = 1;
            var fworkCode = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(false);
            rule.Setup(r => r.FworkCodeConditionMet(fworkCode)).Returns(true);

            rule.Object.ConditionMet(fworkCode, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FworkCode()
        {
            var progType = 1;
            var fworkCode = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            rule.Setup(r => r.FworkCodeConditionMet(fworkCode)).Returns(false);

            rule.Object.ConditionMet(fworkCode, progType).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipConditionMet_True()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.Derive(progType)).Returns("Y");

            NewRule(dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipConditionMet_False_ProgType()
        {
            NewRule().ApprenticeshipConditionMet(25).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipConditionMet_False_DD()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.Derive(progType)).Returns("N");

            NewRule(dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void FworkCodeConditionMet_True()
        {
            NewRule().FworkCodeConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void FworkCodeConditionMet_False()
        {
            NewRule().FworkCodeConditionMet(1).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.Derive(progType)).Returns("Y");

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType
                    }
                }
            };

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
                        FworkCodeNullable = 1
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
            var progType = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(progType);

            validationErrorHandlerMock.Verify();
        }

        private FworkCode_01Rule NewRule(IDD07 dd07 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new FworkCode_01Rule(dd07, validationErrorHandler);
        }
    }
}
