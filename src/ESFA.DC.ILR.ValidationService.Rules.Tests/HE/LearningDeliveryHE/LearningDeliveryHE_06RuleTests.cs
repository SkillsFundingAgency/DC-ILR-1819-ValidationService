using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.LearningDeliveryHE;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.LearningDeliveryHE
{
    public class LearningDeliveryHE_06RuleTests : AbstractRuleTests<LearningDeliveryHE_06Rule>
    {
        private readonly string[] _notionalNVQLevels =
            {
                LARSNotionalNVQLevelV2.Level4,
                LARSNotionalNVQLevelV2.Level5,
                LARSNotionalNVQLevelV2.Level6,
                LARSNotionalNVQLevelV2.Level7,
                LARSNotionalNVQLevelV2.Level8,
                LARSNotionalNVQLevelV2.HigherLevel
            };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearningDeliveryHE_06");
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.OtherAdult)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.EuropeanSocialFund)]
        [InlineData(TypeOfFunding.CommunityLearning)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_True()
        {
            NewRule().LearningDeliveryHEConditionMet(new TestLearningDeliveryHE() { SSN = "TEST1234" }).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False()
        {
            NewRule().LearningDeliveryHEConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LARSNotionalNVQLevelV2ConditionMet_False()
        {
            string learnAimRef = "50022246";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNVQLevels)).Returns(false);

            NewRule(lARSDataService: larsDataServiceMock.Object).LARSNotionalNVQLevelV2Exclusion(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LARSNotionalNVQLevelV2ConditionMet_True()
        {
            string learnAimRef = "50023408";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNVQLevels)).Returns(true);

            NewRule(lARSDataService: larsDataServiceMock.Object).LARSNotionalNVQLevelV2Exclusion(learnAimRef).Should().BeTrue();
        }

        [Theory]
        [InlineData(10)]
        [InlineData(70)]
        public void BuildErrorMessageParameters(int fundModel)
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel);

            validationErrorHandlerMock.Verify();
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.EuropeanSocialFund,
                        LearnAimRef = "50023408",
                        LearningDeliveryHEEntity =
                            new TestLearningDeliveryHE()
                            {
                                DOMICILE = "DOMICILE"
                            }
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels("50023408", _notionalNVQLevels)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    lARSDataService: larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.EuropeanSocialFund,
                        LearnAimRef = "50023408",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                DOMICILE = "AD"
                            }
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.CommunityLearning,
                        LearnAimRef = "50023409",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                DOMICILE = "AE"
                            }
                    },
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels("50023408", _notionalNVQLevels)).Returns(true);
            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels("50023409", _notionalNVQLevels)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    lARSDataService: larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        private LearningDeliveryHE_06Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILARSDataService lARSDataService = null)
        {
            return new LearningDeliveryHE_06Rule(validationErrorHandler: validationErrorHandler, lARSDataService: lARSDataService);
        }
    }
}
