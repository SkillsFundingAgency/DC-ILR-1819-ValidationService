using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_46RuleTests : AbstractRuleTests<LearnDelFAMType_46Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_46");
        }

        [Theory]
        [InlineData(TypeOfFunding.NotFundedByESFA, true)]
        [InlineData(TypeOfFunding.CommunityLearning, true)]
        [InlineData(TypeOfFunding.AdultSkills, false)]
        public void FundModelConditionMetMeetsExpectation(int fundModel, bool expectation)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(expectation);
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ADL, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.EEF, false)]
        [InlineData("ABC", false)]
        [InlineData(null, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.FLN, true)]
        [InlineData("fln", true)]
        public void FAMTypeConditionMetMeetsExpectation(string learnDelFamType, bool expectation)
        {
            NewRule().FAMTypeConditionMet(learnDelFamType).Should().Be(expectation);
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.FLN)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearningDeliveryFAMTypeConstants.FLN);

            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_46Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_46Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
