using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_03RuleTests : AbstractRuleTests<DateOfBirth_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_03");
        }

        [Fact]
        public void DateOfBirthCondtionMet_True()
        {
            NewRule().DateOfBirthConditionMet(new DateTime(2000, 01, 01)).Should().BeTrue();
        }

        [Fact]
        public void DateOfBirthCondtionMet_False()
        {
            NewRule().DateOfBirthConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void AgeCondtionMet_True()
        {
            var dateOfBirth = new DateTime(1918, 01, 01);
            var start = new DateTime(2018, 08, 01);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(start);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, start)).Returns(100);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object).AgeConditionMet(dateOfBirth).Should().BeTrue();
        }

        [Fact]
        public void AgeCondtionMet_False()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var start = new DateTime(2018, 08, 01);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(start);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, start)).Returns(18);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object).AgeConditionMet(dateOfBirth).Should().BeFalse();
        }

        [Fact]
        public void CondtionMet_True()
        {
            var dateOfBirth = new DateTime(1918, 01, 01);
            var start = new DateTime(2018, 08, 01);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(start);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, start)).Returns(100);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object).AgeConditionMet(dateOfBirth).Should().BeTrue();
        }

        [Fact]
        public void CondtionMet_False_NullDOB()
        {
            var dateOfBirth = new DateTime(1918, 01, 01);
            var start = new DateTime(2018, 08, 01);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(start);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, start)).Returns(100);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object).AgeConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void CondtionMet_False_Age()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var start = new DateTime(2018, 08, 01);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(start);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, start)).Returns(18);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object).AgeConditionMet(dateOfBirth).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(1918, 01, 01);
            var start = new DateTime(2018, 08, 01);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(start);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, start)).Returns(100);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var start = new DateTime(2018, 08, 01);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(start);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, start)).Returns(18);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("DateOfBirth", "01/01/2000")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2000, 01, 01));

            validationErrorHandlerMock.Verify();
        }

        private DateOfBirth_03Rule NewRule(IAcademicYearDataService academicYearDataService = null, IDateTimeQueryService dateTimeQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_03Rule(academicYearDataService, dateTimeQueryService, validationErrorHandler);
        }
    }
}
