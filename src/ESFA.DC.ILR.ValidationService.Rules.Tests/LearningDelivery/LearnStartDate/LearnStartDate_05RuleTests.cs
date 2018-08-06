using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_05RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(new DateTime(2018, 1, 1), new DateTime(2017, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_DateOfBirth_Null()
        {
            NewRule().ConditionMet(null, new DateTime(2017, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate_Null()
        {
            NewRule().ConditionMet(new DateTime(1988, 12, 25), null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DateOfBirth()
        {
            NewRule().ConditionMet(new DateTime(1988, 12, 25), new DateTime(2017, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                DateOfBirthNullable = new DateTime(1988, 12, 25),
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDateNullable = new DateTime(2015, 1, 1),
                    }
                }
            };

            NewRule().Validate(learner);
        }

        [Fact]
        public void Validate_Errors()
        {
            var learner = new TestLearner()
            {
                DateOfBirthNullable = new DateTime(2018, 1, 1),
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDateNullable = new DateTime(2005, 1, 1),
                    }
                }
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnStartDate_05", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = new LearnStartDate_05Rule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        private LearnStartDate_05Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnStartDate_05Rule(validationErrorHandler);
        }
    }
}
