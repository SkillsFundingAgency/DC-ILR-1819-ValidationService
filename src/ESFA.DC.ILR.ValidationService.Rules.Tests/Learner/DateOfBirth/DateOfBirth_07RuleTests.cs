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
    public class DateOfBirth_07RuleTests
    {
        [Fact]
        public void LearnerConditionMet_True()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var academicYearAugustThirtyFirst = new DateTime(2017, 8, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearAugustThirtyFirst)).Returns(25);

            var rule = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.LearnerConditionMet(dateOfBirth, academicYearAugustThirtyFirst).Should().BeTrue();
        }

        [Fact]
        public void LearnerConditionMet_False_DateOfBirth_Null()
        {
            var rule = NewRule();

            rule.LearnerConditionMet(null, new DateTime(2017, 8, 31)).Should().BeFalse();
        }

        [Fact]
        public void LearnerConditionMet_False_DateOfBirth()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var academicYearAugustThirtyFirst = new DateTime(2017, 8, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearAugustThirtyFirst)).Returns(24);

            var rule = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.LearnerConditionMet(dateOfBirth, academicYearAugustThirtyFirst).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        public void LearningDeliveryConditionMet_True(long fundModel)
        {
            var rule = NewRule();

            rule.LearningDeliveryConditionMet(fundModel, true).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryConditionMet_False_FamTypeCode()
        {
            var rule = NewRule();

            rule.LearningDeliveryConditionMet(25, false).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryConditionMet_False_FundModel_Null()
        {
            var rule = NewRule();

            rule.LearningDeliveryConditionMet(null, true).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryConditionMet_False_FundModel()
        {
            var rule = NewRule();

            rule.LearningDeliveryConditionMet(1, true).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var academicYearAugustThirtyFirst = new DateTime(2017, 8, 31);
            var learningDeliveryFams = new TestLearningDeliveryFAM[] { };

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 25,
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var validationDataServiceMock = new Mock<IValidationDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationDataServiceMock.SetupGet(vds => vds.AcademicYearAugustThirtyFirst).Returns(academicYearAugustThirtyFirst);

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearAugustThirtyFirst)).Returns(25);

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "SOF", "107")).Returns(true);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("DateOfBirth_07", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(validationDataServiceMock.Object, dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var academicYearAugustThirtyFirst = new DateTime(2017, 8, 31);

            var learner = new TestLearner();

            var validationDataServiceMock = new Mock<IValidationDataService>();

            validationDataServiceMock.SetupGet(vds => vds.AcademicYearAugustThirtyFirst).Returns(academicYearAugustThirtyFirst);

            var rule = NewRule(validationDataServiceMock.Object);

            rule.Validate(learner);
        }

        private DateOfBirth_07Rule NewRule(IValidationDataService validationDataService = null, IDateTimeQueryService dateTimeQueryService = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_07Rule(validationDataService, dateTimeQueryService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
