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
    public class AddHours_04RuleTests : AbstractRuleTests<AddHours_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AddHours_04");
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
        public void AddHoursConditionMet_True()
        {
            NewRule().AddHoursConditionMet(61).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(25).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0.0)]
        [InlineData(24.0)]
        public void ConditionMet_False(double? averageHours)
        {
            NewRule().ConditionMet(averageHours).Should().BeFalse();
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

            learningDeliveryQueryServiceMock.Setup(qs => qs.AverageAddHoursPerLearningDay(learningDelivery)).Returns(25);

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

        private AddHours_04Rule NewRule(ILearningDeliveryQueryService learningDeliveryQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AddHours_04Rule(learningDeliveryQueryService, validationErrorHandler);
        }
    }
}
