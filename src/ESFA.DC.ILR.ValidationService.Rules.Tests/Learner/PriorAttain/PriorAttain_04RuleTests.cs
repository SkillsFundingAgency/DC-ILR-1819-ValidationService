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
    public class PriorAttain_04RuleTests
    {
        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public void ConditionMet_True(long? progType)
        {
            var priorAttainValues = new List<int> { 4, 5, 10, 11, 12, 13 };

            var rule = NewRule();

            foreach (var item in priorAttainValues)
            {
                rule.ConditionMet(item, 35, progType).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData(null, 35, null)]
        [InlineData(null, 35, 2)]
        [InlineData(4, null, null)]
        [InlineData(4, null, 3)]
        [InlineData(4, 9999, 3)]
        public void ConditionMet_False(long? priorAttain, long? fundModel, long? progType)
        {
            NewRule().ConditionMet(priorAttain, fundModel, progType).Should().BeFalse();
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

        [Theory]
        [InlineData(3)]
        [InlineData(2)]
        public void ProgTypeConditionMet_True(long? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner(5);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PriorAttain_04", null, null, null);

            var rule = new PriorAttain_04Rule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner(999);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PriorAttain_04", null, null, null);

            var rule = new PriorAttain_04Rule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private PriorAttain_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PriorAttain_04Rule(validationErrorHandler);
        }

        private ILearner SetupLearner(long priorAttain)
        {
            return new TestLearner()
            {
                PriorAttainNullable = priorAttain,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 35,
                        ProgTypeNullable = 2,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { },
                    }
                }
            };
        }
    }
}
