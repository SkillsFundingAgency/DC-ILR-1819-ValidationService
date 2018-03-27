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
    public class PriorAttain_07RuleTests
    {
        [Fact]
        public void RuleConditionMet_True()
        {
            var priorAttainValues = new List<int> { 4, 5, 10, 11, 12, 13, 97, 98 };

            var rule = NewRule();

            foreach (var item in priorAttainValues)
            {
                rule.ConditionMet(item, 35, 24, new DateTime(2016, 8, 01)).Should().BeTrue();
            }
        }

        [Fact]
        public void RuleConditionMet_False()
        {
            var priorAttainValues = new List<int> { 4, 5, 10, 11, 12, 13, 97, 98 };

            var rule = NewRule();

            foreach (var item in priorAttainValues)
            {
                rule.ConditionMet(item, 35, 10, new DateTime(2016, 8, 01)).Should().BeFalse();
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
        [InlineData(97)]
        [InlineData(98)]
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

        [Theory]
        [InlineData(35)]
        public void FundModelConditionMet_True(long? fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
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
            NewRule().ProgTypeConditionMet(24).Should().BeTrue();
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
            NewRule().LearnStartDateConditionMet(new DateTime(2016, 08, 01)).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner(5, new DateTime(2016, 08, 02));

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PriorAttain_07", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner(999, new DateTime(2015, 07, 31));

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PriorAttain_07", null, null, null);

            var rule = new PriorAttain_07Rule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private PriorAttain_07Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PriorAttain_07Rule(validationErrorHandler);
        }

        private ILearner SetupLearner(long priorAttain, DateTime learnStartDate)
        {
            return new TestLearner
            {
                PriorAttainNullable = priorAttain,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 35,
                        ProgTypeNullable = 24,
                        LearnStartDateNullable = learnStartDate,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
                    }
                }
            };
        }
    }
}
