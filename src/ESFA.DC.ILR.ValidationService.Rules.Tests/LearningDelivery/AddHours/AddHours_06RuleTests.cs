using System;
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
    public class AddHours_06RuleTests : AbstractRuleTests<AddHours_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AddHours_06");
        }

        [Theory]
        [InlineData(9.1)]
        [InlineData(23.9)]
        public void ConditionMet_True(double? addHoursPerDay)
        {
            NewRule().ConditionMet(addHoursPerDay).Should().BeTrue();
        }

        [Theory]
        [InlineData(9)]
        [InlineData(24)]
        public void ConditionMet_False(double? addHoursPerDay)
        {
            NewRule().ConditionMet(addHoursPerDay).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDelivery = new TestLearningDelivery();

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var learningDeliveryQueryServiceMock = new Mock<ILearningDeliveryQueryService>();

            learningDeliveryQueryServiceMock.Setup(qs => qs.AverageAddHoursPerLearningDay(learningDelivery)).Returns(10);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDelivery = new TestLearningDelivery();

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var learningDeliveryQueryServiceMock = new Mock<ILearningDeliveryQueryService>();

            learningDeliveryQueryServiceMock.Setup(qs => qs.AverageAddHoursPerLearningDay(learningDelivery)).Returns(9);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/01/2016")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnPlanEndDate", "01/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AddHours", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2016, 1, 1), new DateTime(2017, 1, 1), 1);

            validationErrorHandlerMock.Verify();
        }

        private AddHours_06Rule NewRule(ILearningDeliveryQueryService learningDeliveryQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AddHours_06Rule(learningDeliveryQueryService, validationErrorHandler);
        }
    }
}
