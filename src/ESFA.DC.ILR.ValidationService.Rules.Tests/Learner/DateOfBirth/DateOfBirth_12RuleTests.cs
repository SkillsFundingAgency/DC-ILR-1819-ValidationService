using System;
using System.Collections.Generic;
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
    public class DateOfBirth_12RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var learnStartDate = new DateTime(2017, 6, 30);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);

            var rule = NewRule(dateTimeQueryServiceMock.Object);

            rule.ConditionMet(10, dateOfBirth, learnStartDate, true).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearningDeliveryFAM()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var learnStartDate = new DateTime(2017, 6, 30);

            var rule = NewRule();

            rule.ConditionMet(10, dateOfBirth, learnStartDate, false).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel_Null()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var learnStartDate = new DateTime(2017, 6, 30);

            var rule = NewRule();

            rule.ConditionMet(null, dateOfBirth, learnStartDate, true).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel_Different()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var learnStartDate = new DateTime(2017, 6, 30);

            var rule = NewRule();

            rule.ConditionMet(11, dateOfBirth, learnStartDate, true).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DateOfBirth_Null()
        {
            var learnStartDate = new DateTime(2017, 6, 30);

            var rule = NewRule();

            rule.ConditionMet(10, null, learnStartDate, true).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate_Null()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);

            var rule = NewRule();

            rule.ConditionMet(10, dateOfBirth, null, true).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Age_19()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var learnStartDate = new DateTime(2017, 6, 30);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(19);

            var rule = NewRule(dateTimeQueryServiceMock.Object);

            rule.ConditionMet(10, dateOfBirth, learnStartDate, true).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
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
                        LearnStartDateNullable = learnStartDate,
                        FundModelNullable = 10,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "ASL", It.IsAny<IEnumerable<string>>())).Returns(true);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("DateOfBirth_12", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object);

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
                        LearnStartDateNullable = learnStartDate,
                        FundModelNullable = 10,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "ASL", It.IsAny<IEnumerable<string>>())).Returns(false);

            var rule = NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object);

            rule.Validate(learner);
        }

        private DateOfBirth_12Rule NewRule(IDateTimeQueryService dateTimeQueryService = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_12Rule(dateTimeQueryService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
