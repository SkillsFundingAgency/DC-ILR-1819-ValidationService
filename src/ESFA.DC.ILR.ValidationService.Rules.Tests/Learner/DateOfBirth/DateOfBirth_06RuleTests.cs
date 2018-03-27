using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_06RuleTests
    {
        public DateOfBirth_06Rule NewRule(IValidationDataService validationDataService = null, IDateTimeQueryService dateTimeQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_06Rule(validationDataService, dateTimeQueryService, validationErrorHandler);
        }

        [Theory]
        [InlineData(25, 1)]
        [InlineData(82, 12)]
        public void ConditionMet_True(long fundModel, int age)
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var academicYearAugustThirtyFirst = new DateTime(2017, 8, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearAugustThirtyFirst)).Returns(age);

            var rule = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.ConditionMet(dateOfBirth, academicYearAugustThirtyFirst, fundModel).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel_Null()
        {
            var rule = NewRule();

            rule.ConditionMet(null, new DateTime(2017, 8, 31), null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var rule = NewRule();

            rule.ConditionMet(null, new DateTime(2017, 8, 31), 1).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel_DateOfBirth_Null()
        {
            var rule = NewRule();

            rule.ConditionMet(null, new DateTime(2017, 8, 31), 10).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Age()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var academicYearAugustThirtyFirst = new DateTime(2017, 8, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearAugustThirtyFirst)).Returns(4);

            var rule = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.ConditionMet(dateOfBirth, academicYearAugustThirtyFirst, 10).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var learnStartDate = new DateTime(2017, 8, 1);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 25,
                    }
                }
            };

            var validationDataServiceMock = new Mock<IValidationDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationDataServiceMock.SetupGet(vds => vds.AcademicYearAugustThirtyFirst).Returns(new DateTime(2017, 8, 31));

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(12);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("DateOfBirth_06", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(validationDataServiceMock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                }
            };

            var validationDataServiceMock = new Mock<IValidationDataService>();
            validationDataServiceMock.SetupGet(vds => vds.AcademicYearAugustThirtyFirst).Returns(new DateTime(2017, 8, 31));

            var rule = NewRule(validationDataServiceMock.Object);

            rule.Validate(learner);
        }
    }
}
