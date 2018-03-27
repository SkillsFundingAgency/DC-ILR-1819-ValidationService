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
    public class DateOfBirth_04RuleTests
    {
        [Theory]
        [InlineData(115)]
        [InlineData(125)]
        [InlineData(999)]
        public void ConditionMet_True(int age)
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var academicYearStart = new DateTime(2017, 8, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearStart)).Returns(age);

            var rule = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.ConditionMet(dateOfBirth, academicYearStart).Should().BeTrue();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(15)]
        [InlineData(114)]
        public void ConditionMet_False(int age)
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var academicYearStart = new DateTime(2017, 8, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearStart)).Returns(age);

            var rule = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.ConditionMet(dateOfBirth, academicYearStart).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            var rule = NewRule();

            rule.ConditionMet(null, new DateTime(2017, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var academicYearStart = new DateTime(2017, 8, 1);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth
            };

            var validationDataServiceMock = new Mock<IValidationDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationDataServiceMock.SetupGet(vds => vds.AcademicYearStart).Returns(academicYearStart);

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearStart)).Returns(115);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("DateOfBirth_04", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(validationDataServiceMock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var academicYearStart = new DateTime(2017, 8, 1);

            var learner = new TestLearner();

            var validationDataServiceMock = new Mock<IValidationDataService>();

            validationDataServiceMock.SetupGet(vds => vds.AcademicYearStart).Returns(academicYearStart);

            var rule = NewRule(validationDataServiceMock.Object);

            rule.Validate(learner);
        }

        private DateOfBirth_04Rule NewRule(IValidationDataService validationDataService = null, IDateTimeQueryService dateTimeQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_04Rule(validationDataService, dateTimeQueryService, validationErrorHandler);
        }
    }
}
