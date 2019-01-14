﻿using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_40RuleTests : AbstractRuleTests<LearnDelFAMType_40Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_40");
        }

        [Theory]
        [InlineData(TypeOfFunding.NotFundedByESFA, true)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.AdultSkills, false)]
        public void FundModelConditionMetMeetsExpectation(int fundModel, bool expectation)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(expectation);
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ADL, true)]
        [InlineData("adL", true)]
        [InlineData(LearningDeliveryFAMTypeConstants.EEF, false)]
        [InlineData("ABC", false)]
        [InlineData(null, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.FLN, false)]
        public void FAMTypeConditionMetMeetsExpectation(string learnDelFamType, bool expectation)
        {
            NewRule().FAMTypeConditionMet(learnDelFamType).Should().Be(expectation);
        }

        [Theory]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, true, true)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, true, true)]
        [InlineData(TypeOfLearningProgramme.Traineeship, false, false)]
        public void DD07ConditionMetMeetsExpectation(int progType, bool isApprenticeship, bool expectation)
        {
            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(isApprenticeship);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().Be(expectation);
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = TypeOfFunding.NotFundedByESFA;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var learnDelFamType = LearningDeliveryFAMTypeConstants.ADL;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ConditionMet(fundModel, progType, learnDelFamType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_InvalidFundModel()
        {
            var fundModel = TypeOfFunding.AdultSkills;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var learnDelFamType = LearningDeliveryFAMTypeConstants.ADL;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ConditionMet(fundModel, progType, learnDelFamType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_InvalidLearnDelFamType()
        {
            var fundModel = TypeOfFunding.NotFundedByESFA;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var learnDelFamType = LearningDeliveryFAMTypeConstants.ALB;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ConditionMet(fundModel, progType, learnDelFamType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DD07RuleReturnsFalse()
        {
            var fundModel = TypeOfFunding.NotFundedByESFA;
            var progType = TypeOfLearningProgramme.Traineeship;
            var learnDelFamType = LearningDeliveryFAMTypeConstants.ADL;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).ConditionMet(fundModel, progType, learnDelFamType).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var fundModel = TypeOfFunding.NotFundedByESFA;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var learnDelFamType = LearningDeliveryFAMTypeConstants.ADL;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = learnDelFamType
                            }
                        }
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var fundModel = TypeOfFunding.AdultSkills;
            var progType = TypeOfLearningProgramme.Traineeship;
            var learnDelFamType = LearningDeliveryFAMTypeConstants.ALB;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = learnDelFamType
                            }
                        }
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.NotFundedByESFA)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ADL)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(TypeOfFunding.NotFundedByESFA, LearningDeliveryFAMTypeConstants.ADL);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMType_40Rule NewRule(
            IDD07 dd07 = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_40Rule(dd07, validationErrorHandler);
        }
    }
}
