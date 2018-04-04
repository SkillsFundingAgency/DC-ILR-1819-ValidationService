using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AddHours
{
    public class AddHours_01RuleTests : AbstractRuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(35, 1, new DateTime(2015, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(34, null, new DateTime(2015, 9, 1)).Should().BeFalse();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(81)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(99)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void AddHoursConditionMet_True()
        {
            NewRule().AddHoursConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void AddHoursConditionMet_False()
        {
            NewRule().AddHoursConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDate_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2015, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDate_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2015, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        AddHoursNullable = 1,
                        LearnStartDate = new DateTime(2015, 1, 1),
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError("AddHours_01"))
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
                        FundModel = 34,
                        LearnStartDate = new DateTime(2015, 1, 1),
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AddHours_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AddHours_01Rule(validationErrorHandler);
        }
    }
}
