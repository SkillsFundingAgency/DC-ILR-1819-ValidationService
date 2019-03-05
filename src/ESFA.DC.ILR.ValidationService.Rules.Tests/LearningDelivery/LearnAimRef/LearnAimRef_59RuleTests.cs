using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_59RuleTests : AbstractRuleTests<LearnAimRef_59Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_59");
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(99).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 24;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_True_Null()
        {
            int? progType = null;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var progType = 24;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            var learnStartDate = new DateTime(2015, 8, 1);

            NewRule().LearnStartDateConditionMet(learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            var learnStartDate = new DateTime(2014, 8, 1);

            NewRule().LearnStartDateConditionMet(learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void AgeConditionMet_True()
        {
            var dateOfBirth = new DateTime(1990, 8, 1);
            var learnStartDate = new DateTime(2015, 8, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object)
                .AgeConditionMet(dateOfBirth, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void AgeConditionMet_False()
        {
            var dateOfBirth = new DateTime(2000, 8, 1);
            var learnStartDate = new DateTime(2015, 8, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(15);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object)
                .AgeConditionMet(dateOfBirth, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void AgeConditionMet_False_NullOB()
        {
            var dateOfBirth = new DateTime(2000, 8, 1);
            var learnStartDate = new DateTime(2015, 8, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(15);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object)
                .AgeConditionMet(null, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "346"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "347"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "346")).Returns(true);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_famCode346()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "345"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "347"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "346")).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_famCode034()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "346"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(true);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_RES()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "346"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "RES",
                    LearnDelFAMCode = "347"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "346")).Returns(true);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LARSCategoryConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";
            var category = 19;

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, category)).Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object)
                .LARSCategoryConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LARSCategoryConditionMet_False()
        {
            var learnAimRef = "LearnAimRef";
            var category = 19;

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, category)).Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object)
                .LARSCategoryConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LARSNVQLevel2ConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";
            var level = "3";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, level)).Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object)
                .LARSNVQLevel2ConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LARSNVQLevel2ConditionMet_False()
        {
            var learnAimRef = "LearnAimRef";
            var level = "3";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, level)).Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object)
                .LARSNVQLevel2ConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void Level3QualificationConditionMet_True_LARSLevel3()
        {
            int? priorAttain = 20;
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2015, 8, 1);
            var percentValue = 100m;

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.EffectiveDatesValidforLearnAimRef(learnAimRef, learnStartDate)).Returns(true);
            larsDataServiceMock.Setup(ds => ds.FullLevel3PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, learnStartDate, percentValue)).Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object)
                .Level3QualificationConditionMet(priorAttain, learnAimRef, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void Level3QualificationConditionMet_True_PriorAttain()
        {
            int? priorAttain = 3;
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2015, 8, 1);
            var percentValue = 100m;

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.EffectiveDatesValidforLearnAimRef(learnAimRef, learnStartDate)).Returns(true);
            larsDataServiceMock.Setup(ds => ds.FullLevel3PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, learnStartDate, percentValue)).Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object)
                .Level3QualificationConditionMet(priorAttain, learnAimRef, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void Level3QualificationConditionMet_False_Level3Percent()
        {
            int? priorAttain = 20;
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2015, 8, 1);
            var percentValue = 100m;

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.EffectiveDatesValidforLearnAimRef(learnAimRef, learnStartDate)).Returns(true);
            larsDataServiceMock.Setup(ds => ds.FullLevel3PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, learnStartDate, percentValue)).Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object)
                .Level3QualificationConditionMet(priorAttain, learnAimRef, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void Level3QualificationConditionMet_False_LarsEffectiveDates()
        {
            int? priorAttain = 20;
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2015, 8, 1);
            var percentValue = 100m;

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.EffectiveDatesValidforLearnAimRef(learnAimRef, learnStartDate)).Returns(false);
            larsDataServiceMock.Setup(ds => ds.FullLevel3PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, learnStartDate, percentValue)).Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object)
                .Level3QualificationConditionMet(priorAttain, learnAimRef, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 1;
            var priorAttain = 1;
            var dateOfBirth = new DateTime(1990, 1, 1);
            var learnStartDate = new DateTime(2015, 8, 1);
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.DD07ConditionMet(priorAttain)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.AgeConditionMet(dateOfBirth, learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.LearningDeliveryFAMConditionMet(learningDeliveryFams)).Returns(true);
            ruleMock.Setup(r => r.LARSCategoryConditionMet(learnAimRef)).Returns(true);
            ruleMock.Setup(r => r.LARSNVQLevel2ConditionMet(learnAimRef)).Returns(true);
            ruleMock.Setup(r => r.Level3QualificationConditionMet(priorAttain, learnAimRef, learnStartDate)).Returns(true);

            ruleMock.Object.ConditionMet(fundModel, priorAttain, progType, learnAimRef, dateOfBirth, learnStartDate, learningDeliveryFams).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, true, true, true, true, true, true, true)]
        [InlineData(true, false, true, true, true, true, true, true)]
        [InlineData(true, true, false, true, true, true, true, true)]
        [InlineData(true, true, true, false, true, true, true, true)]
        [InlineData(true, true, true, true, false, true, true, true)]
        [InlineData(true, true, true, true, true, false, true, true)]
        [InlineData(true, true, true, true, true, true, false, true)]
        [InlineData(true, true, true, true, true, true, true, false)]
        public void ConditionMet_False(bool mock1, bool mock2, bool mock3, bool mock4, bool mock5, bool mock6, bool mock7, bool mock8)
        {
            var fundModel = 1;
            var priorAttain = 1;
            var dateOfBirth = new DateTime(1990, 1, 1);
            var learnStartDate = new DateTime(2015, 8, 1);
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(mock1);
            ruleMock.Setup(r => r.DD07ConditionMet(priorAttain)).Returns(mock2);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(mock3);
            ruleMock.Setup(r => r.AgeConditionMet(dateOfBirth, learnStartDate)).Returns(mock4);
            ruleMock.Setup(r => r.LearningDeliveryFAMConditionMet(learningDeliveryFams)).Returns(mock5);
            ruleMock.Setup(r => r.LARSCategoryConditionMet(learnAimRef)).Returns(mock6);
            ruleMock.Setup(r => r.LARSNVQLevel2ConditionMet(learnAimRef)).Returns(mock7);
            ruleMock.Setup(r => r.Level3QualificationConditionMet(priorAttain, learnAimRef, learnStartDate)).Returns(mock8);

            ruleMock.Object.ConditionMet(fundModel, priorAttain, progType, learnAimRef, dateOfBirth, learnStartDate, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var fundModel = 35;
            var priorAttain = 3;
            var dateOfBirth = new DateTime(1990, 1, 1);
            var learnStartDate = new DateTime(2015, 8, 1);
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                 new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "346"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "04"
                }
            };

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                PriorAttainNullable = priorAttain,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        FundModel = fundModel,
                        LearnAimRef = learnAimRef,
                        ProgTypeNullable = progType,
                        LearningDeliveryFAMs = learningDeliveryFams,
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);
            larsDataServiceMock.Setup(ds => ds.EffectiveDatesValidforLearnAimRef(learnAimRef, learnStartDate)).Returns(true);
            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, "3")).Returns(true);
            larsDataServiceMock.Setup(ds => ds.FullLevel3PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, learnStartDate, 100m)).Returns(false);
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "LDM", "346")).Returns(true);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    dd07Mock.Object,
                    dateTimeQueryServiceMock.Object,
                    larsDataServiceMock.Object,
                    learningDeliveryFamQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var fundModel = 35;
            var priorAttain = 3;
            var dateOfBirth = new DateTime(1990, 1, 1);
            var learnStartDate = new DateTime(2015, 8, 1);
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                 new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "346"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "RES",
                    LearnDelFAMCode = "04"
                }
            };

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                PriorAttainNullable = priorAttain,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        FundModel = fundModel,
                        LearnAimRef = learnAimRef,
                        ProgTypeNullable = progType,
                        LearningDeliveryFAMs = learningDeliveryFams,
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);
            larsDataServiceMock.Setup(ds => ds.EffectiveDatesValidforLearnAimRef(learnAimRef, learnStartDate)).Returns(true);
            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, "3")).Returns(true);
            larsDataServiceMock.Setup(ds => ds.FullLevel3PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, learnStartDate, 100m)).Returns(false);
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "LDM", "346")).Returns(true);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dd07Mock.Object,
                    dateTimeQueryServiceMock.Object,
                    larsDataServiceMock.Object,
                    learningDeliveryFamQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var learnAimRef = "LearnAimRef";
            var dateOfBirth = new DateTime(1990, 1, 1);
            var learnStartDate = new DateTime(2015, 8, 1);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", learnAimRef)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("DateOfBirth", "01/01/1990")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/08/2015")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnAimRef, dateOfBirth, learnStartDate);

            validationErrorHandlerMock.Verify();
        }

        private LearnAimRef_59Rule NewRule(
            IDerivedData_07Rule dd07 = null,
            IDateTimeQueryService dateTimeQueryService = null,
            ILARSDataService larsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnAimRef_59Rule(
                dd07,
                dateTimeQueryService,
                larsDataService,
                learningDeliveryFamQueryService,
                validationErrorHandler);
        }
    }
}
