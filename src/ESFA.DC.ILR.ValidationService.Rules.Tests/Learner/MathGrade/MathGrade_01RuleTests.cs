using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.MathGrade
{
    public class MathGrade_01RuleTests
    {
        [Theory]
        [InlineData(null, 25)]
        [InlineData(null, 82)]
        [InlineData(" ", 82)]
        [InlineData("", 25)]
        public void ConditionMet_True(string mathGrade, long? fundModel)
        {
            NewRule().ConditionMet(mathGrade, fundModel).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet("X", 82).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        public void FundModelConditionMet_True(long? fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(15)]
        [InlineData(null)]
        public void FundModelConditionMet_False(long? fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner(string.Empty);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("MathGrade_01", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner("A");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("MathGrade_01", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private ILearner SetupLearner(string mathGrade)
        {
            var learner = new TestLearner()
            {
                MathGrade = mathGrade,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 25
                    }
                }
            };

            return learner;
        }

        private MathGrade_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new MathGrade_01Rule(validationErrorHandler);
        }
    }
}
