using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
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
    public class LearnDelFAMType_53RuleTests : AbstractRuleTests<LearnDelFAMType_53Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_53");
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ALB, "1", false, true)]
        [InlineData(LearningDeliveryFAMTypeConstants.ALB, "3", false, true)]
        [InlineData(LearningDeliveryFAMTypeConstants.ALB, "1", true, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.ALB, "3", true, false)]
        public void ConditionMet_False(string learDelFamType, string famCode, bool hasLearningDeliveryFamCodesForType, bool hasFundingStreamPeriodCode)
        {
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = learDelFamType,
                LearnDelFAMCode = famCode
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>() { testLearningDeliveryFam };

            var learningDeliveryFaMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            learningDeliveryFaMsQueryServiceMock.Setup(s => s.HasLearningDeliveryFAMCodeForType(It.IsAny<List<ILearningDeliveryFAM>>(), learDelFamType, famCode)).Returns(hasLearningDeliveryFamCodesForType);

            fcsDataServiceMock.Setup(x => x.FundingRelationshipFCTExists(It.IsAny<List<string>>())).Returns(hasFundingStreamPeriodCode);

            NewRule(fcsDataServiceMock.Object, learningDeliveryFaMsQueryServiceMock.Object)
                .ConditionMet(learningDeliveryFams)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ALB, "1", true, true)]
        [InlineData(LearningDeliveryFAMTypeConstants.ALB, "3", true, true)]
        public void ConditionMet_True(string learDelFamType, string famCode, bool hasLearningDeliveryFamCodesForType, bool hasFundingStreamPeriodCode)
        {
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = learDelFamType,
                LearnDelFAMCode = famCode
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>() { testLearningDeliveryFam };

            var learningDeliveryFaMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            learningDeliveryFaMsQueryServiceMock.Setup(s => s.HasLearningDeliveryFAMCodeForType(It.IsAny<List<ILearningDeliveryFAM>>(), learDelFamType, famCode)).Returns(hasLearningDeliveryFamCodesForType);

            fcsDataServiceMock.Setup(x => x.FundingRelationshipFCTExists(It.IsAny<List<string>>())).Returns(hasFundingStreamPeriodCode);

            NewRule(fcsDataServiceMock.Object, learningDeliveryFaMsQueryServiceMock.Object)
                .ConditionMet(learningDeliveryFams)
                .Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NullCheck()
        {
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(d => d.GetLearningDeliveryFAMsCountByFAMType(null, LearningDeliveryFAMTypeConstants.HHS)).Returns(0);
            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.HHS)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearningDeliveryFAMTypeConstants.HHS);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMType_53Rule NewRule(
            IFCSDataService fcsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_53Rule(fcsDataService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
