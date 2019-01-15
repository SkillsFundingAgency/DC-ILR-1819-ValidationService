using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_07RuleTests : AbstractRuleTests<LearnStartDate_07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnStartDate_07");
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 2;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock
                .Setup(dm => dm.IsApprenticeship(progType))
                .Returns(true);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var progType = 0;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock
                .Setup(dm => dm.IsApprenticeship(progType))
                .Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            var aimType = 3;

            NewRule().AimTypeConditionMet(aimType).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            var aimType = 1;

            NewRule().AimTypeConditionMet(aimType).Should().BeFalse();
        }

        [Fact]
        public void FrameworkAimsConditionMet_True()
        {
            var dd04Date = new DateTime(2018, 10, 01);
            var learnAimRef = "learnAimRef";
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode))
                .Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object)
                .FrameworkAimsConditionMet(dd04Date, learnAimRef, progType, fworkCode, pwayCode)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void FrameworkAimsConditionMet_False()
        {
            var dd04Date = new DateTime(2018, 10, 01);
            var learnAimRef = "learnAimRef";
            var progType = 0;
            var fworkCode = 0;
            var pwayCode = 0;

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode))
                .Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object)
                .FrameworkAimsConditionMet(dd04Date, learnAimRef, progType, fworkCode, pwayCode)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Excluded_TrueProgType()
        {
            var progType = 25;
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>();

            NewRule().Excluded(progType, learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void Excluded_TrueRestart()
        {
            var progType = 1;
            var famType = "RES";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, famType))
                .Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .Excluded(progType, learningDeliveryFams)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Excluded_False()
        {
            var progType = 1;
            var famType = "XXX";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, famType))
                .Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .Excluded(progType, learningDeliveryFams)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var aimType = 3;
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var dd04Date = new DateTime(2018, 10, 01);
            var fworkCode = 1;
            var pwayCode = 1;
            var famType = "XXX";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode))
                .Returns(true);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, famType))
                .Returns(false);

            NewRule(dd07: dd07Mock.Object, larsDataService: larsDataServiceMock.Object, learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(aimType, progType, learnAimRef, dd04Date, fworkCode, pwayCode, learningDeliveryFams)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseExcluded()
        {
            var aimType = 3;
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var dd04Date = new DateTime(2018, 10, 01);
            var fworkCode = 1;
            var pwayCode = 1;
            var famType = "RES";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode))
                .Returns(true);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, famType))
                .Returns(true);

            NewRule(dd07: dd07Mock.Object, larsDataService: larsDataServiceMock.Object, learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(aimType, progType, learnAimRef, dd04Date, fworkCode, pwayCode, learningDeliveryFams)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseAimType()
        {
            var aimType = 5;
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var dd04Date = new DateTime(2018, 10, 01);
            var fworkCode = 1;
            var pwayCode = 1;
            var famType = "XXX";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode))
                .Returns(true);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, famType))
                .Returns(false);

            NewRule(dd07: dd07Mock.Object, larsDataService: larsDataServiceMock.Object, learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(aimType, progType, learnAimRef, dd04Date, fworkCode, pwayCode, learningDeliveryFams)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseDD07()
        {
            var aimType = 3;
            var progType = 0;
            var learnAimRef = "LearnAimRef";
            var dd04Date = new DateTime(2018, 10, 01);
            var fworkCode = 1;
            var pwayCode = 1;
            var famType = "XXX";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode))
                .Returns(true);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, famType))
                .Returns(false);

            NewRule(dd07: dd07Mock.Object, larsDataService: larsDataServiceMock.Object, learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(aimType, progType, learnAimRef, dd04Date, fworkCode, pwayCode, learningDeliveryFams)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseFrameworkAims()
        {
            var aimType = 3;
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var dd04Date = new DateTime(2018, 10, 01);
            var fworkCode = 1;
            var pwayCode = 1;
            var famType = "XXX";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode))
                .Returns(false);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, famType))
                .Returns(false);

            NewRule(dd07: dd07Mock.Object, larsDataService: larsDataServiceMock.Object, learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(aimType, progType, learnAimRef, dd04Date, fworkCode, pwayCode, learningDeliveryFams)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var aimType = 3;
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var dd04Date = new DateTime(2018, 10, 01);
            var fworkCode = 1;
            var pwayCode = 1;
            var famType = "XXX";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        LearnAimRef = learnAimRef,
                        FworkCodeNullable = fworkCode,
                        PwayCodeNullable = pwayCode,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = famType
                            }
                        }
                    }
                }
            };

            var learningDeliveryFams = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var dd04Mock = new Mock<IDerivedData_04Rule>();
            dd04Mock.Setup(dm => dm.Derive(It.IsAny<IEnumerable<ILearningDelivery>>(), It.IsAny<ILearningDelivery>())).Returns(dd04Date);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode))
                .Returns(true);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, famType))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, dd04Mock.Object, larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var aimType = 3;
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var dd04Date = new DateTime(2018, 10, 01);
            var fworkCode = 1;
            var pwayCode = 1;
            var famType = "RES";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        LearnAimRef = learnAimRef,
                        FworkCodeNullable = fworkCode,
                        PwayCodeNullable = pwayCode,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = famType
                            }
                        }
                    }
                }
            };

            var learningDeliveryFams = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var dd04Mock = new Mock<IDerivedData_04Rule>();
            dd04Mock.Setup(dm => dm.Derive(It.IsAny<IEnumerable<ILearningDelivery>>(), It.IsAny<ILearningDelivery>())).Returns(dd04Date);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode))
                .Returns(true);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, famType))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, dd04Mock.Object, larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var pwayCode = 1;
            var progType = 1;
            var fworkCode = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/10/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PwayCode, pwayCode)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FworkCode, fworkCode)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 10, 01), pwayCode, progType, fworkCode);

            validationErrorHandlerMock.Verify();
        }

        private LearnStartDate_07Rule NewRule(
            IDerivedData_07Rule dd07 = null,
            IDerivedData_04Rule dd04 = null,
            ILARSDataService larsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnStartDate_07Rule(dd07, dd04, larsDataService, learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
