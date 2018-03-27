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
    public class DateOfBirth_14RuleTests
    {
        [Theory]
        [InlineData(35)]
        [InlineData(81)]
        public void ConditionMet_True(long? fundModel)
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var learnStartDate = new DateTime(2017, 6, 30);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(17);

            var rule = NewRule(dateTimeQueryServiceMock.Object);

            rule.ConditionMet(fundModel, dateOfBirth, learnStartDate, true).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearningDeliveryFAM()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var learnStartDate = new DateTime(2017, 6, 30);

            NewRule().ConditionMet(35, dateOfBirth, learnStartDate, false).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel_Null()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var learnStartDate = new DateTime(2017, 6, 30);

            NewRule().ConditionMet(null, dateOfBirth, learnStartDate, true).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel_Different()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var learnStartDate = new DateTime(2017, 6, 30);

            var rule = NewRule();

            rule.ConditionMet(36, dateOfBirth, learnStartDate, true).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DateOfBirth_Null()
        {
            var learnStartDate = new DateTime(2017, 6, 30);

            NewRule().ConditionMet(35, null, learnStartDate, true).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate_Null()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);

            NewRule().ConditionMet(35, dateOfBirth, null, true).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Age_19()
        {
            var dateOfBirth = new DateTime(2000, 1, 1);
            var learnStartDate = new DateTime(2017, 6, 30);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);

            var rule = NewRule(dateTimeQueryServiceMock.Object);

            rule.ConditionMet(35, dateOfBirth, learnStartDate, true).Should().BeFalse();
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
                        FundModelNullable = 35,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(17);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(true);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("DateOfBirth_14", null, null, null);

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
                        FundModelNullable = 35,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            var rule = NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object);

            rule.Validate(learner);
        }

        private DateOfBirth_14Rule NewRule(IDateTimeQueryService dateTimeQueryService = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_14Rule(dateTimeQueryService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
