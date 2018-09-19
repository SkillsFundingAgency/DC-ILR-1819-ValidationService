using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PriorAttain
{
    public class PriorAttain_05RuleTests
    {
        [Fact]
        public void RuleConditionMet_True()
        {
            var priorAttainValues = new List<int> { 4, 5, 10, 11, 12, 13 };

            var rule = NewRule();

            foreach (var item in priorAttainValues)
            {
                rule.ConditionMet(item, 35, 20, new DateTime(2015, 8, 02)).Should().BeTrue();
            }
        }

        [Fact]
        public void RuleConditionMet_False()
        {
            var priorAttainValues = new List<int> { 4, 5, 10, 11, 12, 13 };
            var rule = NewRule();

            foreach (var item in priorAttainValues)
            {
                rule.ConditionMet(item, 35, 10, new DateTime(2015, 8, 02)).Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(999)]
        public void PriorAttainConditionMet_False(long? priorAttain)
        {
            NewRule().PriorAttainConditionMet(priorAttain).Should().BeFalse();
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        public void PriorAttainConditionMet_True(long? priorAttain)
        {
            NewRule().PriorAttainConditionMet(priorAttain).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(999)]
        public void FundModelConditionMet_False(long? fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(999)]
        public void ProgTypeConditionMet_False(long? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeConditionMet_True()
        {
            NewRule().ProgTypeConditionMet(20).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2015, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_False_Null()
        {
            NewRule().LearnStartDateConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2015, 08, 01)).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner(5, new DateTime(2015, 08, 02));

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PriorAttain_05", null, null, null);

            var rule = new PriorAttain_05Rule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner(999, new DateTime(2015, 07, 31));

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PriorAttain_05", null, null, null);

            var rule = new PriorAttain_05Rule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private ILearner SetupLearner(long priorAttain, DateTime learnStartDate)
        {
            return new TestLearner()
            {
                PriorAttainNullable = priorAttain,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { },
                        FundModelNullable = 35,
                        ProgTypeNullable = 20,
                        LearnStartDateNullable = learnStartDate,
                    }
                }
            };
        }

        private PriorAttain_05Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PriorAttain_05Rule(validationErrorHandler);
        }
    }
}
