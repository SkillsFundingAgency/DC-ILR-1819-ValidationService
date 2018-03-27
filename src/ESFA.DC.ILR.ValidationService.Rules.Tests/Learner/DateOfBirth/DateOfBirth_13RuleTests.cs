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
    public class DateOfBirth_13RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var academicYearEnd = new DateTime(2017, 7, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearEnd)).Returns(15);

            var rule = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.ConditionMet(99, dateOfBirth, academicYearEnd, true).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearningDeliveryFAM()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var academicYearEnd = new DateTime(2017, 7, 31);

            var rule = NewRule();

            rule.ConditionMet(99, dateOfBirth, academicYearEnd, false).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel_Null()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var academicYearEnd = new DateTime(2017, 7, 31);

            var rule = NewRule();

            rule.ConditionMet(null, dateOfBirth, academicYearEnd, true).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel_Different()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var academicYearEnd = new DateTime(2017, 7, 31);

            var rule = NewRule();

            rule.ConditionMet(100, dateOfBirth, academicYearEnd, true).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DateOfBirth_Null()
        {
            var learnStartDate = new DateTime(2017, 6, 30);

            var rule = NewRule();

            rule.ConditionMet(99, null, learnStartDate, true).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Age_17()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var academicYearEnd = new DateTime(2017, 7, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearEnd)).Returns(17);

            var rule = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.ConditionMet(10, dateOfBirth, academicYearEnd, true).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var academicYearEnd = new DateTime(2017, 7, 31);
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[] { };

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 99,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var validationDataServiceMock = new Mock<IValidationDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationDataServiceMock.SetupGet(vds => vds.AcademicYearEnd).Returns(new DateTime(2017, 7, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearEnd)).Returns(15);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "1")).Returns(true);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("DateOfBirth_13", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(validationDataServiceMock.Object, dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var learnStartDate = new DateTime(2017, 6, 30);
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[] { };

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 99,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var validationDataServiceMock = new Mock<IValidationDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            validationDataServiceMock.SetupGet(vds => vds.AcademicYearEnd).Returns(new DateTime(2017, 7, 31));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "1")).Returns(false);

            var rule = NewRule(validationDataServiceMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object);

            rule.Validate(learner);
        }

        private DateOfBirth_13Rule NewRule(IValidationDataService validationDataService = null, IDateTimeQueryService dateTimeQueryService = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_13Rule(validationDataService, dateTimeQueryService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
