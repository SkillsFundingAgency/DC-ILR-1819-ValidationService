using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FworkCode;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.FworkCode
{
    public class FworkCode_05RuleTests : AbstractRuleTests<FworkCode_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FworkCode_05");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var aimType = 1;
            var learnAimRef = "LearnAimRef";
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            rule.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            rule.Setup(r => r.FworkCodeConditionMet(learnAimRef, progType, fworkCode, pwayCode)).Returns(true);

            rule.Object.ConditionMet(aimType, progType, learnAimRef, fworkCode, pwayCode).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var aimType = 1;
            var learnAimRef = "LearnAimRef";
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.AimTypeConditionMet(aimType)).Returns(false);
            rule.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            rule.Setup(r => r.FworkCodeConditionMet(learnAimRef, progType, fworkCode, pwayCode)).Returns(true);

            rule.Object.ConditionMet(aimType, progType, learnAimRef, fworkCode, pwayCode).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Apprenticeship()
        {
            var aimType = 1;
            var learnAimRef = "LearnAimRef";
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            rule.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(false);
            rule.Setup(r => r.FworkCodeConditionMet(learnAimRef, progType, fworkCode, pwayCode)).Returns(true);

            rule.Object.ConditionMet(aimType, progType, learnAimRef, fworkCode, pwayCode).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FworkCode()
        {
            var aimType = 1;
            var learnAimRef = "LearnAimRef";
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.AimTypeConditionMet(aimType)).Returns(true);
            rule.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            rule.Setup(r => r.FworkCodeConditionMet(learnAimRef, progType, fworkCode, pwayCode)).Returns(false);

            rule.Object.ConditionMet(aimType, progType, learnAimRef, fworkCode, pwayCode).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(3).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(1).Should().BeFalse();
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
        public void ApprenticeshipConditionMet_False_ProgType()
        {
            NewRule().ApprenticeshipConditionMet(25).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipConditionMet_False_DD()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.Derive(progType)).Returns("N");

            NewRule(dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void FworkCodeConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.FrameworkCodeExists(learnAimRef, progType, fworkCode, pwayCode)).Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object).FworkCodeConditionMet(learnAimRef, progType, fworkCode, pwayCode).Should().BeTrue();
        }

        [Fact]
        public void FworkCodeConditionMet_False()
        {
            var learnAimRef = "LearnAimRef";
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.FrameworkCodeExists(learnAimRef, progType, fworkCode, pwayCode)).Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object).FworkCodeConditionMet(learnAimRef, progType, fworkCode, pwayCode).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var aimType = 3;
            var learnAimRef = "LearnAimRef";
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.Derive(progType)).Returns("Y");

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.FrameworkCodeExists(learnAimRef, progType, fworkCode, pwayCode)).Returns(false);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = aimType,
                        LearnAimRef = learnAimRef,
                        ProgTypeNullable = progType,
                        FworkCodeNullable = fworkCode,
                        PwayCodeNullable = pwayCode,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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
                        AimType = 1
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
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FworkCode", fworkCode)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PwayCode", pwayCode)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(progType, fworkCode, pwayCode);

            validationErrorHandlerMock.Verify();
        }

        private FworkCode_05Rule NewRule(IDD07 dd07 = null, ILARSDataService larsDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new FworkCode_05Rule(dd07, larsDataService, validationErrorHandler);
        }
    }
}
