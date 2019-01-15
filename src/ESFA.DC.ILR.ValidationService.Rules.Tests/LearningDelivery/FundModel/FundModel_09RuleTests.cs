using System;
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
    public class FundModel_09RuleTests : AbstractRuleTests<FundModel_09Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FundModel_09");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var aimType = 1;
            var fundModel = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var progType = 1;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ProgTypeConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(fundModel, progType)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, learnStartDate, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var aimType = 1;
            var fundModel = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var progType = 1;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(false);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ProgTypeConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(fundModel, progType)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, learnStartDate, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var aimType = 1;
            var fundModel = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var progType = 1;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(false);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ProgTypeConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(fundModel, progType)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, learnStartDate, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate()
        {
            var aimType = 1;
            var fundModel = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var progType = 1;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(false);
            ruleMock.Setup(r => r.ProgTypeConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(fundModel, progType)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, learnStartDate, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ProgType()
        {
            var aimType = 1;
            var fundModel = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var progType = 1;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ProgTypeConditionMet(progType)).Returns(false);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(fundModel, progType)).Returns(true);

            ruleMock.Object.ConditionMet(aimType, fundModel, learnStartDate, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Apprentiship()
        {
            var aimType = 1;
            var fundModel = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var progType = 1;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ProgTypeConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(fundModel, progType)).Returns(false);

            ruleMock.Object.ConditionMet(aimType, fundModel, learnStartDate, progType).Should().BeFalse();
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

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(81).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2016, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2018, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeConditionMet_True()
        {
            NewRule().ProgTypeConditionMet(25).Should().BeTrue();
        }

        [Fact]
        public void ProgTypeConditionMet_False()
        {
            NewRule().ProgTypeConditionMet(1).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipConditionMet_False()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ApprenticeshipConditionMet(99, progType).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipConditionMet_True_FundModel()
        {
            NewRule().ApprenticeshipConditionMet(1, null).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipConditionMet_True_DD()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).ApprenticeshipConditionMet(99, progType).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var progType = 25;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        ProgTypeNullable = progType,
                        FundModel = 10,
                        LearnStartDate = new DateTime(2016, 1, 1),
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

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

        private FundModel_09Rule NewRule(IDerivedData_07Rule dd07 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new FundModel_09Rule(dd07, validationErrorHandler);
        }
    }
}
