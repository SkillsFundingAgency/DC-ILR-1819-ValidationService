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
    public class DateOfBirth_05RuleTests
    {
        public DateOfBirth_05Rule NewRule(IDateTimeQueryService dateTimeQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_05Rule(dateTimeQueryService, validationErrorHandler);
        }

        [Theory]
        [InlineData(10, 1)]
        [InlineData(99, 3)]
        public void ConditionMet_True(long fundModel, int age)
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var learnStartDate = new DateTime(2017, 8, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(age);

            var rule = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.ConditionMet(dateOfBirth, learnStartDate, fundModel).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel_Null()
        {
            var rule = NewRule();

            rule.ConditionMet(null, null, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var rule = NewRule();

            rule.ConditionMet(null, null, 1).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel_DateOfBirth_Null()
        {
            var rule = NewRule();

            rule.ConditionMet(null, null, 10).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel_LearnStartDate_Null()
        {
            var rule = NewRule();

            rule.ConditionMet(new DateTime(1988, 12, 25), null, 10).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Age()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var learnStartDate = new DateTime(2017, 8, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(4);

            var rule = NewRule(dateTimeQueryServiceMock.Object);

            rule.ConditionMet(dateOfBirth, learnStartDate, 10).Should().BeFalse();
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
                        FundModelNullable = 10,
                        LearnStartDateNullable = learnStartDate
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(3);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("DateOfBirth_05", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object);

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

            NewRule().Validate(learner);
        }
    }
}
