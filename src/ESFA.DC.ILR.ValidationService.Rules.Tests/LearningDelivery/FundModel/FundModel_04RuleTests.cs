using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.FundModel
{
    public class FundModel_04RuleTests : AbstractRuleTests<FundModel_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FundModel_04");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = 1;
            var fundModel = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            rule.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);

            rule.Object.ConditionMet(fundModel, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var progType = 1;
            var fundModel = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            rule.Setup(r => r.FundModelConditionMet(fundModel)).Returns(false);

            rule.Object.ConditionMet(fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Apprenticeship()
        {
            var progType = 1;
            var fundModel = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(false);
            rule.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);

            rule.Object.ConditionMet(fundModel, progType).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(1).Should().BeFalse();
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
        public void ApprenticeshipConditionMet_False()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.Derive(progType)).Returns("N");

            NewRule(dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var progType = 1;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                        ProgTypeNullable = progType,
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.Derive(progType)).Returns("Y");

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
                        FundModel = 10,
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private FundModel_04Rule NewRule(IDD07 dd07 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new FundModel_04Rule(dd07, validationErrorHandler);
        }
    }
}
