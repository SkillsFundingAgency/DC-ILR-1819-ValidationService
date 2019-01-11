using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_08RuleTests : AbstractRuleTests<EmpStat_08Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpStat_08");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(TypeOfFunding.ApprenticeshipsFrom1May2017).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.OtherAdult)]
        [InlineData(TypeOfFunding.NotFundedByESFA)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, "2012-01-01")]
        [InlineData("1994-01-01", "2012-01-01")]
        public void LearningDeliveryConditionMet_False(string dateOfBirthString, string learnStartDateString)
        {
            DateTime? dateOfBirth = string.IsNullOrEmpty(dateOfBirthString) ? (DateTime?)null : DateTime.Parse(dateOfBirthString);
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearQueryService = new Mock<IAcademicYearQueryService>();

            academicYearQueryService.Setup(dd => dd.AugustThirtyFirstOfLearnStartDate(learnStartDate)).Returns(new DateTime(2012, 08, 31));
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth ?? new DateTime(1994, 08, 31), new DateTime(2012, 08, 31))).Returns(18);

            NewRule(
                academicYearQueryService: academicYearQueryService.Object,
                dateTimeQueryService: dateTimeQueryServiceMock.Object).LearningDeliveryConditionMet(dateOfBirth, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryConditionMet_True()
        {
            DateTime dateOfBirth = new DateTime(1997, 08, 31);
            DateTime learnStartDate = new DateTime(2017, 08, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            academicYearQueryServiceMock.Setup(dd => dd.AugustThirtyFirstOfLearnStartDate(learnStartDate)).Returns(new DateTime(2017, 08, 31));
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);

            NewRule(
                academicYearQueryService: academicYearQueryServiceMock.Object,
                dateTimeQueryService: dateTimeQueryServiceMock.Object).LearningDeliveryConditionMet(dateOfBirth, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void EmploymentStatusConditionMet_False()
        {
            var learnStartDate = new DateTime(2017, 8, 31);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(false);

            NewRule(learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object).EmploymentStatusConditionMet(learnerEmploymentStatuses, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void EmploymentStatusConditionMet_False_Null()
        {
            var learnStartDate = new DateTime(2017, 8, 31);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(false);

            NewRule(learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object).EmploymentStatusConditionMet(null, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void EmploymentStatusConditionMet_True()
        {
            var learnStartDate = new DateTime(2017, 8, 31);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2017, 7, 1)
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(true);

            NewRule(learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object).EmploymentStatusConditionMet(learnerEmploymentStatuses, learnStartDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(24)]
        [InlineData(2)]
        public void DD07ConditionMet_False(int? progType)
        {
            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(4)]
        public void DD07ConditionMet_True(int? progType)
        {
            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(8, LearningDeliveryFAMTypeConstants.LDM, "034")]
        [InlineData(12, LearningDeliveryFAMTypeConstants.SOF, "108")]
        public void LearningDeliveryFAMsConditionMet_False(int fundModel, string learnDelFAMType, string learnDelFAMCode)
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = learnDelFAMType, LearnDelFAMCode = learnDelFAMCode },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT, LearnDelFAMCode = "44" }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, learnDelFAMType, learnDelFAMCode)).Returns(true);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT, "44")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(fundModel, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            int fundModel = 99;
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT, LearnDelFAMCode = "44" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL, LearnDelFAMCode = "2" }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT, "44")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(fundModel, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True_Null()
        {
            int fundModel = 99;
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(fundModel, null).Should().BeTrue();
        }

        [Theory]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, null, "2016-12-01", 2, LearningDeliveryFAMTypeConstants.SOF, "108")]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, "2002-05-01", "2016-12-01", 2, LearningDeliveryFAMTypeConstants.LDM, "034")]
        public void ConditionMet_False(
           int fundModel,
           string dateOfBirthString,
           string learnStartDateString,
           int? progType,
           string learnDelFAMType,
           string learnDelFAMCode)
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = learnDelFAMType, LearnDelFAMCode = learnDelFAMCode },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES, LearnDelFAMCode = "15" }
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2017, 7, 1)
                }
            };

            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? dateOfBirth = string.IsNullOrEmpty(dateOfBirthString) ? (DateTime?)null : DateTime.Parse(dateOfBirthString);

            var dd07Mock = new Mock<IDD07>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            academicYearQueryServiceMock.Setup(dd => dd.AugustThirtyFirstOfLearnStartDate(learnStartDate)).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth ?? new DateTime(2000, 06, 01), learnStartDate)).Returns(18);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, learnDelFAMType, learnDelFAMCode)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(false);

            NewRule(
                dd07: dd07Mock.Object,
                dateTimeQueryService: dateTimeQueryServiceMock.Object,
                academicYearQueryService: academicYearQueryServiceMock.Object,
                learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(fundModel, dateOfBirth, learnStartDate, progType, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, "1997-08-31", "2017-08-31", 4, LearningDeliveryFAMTypeConstants.ACT, "022")]
        [InlineData(TypeOfFunding.OtherAdult, "1997-08-31", "2017-08-31", 5, LearningDeliveryFAMTypeConstants.ADL, "022")]
        [InlineData(TypeOfFunding.NotFundedByESFA, "1997-08-31", "2017-08-31", null, LearningDeliveryFAMTypeConstants.RES, "022")]
        public void ConditionMet_True(
           int fundModel,
           string dateOfBirthString,
           string learnStartDateString,
           int? progType,
           string learnDelFAMType,
           string learnDelFAMCode)
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = learnDelFAMType, LearnDelFAMCode = learnDelFAMCode },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL, LearnDelFAMCode = "031" }
            };
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2017, 7, 1)
                }
            };

            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? dateOfBirth = string.IsNullOrEmpty(dateOfBirthString) ? (DateTime?)null : DateTime.Parse(dateOfBirthString);

            var dd07Mock = new Mock<IDD07>();
            var academicYearQueryService = new Mock<IAcademicYearQueryService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            academicYearQueryService.Setup(dd => dd.AugustThirtyFirstOfLearnStartDate(learnStartDate)).Returns(new DateTime(2017, 08, 31));
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth ?? new DateTime(1997, 08, 31), learnStartDate)).Returns(20);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(false);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(true);

            NewRule(
                dd07: dd07Mock.Object,
                academicYearQueryService: academicYearQueryService.Object,
                dateTimeQueryService: dateTimeQueryServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object).ConditionMet(fundModel, dateOfBirth, learnStartDate, progType, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            DateTime dateOfBirth = new DateTime(1997, 08, 31);
            DateTime learnStartDate = new DateTime(2017, 08, 31);

            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT, LearnDelFAMCode = "44" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL, LearnDelFAMCode = "031" }
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2017, 7, 1)
                }
            };

            ILearner learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = learnStartDate,
                        FundModel = TypeOfFunding.AdultSkills,
                        ProgTypeNullable = 4,
                        LearningDeliveryFAMs = learningDeliveryFAMs.ToList()
                    }
                },
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            var dd07Mock = new Mock<IDD07>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(4)).Returns(false);
            academicYearQueryServiceMock.Setup(dd => dd.AugustThirtyFirstOfLearnStartDate(learnStartDate)).Returns(new DateTime(2017, 08, 31));
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(false);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    dd07: dd07Mock.Object,
                    academicYearQueryService: academicYearQueryServiceMock.Object,
                    dateTimeQueryService: dateTimeQueryServiceMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                    learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            DateTime dateOfBirth = new DateTime(1997, 08, 31);
            DateTime learnStartDate = new DateTime(2015, 08, 31);

            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES }
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2017, 7, 1)
                }
            };

            ILearner learner = new TestLearner()
            {
                DateOfBirthNullable = new DateTime(1996, 01, 01),
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2016, 06, 01),
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = 23,
                        LearningDeliveryFAMs = learningDeliveryFAMs.ToList()
                    }
                },
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            var dd07Mock = new Mock<IDD07>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(23)).Returns(false);
            academicYearQueryServiceMock.Setup(dd => dd.AugustThirtyFirstOfLearnStartDate(learnStartDate)).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034")).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dd07: dd07Mock.Object,
                    dateTimeQueryService: dateTimeQueryServiceMock.Object,
                    academicYearQueryService: academicYearQueryServiceMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                    learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/08/2014")).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.AdultSkills)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2014, 08, 01), TypeOfFunding.AdultSkills);

            validationErrorHandlerMock.Verify();
        }

        public EmpStat_08Rule NewRule(
            IDD07 dd07 = null,
            IDateTimeQueryService dateTimeQueryService = null,
            IAcademicYearQueryService academicYearQueryService = null,
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpStat_08Rule(
                dd07: dd07,
                dateTimeQueryService: dateTimeQueryService,
                academicYearQueryService: academicYearQueryService,
                learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryService,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService,
                validationErrorHandler: validationErrorHandler);
        }
    }
}
