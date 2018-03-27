using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_10RuleTests
    {
        [Fact]
        public void LearnerNullConditionMet_True()
        {
            var rule = NewRule();

            rule.LearnerNullConditionMet(new DateTime(1988, 12, 25)).Should().BeTrue();
        }

        [Fact]
        public void LearnerNullConditionMet_False()
        {
            var rule = NewRule();

            rule.LearnerNullConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryNullConditionMet_True()
        {
            var rule = NewRule();

            rule.LearningDeliveryNullConditionMet(new DateTime(2016, 12, 25), new DateTime(2017, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryNullConditionMet_False_DD04()
        {
            var rule = NewRule();

            rule.LearningDeliveryNullConditionMet(null, new DateTime(2017, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryNullConditionMet_False_LearnStartDate()
        {
            var rule = NewRule();

            rule.LearningDeliveryNullConditionMet(new DateTime(2017, 8, 1), null).Should().BeFalse();
        }

        [Fact]
        public void DD04ConditionMet_True()
        {
            var rule = NewRule();

            rule.DD04ConditionMet(new DateTime(2014, 8, 1)).Should().BeTrue();
        }

        [Theory]
        [InlineData("2012-8-1")]
        [InlineData("2016-8-1")]
        public void DD04ConditionMet_False(string dd04)
        {
            var rule = NewRule();

            rule.DD04ConditionMet(DateTime.Parse(dd04)).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var rule = NewRule();

            rule.DD07ConditionMet("Y").Should().BeTrue();
        }

        [Theory]
        [InlineData("N")]
        [InlineData("AnythingElse")]
        [InlineData(null)]
        public void DD07ConditionMet_False(string dd07)
        {
            var rule = NewRule();

            rule.DD07ConditionMet(dd07).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthLearnStartDateConditionMet_True()
        {
            var learnStartDate = new DateTime(2014, 7, 1);
            var dateOfBirth = new DateTime(1998, 7, 2);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearCalendarServiceMock = new Mock<IAcademicYearCalendarService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(13);

            academicYearCalendarServiceMock.Setup(aycs => aycs.LastFridayInJuneForDateInAcademicYear(learnStartDate)).Returns(new DateTime(2014, 6, 27));

            var rule = NewRule(academicYearCalendarService: academicYearCalendarServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.DateOfBirthLearnStartDateConditionMet(learnStartDate, dateOfBirth).Should().BeTrue();
        }

        [Fact]
        public void DateOfBirthLearnStartDateConditionMet_True_15()
        {
            var learnStartDate = new DateTime(2014, 7, 1);
            var dateOfBirth = new DateTime(1999, 7, 2);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearCalendarServiceMock = new Mock<IAcademicYearCalendarService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(15);

            academicYearCalendarServiceMock.Setup(aycs => aycs.LastFridayInJuneForDateInAcademicYear(learnStartDate)).Returns(new DateTime(2014, 6, 27));

            var rule = NewRule(academicYearCalendarService: academicYearCalendarServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.DateOfBirthLearnStartDateConditionMet(learnStartDate, dateOfBirth).Should().BeTrue();
        }

        [Fact]
        public void DateOfBirthLearnStartDateConditionMet_False_Age()
        {
            var learnStartDate = new DateTime(2014, 7, 1);
            var dateOfBirth = new DateTime(1998, 7, 2);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearCalendarServiceMock = new Mock<IAcademicYearCalendarService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(17);

            academicYearCalendarServiceMock.Setup(aycs => aycs.LastFridayInJuneForDateInAcademicYear(learnStartDate)).Returns(new DateTime(2014, 6, 27));

            var rule = NewRule(academicYearCalendarService: academicYearCalendarServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.DateOfBirthLearnStartDateConditionMet(learnStartDate, dateOfBirth).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthLearnStartDateConditionMet_True_SixteenBefore()
        {
            var learnStartDate = new DateTime(2014, 7, 1);
            var dateOfBirth = new DateTime(1998, 5, 2);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearCalendarServiceMock = new Mock<IAcademicYearCalendarService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(15);

            academicYearCalendarServiceMock.Setup(aycs => aycs.LastFridayInJuneForDateInAcademicYear(learnStartDate)).Returns(new DateTime(2014, 6, 27));

            var rule = NewRule(academicYearCalendarService: academicYearCalendarServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.DateOfBirthLearnStartDateConditionMet(learnStartDate, dateOfBirth).Should().BeTrue();
        }

        [Fact]
        public void DateOfBirthLearnStartDateConditionMet_True_SixteenAfter()
        {
            var learnStartDate = new DateTime(2014, 7, 1);
            var dateOfBirth = new DateTime(1998, 9, 2);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearCalendarServiceMock = new Mock<IAcademicYearCalendarService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(15);

            academicYearCalendarServiceMock.Setup(aycs => aycs.LastFridayInJuneForDateInAcademicYear(learnStartDate)).Returns(new DateTime(2014, 6, 27));

            var rule = NewRule(academicYearCalendarService: academicYearCalendarServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.DateOfBirthLearnStartDateConditionMet(learnStartDate, dateOfBirth).Should().BeTrue();
        }

        [Fact]
        public void DateOfBirthLearnStartDateConditionMet_True_LearnStartDateBefore()
        {
            var learnStartDate = new DateTime(2014, 5, 1);
            var dateOfBirth = new DateTime(1999, 7, 2);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearCalendarServiceMock = new Mock<IAcademicYearCalendarService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(15);

            academicYearCalendarServiceMock.Setup(aycs => aycs.LastFridayInJuneForDateInAcademicYear(learnStartDate)).Returns(new DateTime(2014, 6, 27));

            var rule = NewRule(academicYearCalendarService: academicYearCalendarServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.DateOfBirthLearnStartDateConditionMet(learnStartDate, dateOfBirth).Should().BeTrue();
        }

        [Fact]
        public void DateOfBirthLearnStartDateConditionMet_True_LearnStartDate_After()
        {
            var learnStartDate = new DateTime(2014, 9, 1);
            var dateOfBirth = new DateTime(1999, 7, 2);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearCalendarServiceMock = new Mock<IAcademicYearCalendarService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(15);

            academicYearCalendarServiceMock.Setup(aycs => aycs.LastFridayInJuneForDateInAcademicYear(learnStartDate)).Returns(new DateTime(2014, 6, 27));

            var rule = NewRule(academicYearCalendarService: academicYearCalendarServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.DateOfBirthLearnStartDateConditionMet(learnStartDate, dateOfBirth).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnStartDate = new DateTime(2014, 7, 1);
            var dateOfBirth = new DateTime(1998, 7, 2);

            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDateNullable = learnStartDate,
                ProgTypeNullable = 25,
            };

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    learningDelivery
                }
            };

            var dd04Mock = new Mock<IDD04>();
            var dd07Mock = new Mock<IDD07>();
            var academicYearCalendarServiceMock = new Mock<IAcademicYearCalendarService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            dd04Mock.Setup(dd => dd.Derive(learner.LearningDeliveries, learningDelivery)).Returns(learnStartDate);
            dd07Mock.Setup(dd => dd.Derive(learningDelivery.ProgTypeNullable)).Returns("Y");
            academicYearCalendarServiceMock.Setup(aycs => aycs.LastFridayInJuneForDateInAcademicYear(learnStartDate)).Returns(new DateTime(2018, 6, 29));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(13);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("DateOfBirth_10", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(dd04Mock.Object, dd07Mock.Object, academicYearCalendarServiceMock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner();

            NewRule().Validate(learner);
        }

        private DateOfBirth_10Rule NewRule(IDD04 dd04 = null, IDD07 dd07 = null, IAcademicYearCalendarService academicYearCalendarService = null, IDateTimeQueryService dateTimeQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_10Rule(dd04, dd07, academicYearCalendarService, dateTimeQueryService, validationErrorHandler);
        }
    }
}
