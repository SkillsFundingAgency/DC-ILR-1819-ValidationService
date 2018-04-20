using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.FundModel
{
    public class FundModel_07RuleTests : AbstractRuleTests<FundModel_07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FundModel_07");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var aimType = 1;
            var fundModel = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var progType = 1;
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.LearningDeliveryFAMConditionMet(learningDeliveryFams)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, learnStartDate, progType, learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var aimType = 1;
            var fundModel = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var progType = 1;
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(false);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.LearningDeliveryFAMConditionMet(learningDeliveryFams)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, learnStartDate, progType, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var aimType = 1;
            var fundModel = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var progType = 1;
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(false);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.LearningDeliveryFAMConditionMet(learningDeliveryFams)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, learnStartDate, progType, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate()
        {
            var aimType = 1;
            var fundModel = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var progType = 1;
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(false);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.LearningDeliveryFAMConditionMet(learningDeliveryFams)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, learnStartDate, progType, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Apprentiship()
        {
            var aimType = 1;
            var fundModel = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var progType = 1;
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(false);
            ruleMock.Setup(r => r.LearningDeliveryFAMConditionMet(learningDeliveryFams)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, learnStartDate, progType, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearningDeliveryFAM()
        {
            var aimType = 1;
            var fundModel = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var progType = 1;
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.LearningDeliveryFAMConditionMet(learningDeliveryFams)).Returns(false);

            ruleMock.Object.ConditionMet(aimType, fundModel, learnStartDate, progType, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(0).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(1).Should().BeTrue();
        }

        [Theory]
        [InlineData(36)]
        [InlineData(99)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2018, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2016, 1, 1)).Should().BeFalse();
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
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES")).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFams, "LDM", It.IsAny<IEnumerable<string>>())).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_RES()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES")).Returns(true);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFams, "LDM", It.IsAny<IEnumerable<string>>())).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_LDM()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES")).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFams, "LDM", It.IsAny<IEnumerable<string>>())).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var progType = 1;
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>();

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        ProgTypeNullable = progType,
                        FundModel = 10,
                        LearnStartDate = new DateTime(2018, 1, 1),
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.Derive(progType)).Returns("Y");

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES")).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFams, "LDM", It.IsAny<IEnumerable<string>>())).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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
                        AimType = 0,
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

        private FundModel_07Rule NewRule(IDD07 dd07 = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new FundModel_07Rule(dd07, learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
