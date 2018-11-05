using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_14RuleTests : AbstractRuleTests<LearnDelFAMType_14Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_14");
        }

        [Theory]
        [InlineData(FundModelConstants.AdultSkills)]
        [InlineData(FundModelConstants.Apprenticeships)]
        [InlineData(FundModelConstants.OtherAdult)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(FundModelConstants.SixteenToNineteen).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ADL)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.EEF
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.EEF)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(TypeOfLearningProgramme.ApprenticeshipStandard)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(TypeOfLearningProgramme.ApprenticeshipStandard).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(null)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(null).Should().BeTrue();
        }

        [Theory]
        [InlineData(FundModelConstants.AdultSkills, false, true)]
        [InlineData(FundModelConstants.Apprenticeships, true, false)]
        [InlineData(FundModelConstants.OtherAdult, false, false)]
        [InlineData(FundModelConstants.NonFunded, false, false)]
        public void ConditionMet_False(int fundModel, bool dd07Result, bool famsResult)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDD07>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ADL)).Returns(famsResult);
            dd07Mock.Setup(dd => dd.IsApprenticeship(TypeOfLearningProgramme.ApprenticeshipStandard)).Returns(dd07Result);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object, dd07: dd07Mock.Object).ConditionMet(fundModel, TypeOfLearningProgramme.ApprenticeshipStandard, testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(FundModelConstants.AdultSkills, true, true)]
        [InlineData(FundModelConstants.NonFunded, true, true)]
        [InlineData(FundModelConstants.NonFunded, false, true)]
        public void ConditionMet_True(int fundModel, bool dd07Result, bool famsResult)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.EEF
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDD07>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.EEF)).Returns(famsResult);
            dd07Mock.Setup(dd => dd.IsApprenticeship(null)).Returns(dd07Result);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object, dd07: dd07Mock.Object).ConditionMet(fundModel, TypeOfLearningProgramme.ApprenticeshipStandard, testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.EEF
                }
            };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModelConstants.NonFunded,
                        ProgTypeNullable = null,
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDD07>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.EEF)).Returns(true);
            dd07Mock.Setup(dd => dd.IsApprenticeship(null)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object, dd07: dd07Mock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT
                }
            };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModelConstants.Apprenticeships,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDD07>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(false);
            dd07Mock.Setup(dd => dd.IsApprenticeship(TypeOfLearningProgramme.AdvancedLevelApprenticeship)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object, dd07: dd07Mock.Object).Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(FundModelConstants.Apprenticeships)]
        [InlineData(FundModelConstants.AdultSkills)]
        [InlineData(FundModelConstants.OtherAdult)]
        public void BuildErrorMessageParameters(int fundModel)
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.EEF)).Verifiable();
            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel, LearningDeliveryFAMTypeConstants.EEF);

            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_14Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IDD07 dd07 = null)
        {
            return new LearnDelFAMType_14Rule(validationErrorHandler: validationErrorHandler, learningDeliveryFAMQueryService: learningDeliveryFAMQueryService, dd07: dd07);
        }
    }
}
