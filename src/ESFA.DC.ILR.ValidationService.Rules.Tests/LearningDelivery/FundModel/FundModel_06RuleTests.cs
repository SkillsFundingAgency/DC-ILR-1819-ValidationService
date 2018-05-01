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
    public class FundModel_06RuleTests : AbstractRuleTests<FundModel_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FundModel_06");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).ConditionMet(36, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            NewRule().ConditionMet(10, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Apprentiship()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ConditionMet(36, progType).Should().BeFalse();
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
                        FundModel = 36,
                        ProgTypeNullable = progType
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

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
            var fundModel = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", fundModel)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel);

            validationErrorHandlerMock.Verify();
        }

        private FundModel_06Rule NewRule(IDD07 dd07 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new FundModel_06Rule(dd07, validationErrorHandler);
        }
    }
}
