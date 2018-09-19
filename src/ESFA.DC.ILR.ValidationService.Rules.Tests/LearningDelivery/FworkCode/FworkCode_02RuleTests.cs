using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FworkCode;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.FworkCode
{
    public class FworkCode_02RuleTests : AbstractRuleTests<FworkCode_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FworkCode_02");
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

        [Theory]
        [InlineData(null)]
        [InlineData(24)]
        [InlineData(25)]
        public void ApprenticeshipConditionMet_True(int? progType)
        {
            NewRule().ApprenticeshipConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipConditionMet_False_ProgType()
        {
            NewRule().ApprenticeshipConditionMet(1).Should().BeFalse();
        }

        [Fact]
        public void FworkCodeConditionMet_True()
        {
            NewRule().FworkCodeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void FworkCodeConditionMet_False()
        {
            NewRule().FworkCodeConditionMet(null).Should().BeFalse();
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
                        FworkCodeNullable = 1
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
            var fworkCode = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FworkCode", fworkCode)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(fworkCode);

            validationErrorHandlerMock.Verify();
        }

        private FworkCode_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new FworkCode_02Rule(validationErrorHandler);
        }
    }
}
