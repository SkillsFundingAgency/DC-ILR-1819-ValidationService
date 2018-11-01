using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R91RuleTests : AbstractRuleTests<R91Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R91");
        }

        [Fact]
        public void ConditionMet_False()
        {
            var testLearningDeliveries = new[]
            {
                new TestLearningDelivery()
                {
                    FundModel = TypeOfFunding.EuropeanSocialFund,
                    LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment
                },
                new TestLearningDelivery()
                {
                    FundModel = TypeOfFunding.EuropeanSocialFund,
                    LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment
                }
            };

            NewRule().ConditionMet(testLearningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.EuropeanSocialFund)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, string.Empty)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.CompStatus, CompletionState.HasCompleted)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(TypeOfFunding.EuropeanSocialFund, string.Empty, CompletionState.HasCompleted);
            validationErrorHandlerMock.Verify();
        }

        public R91Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R91Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
