using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AddHours
{
    public class AddHours_02RuleTests
    {
        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        [InlineData(10)]
        [InlineData(99)]
        public void ConditionMet_True(int fundModel)
        {
            NewRule().ConditionMet(fundModel, 1).Should().BeTrue();
        }

        [Theory]
        [InlineData(25, null)]
        [InlineData(24, 1)]
        public void ConditionMet_False(int fundModel, int? addHours)
        {
            NewRule().ConditionMet(fundModel, addHours).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                        AddHoursNullable = 1,
                    }
                }
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("AddHours_02", null, 0, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.VerifyAll();
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 24
                    }
                }
            };

            NewRule().Validate(learner);
        }

        private AddHours_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AddHours_02Rule(validationErrorHandler);
        }
    }
}
