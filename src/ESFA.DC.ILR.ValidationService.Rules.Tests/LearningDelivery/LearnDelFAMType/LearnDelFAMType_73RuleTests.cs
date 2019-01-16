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
    public class LearnDelFAMType_73RuleTests : AbstractRuleTests<LearnDelFAMType_73Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_73");
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.CommunityLearning, true)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        public void FundModelConditionMetMeetsExpectation(int fundModel, bool expectation)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(expectation);
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(
                testLearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LSF_Code
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(
                testLearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.ACT,
                LearningDeliveryFAMCodeConstants.LSF_Code)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.OtherAdult, false, false)]
        [InlineData(TypeOfFunding.OtherAdult, true, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, true, false)]
        [InlineData(TypeOfFunding.AdultSkills, true, true)]
        [InlineData(TypeOfFunding.CommunityLearning, true, true)]
        [InlineData(TypeOfFunding.CommunityLearning, false, false)]
        public void ConditionMetMeetsExpectation(int fundModel, bool famQueryServiceResult, bool expectation)
        {
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(),
                It.IsAny<string>(),
                It.IsAny<string>())).Returns(famQueryServiceResult);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(
                fundModel,
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>()).Should().Be(expectation);
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.FLN,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.AdultSkills
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(
                learningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.FLN,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.AdultSkills
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(
                learningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_WithNoLearnerFAMS_Returns_NoError()
        {
            var learner = new TestLearner();
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.AdultSkills)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(TypeOfFunding.AdultSkills, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot);

            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_73Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new LearnDelFAMType_73Rule(validationErrorHandler: validationErrorHandler, learningDeliveryFamQueryService: learningDeliveryFAMQueryService);
        }
    }
}
