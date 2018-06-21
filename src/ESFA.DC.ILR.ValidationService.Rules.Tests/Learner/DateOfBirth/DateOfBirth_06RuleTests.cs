using System;
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
    public class DateOfBirth_06RuleTests : AbstractRuleTests<DateOfBirth_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_06");
        }

        [Theory]
        [InlineData(25, 1)]
        [InlineData(82, 12)]
        public void ConditionMet_True(int fundModel, int age)
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var academicYearAugustThirtyFirst = new DateTime(2018, 8, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearAugustThirtyFirst)).Returns(age);

            var rule = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.ConditionMet(dateOfBirth, academicYearAugustThirtyFirst, fundModel).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            NewRule().ConditionMet(null, new DateTime(2018, 8, 31), 1).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel_DateOfBirth_Null()
        {
            NewRule().ConditionMet(null, new DateTime(2018, 8, 31), 25).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Age()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var academicYearAugustThirtyFirst = new DateTime(2018, 8, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearAugustThirtyFirst)).Returns(13);

            var rule = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.ConditionMet(dateOfBirth, academicYearAugustThirtyFirst, 25).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 8, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(12);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dateTimeQueryServiceMock.Object, academicYearDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 8, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(20);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dateTimeQueryServiceMock.Object, academicYearDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("DateOfBirth", "01/01/2015")).Verifiable();

            NewRule(null, validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2015, 01, 01));

            validationErrorHandlerMock.Verify();
        }

        public DateOfBirth_06Rule NewRule(IDateTimeQueryService dateTimeQueryService = null, IAcademicYearDataService academicYearDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_06Rule(dateTimeQueryService, academicYearDataService, validationErrorHandler);
        }
    }
}
