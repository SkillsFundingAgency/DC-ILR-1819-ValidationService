using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.EngGrade
{
    public class EngGrade_01RuleTests
    {
        [Theory]
        [InlineData(null, 25)]
        [InlineData(null, 82)]
        [InlineData(" ", 82)]
        [InlineData("", 25)]
        public void ConditionMet_True(string engGrade, long? fundModel)
        {
            NewRule().ConditionMet(engGrade, fundModel).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet("X", 82).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        public void FundModelCondition_True(long? fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(15)]
        [InlineData(null)]
        public void FundModelCondition_False(long? fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner(string.Empty);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("EngGrade_01", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner("A");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("EngGrade_01", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private EngGrade_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new EngGrade_01Rule(validationErrorHandler);
        }

        private ILearner SetupLearner(string engGrade)
        {
            return new TestLearner
            {
                EngGrade = engGrade,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 25
                    }
                }
            };
        }
    }
}
