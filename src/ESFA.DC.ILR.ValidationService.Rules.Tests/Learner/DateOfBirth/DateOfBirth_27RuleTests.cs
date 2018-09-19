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
    public class DateOfBirth_27RuleTests : AbstractRuleTests<DateOfBirth_27Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_27");
        }

        [Fact]
        public void CondtionMet_True()
        {
            var dateOfBirth = new DateTime(2020, 01, 01);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(new DateTime(2018, 8, 1));

            NewRule(academicYearDataServiceMock.Object).ConditionMet(dateOfBirth).Should().BeTrue();
        }

        [Fact]
        public void CondtionMet_False()
        {
            var dateOfBirth = new DateTime(2000, 09, 01);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(new DateTime(2018, 8, 1));

            NewRule(academicYearDataServiceMock.Object).ConditionMet(dateOfBirth).Should().BeFalse();
        }

        [Fact]
        public void CondtionMet_False_Null()
        {
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(new DateTime(2018, 8, 1));

            NewRule(academicYearDataServiceMock.Object).ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(2020, 01, 01);
            var start = new DateTime(2018, 08, 01);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(new DateTime(2018, 8, 1));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(academicYearDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(new DateTime(2018, 8, 1));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(academicYearDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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

        private DateOfBirth_27Rule NewRule(IAcademicYearDataService academicYearDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_27Rule(academicYearDataService, validationErrorHandler);
        }
    }
}
