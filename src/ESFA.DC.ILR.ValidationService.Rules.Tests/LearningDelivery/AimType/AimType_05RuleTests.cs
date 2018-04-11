using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AimType
{
    public class AimType_05RuleTests : AbstractRuleTests
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AimType_05");
        }

        [Theory]
        [InlineData(5, 25)]
        [InlineData(5, 82)]
        [InlineData(4, 25)]
        public void ConditionMet_False(int aimType, int fundModel)
        {
            NewRule().ConditionMet(aimType, fundModel).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(5, 10).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 5,
                        FundModel = 10
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
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
                        AimType = 5,
                        FundModel = 25
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AimType_05Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AimType_05Rule(validationErrorHandler);
        }
    }
}
