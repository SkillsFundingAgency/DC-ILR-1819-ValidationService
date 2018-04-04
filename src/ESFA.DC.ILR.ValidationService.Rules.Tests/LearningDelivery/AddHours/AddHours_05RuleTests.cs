using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AddHours
{
    public class AddHours_05RuleTests
    {
        [Fact]
        public void AddHoursConditionMet_True()
        {
            NewRule().AddHoursConditionMet(61).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(60)]
        public void AddHoursConditionMet_False(int? addHours)
        {
            NewRule().AddHoursConditionMet(addHours).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(5.1).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        [InlineData(1)]
        public void ConditionMet_False(double? averageHoursPerDay)
        {
            NewRule().ConditionMet(averageHoursPerDay).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                AddHoursNullable = 70
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var learningDeliveryQueryServiceMock = new Mock<ILearningDeliveryQueryService>();

            learningDeliveryQueryServiceMock.Setup(qs => qs.AverageAddHoursPerLearningDay(learningDelivery)).Returns(6);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("AddHours_05", null, 0, null);

            validationErrorHandlerMock.Setup(handle);

            NewRule(learningDeliveryQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);

            validationErrorHandlerMock.Verify(handle);
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AddHoursNullable = null
                    }
                }
            };

            NewRule().Validate(learner);
        }

        private AddHours_05Rule NewRule(ILearningDeliveryQueryService learningDeliveryQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AddHours_05Rule(learningDeliveryQueryService, validationErrorHandler);
        }
    }
}
