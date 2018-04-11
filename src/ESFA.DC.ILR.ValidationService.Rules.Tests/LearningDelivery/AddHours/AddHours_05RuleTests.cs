using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AddHours
{
    public class AddHours_05RuleTests : AbstractRuleTests
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AddHours_05");
        }

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

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
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

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AddHours_05Rule NewRule(ILearningDeliveryQueryService learningDeliveryQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AddHours_05Rule(learningDeliveryQueryService, validationErrorHandler);
        }
    }
}
