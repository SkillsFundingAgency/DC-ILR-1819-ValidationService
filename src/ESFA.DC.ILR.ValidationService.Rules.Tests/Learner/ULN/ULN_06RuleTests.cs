using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class ULN_06RuleTests : AbstractRuleTests<ULN_06Rule>
    {
        [Fact]
        public void UlnConditionMet_True()
        {
            NewRule().UlnConditionMet(9999999999).Should().BeTrue();
        }

        [Fact]
        public void UlnConditionMet_False()
        {
            NewRule().UlnConditionMet(1111111111).Should().BeFalse();
        }

        [Theory]
        [InlineData(35, false)]
        [InlineData(99, true)]
        public void FundModelConditionMet_True(int fundModel, bool mockValue)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "ADL", "1")).Returns(mockValue);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .FundModelConditionMet(fundModel, learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(80, false)]
        [InlineData(80, true)]
        [InlineData(99, false)]
        public void FundModelConditionMet_False(int fundModel, bool mockValue)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "ADL", "1")).Returns(mockValue);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .FundModelConditionMet(fundModel, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void FilePreparationDateConditionMet_True()
        {
            var learnStartDate = new DateTime(2019, 04, 01);
            var filePrepDate = new DateTime(2019, 05, 01);
            var januaryFirst = new DateTime(2019, 01, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, filePrepDate)).Returns(30);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).FilePreparationDateConditionMet(learnStartDate, filePrepDate,  januaryFirst).Should().BeTrue();
        }

        [Theory]
        [InlineData(2018)]
        [InlineData(2019)]
        public void FilePreparationDateConditionMet_False(int filePrepDateYear)
        {
            var learnStartDate = new DateTime(2019, 03, 01);
            var filePrepDate = new DateTime(filePrepDateYear, 07, 01);
            var januaryFirst = new DateTime(2019, 01, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, filePrepDate)).Returns(100);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).FilePreparationDateConditionMet(learnStartDate, filePrepDate, januaryFirst).Should().BeFalse();
        }

        [Fact]
        public void LearningDatesConditionMet_True_LearnPlanEndDate()
        {
            var learnStartDate = new DateTime(2019, 01, 01);
            var learnPlanEndDate = new DateTime(2019, 05, 01);
            var learnActEndDate = new DateTime(2018, 05, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(10);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).LearningDatesConditionMet(learnStartDate, learnPlanEndDate, learnActEndDate).Should().BeTrue();
        }

        [Fact]
        public void LearningDatesConditionMet_True_LearnActEndDate()
        {
            var learnStartDate = new DateTime(2019, 01, 01);
            var learnPlanEndDate = new DateTime(2018, 05, 01);
            var learnActEndDate = new DateTime(2019, 05, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnActEndDate)).Returns(10);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).LearningDatesConditionMet(learnStartDate, learnPlanEndDate, learnActEndDate).Should().BeTrue();
        }

        [Fact]
        public void LearningDatesConditionMet_False_LearnActEndDateNull()
        {
            var learnStartDate = new DateTime(2019, 01, 01);
            var learnPlanEndDate = new DateTime(2018, 05, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(0);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).LearningDatesConditionMet(learnStartDate, learnPlanEndDate, null).Should().BeFalse();
        }

        [Theory]
        [InlineData(10, 10, 10)]
        [InlineData(01, 04, 03)]
        public void LearningDatesConditionMet_False(int learnStartDateDay, int learnPlanEndDateDay, int learnActEndDateDay)
        {
            var learnStartDate = new DateTime(2019, 05, learnStartDateDay);
            var learnPlanEndDate = new DateTime(2019, 05, learnPlanEndDateDay);
            var learnActEndDate = new DateTime(2019, 05, learnActEndDateDay);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, It.IsAny<DateTime>())).Returns(0);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).LearningDatesConditionMet(learnStartDate, learnPlanEndDate, learnActEndDate).Should().BeFalse();
        }

        [Theory]
        [InlineData("LDM", "034")]
        [InlineData("ACT", "1")]
        public void LearningDeliveryFAMConditionMet_True(string famType, string famCode)
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>();

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famType, famCode)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFams).Should().BeTrue();
        }

        [Theory]
        [InlineData("LDM", "034")]
        [InlineData("ACT", "1")]
        public void LearningDeliveryFAMConditionMet_False(string famType, string famCode)
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>();

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famType, famCode)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var uln = 9999999999;
            var fundModel = 35;
            var learnStartDate = new DateTime(2019, 02, 01);
            var learnPlanEndDate = new DateTime(2019, 05, 10);
            var learnActEndDate = new DateTime(2019, 05, 10);
            var filePrepDate = new DateTime(2019, 01, 01);
            var januaryFirst = new DateTime(2019, 01, 01);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(50);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(uln, fundModel, learningDeliveryFAMs, learnStartDate, learnPlanEndDate, learnActEndDate, filePrepDate, januaryFirst)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(1111111111, 35, "2019-01-01", "2019-05-10", 1, "LDM", "033")]
        [InlineData(9999999999, 100, "2019-01-01", "2019-05-10", 1, "LDM", "033")]
        [InlineData(9999999999, 35, "2018-3-10", "2019-05-10", 1, "LDM", "033")]
        [InlineData(9999999999, 35, "2019-01-01", "2019-05-01", 100, "LDM", "033")]
        [InlineData(9999999999, 99, "2019-01-01", "2019-05-01", 1, "LDM", "034")]
        [InlineData(9999999999, 99, "2019-01-01", "2019-05-01", 1, "ACT", "1")]
        public void ConditionMet_False(long uln, int fundModel, string filePrepDateString, string learnPlanEndDateString, double dateTimeMock, string famType, string famCode)
        {
            var learnStartDate = new DateTime(2019, 05, 01);
            var learnPlanEndDate = DateTime.Parse(learnPlanEndDateString);
            var learnActEndDate = new DateTime(2019, 05, 10);
            var filePrepDate = DateTime.Parse(filePrepDateString);
            var januaryFirst = new DateTime(2019, 01, 01);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ADL",
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(dateTimeMock);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(uln, fundModel, learningDeliveryFAMs, learnStartDate, learnPlanEndDate, learnActEndDate, filePrepDate, januaryFirst)
                .Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();
            var learner = new TestLearner()
            {
                ULN = 9999999999,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 35,
                        LearnStartDate = new DateTime(2019, 02, 01),
                        LearnPlanEndDate = new DateTime(2019, 05, 10),
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var academicDataQueryServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var fileDataCacheMock = new Mock<IFileDataCache>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicDataQueryServiceMock.Setup(qs => qs.JanuaryFirst()).Returns(new DateTime(2019, 01, 01));
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(50);
            fileDataCacheMock.Setup(fd => fd.FilePreparationDate).Returns(new DateTime(2019, 01, 01));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "ADL", "1")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    academicDataQueryServiceMock.Object,
                    dateTimeQueryServiceMock.Object,
                    fileDataCacheMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();
            var learner = new TestLearner()
            {
                ULN = 1,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 35,
                        LearnStartDate = new DateTime(2019, 05, 01),
                        LearnPlanEndDate = new DateTime(2019, 05, 10),
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var academicDataQueryServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var fileDataCacheMock = new Mock<IFileDataCache>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicDataQueryServiceMock.Setup(qs => qs.JanuaryFirst()).Returns(new DateTime(2019, 01, 01));
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(100);
            fileDataCacheMock.Setup(fd => fd.FilePreparationDate).Returns(new DateTime(2019, 01, 01));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "ADL", "1")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    academicDataQueryServiceMock.Object,
                    dateTimeQueryServiceMock.Object,
                    fileDataCacheMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ULN", (long)1234567890)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FilePrepDate", "01/01/2019")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/12/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1234567890, new DateTime(2019, 01, 01), new DateTime(2018, 12, 01));

            validationErrorHandlerMock.Verify();
        }

        private ULN_06Rule NewRule(
            IAcademicYearDataService academicDataQueryService = null,
            IDateTimeQueryService dateTimeQueryService = null,
            IFileDataCache fileDataCache = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_06Rule(academicDataQueryService, dateTimeQueryService, fileDataCache, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
