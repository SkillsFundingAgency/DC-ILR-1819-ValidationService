using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class ULN_04RuleTests
    {
        [Theory]
        [InlineData(1000000043)]
        [InlineData(null)]
        public void ConditionMet_True(long? uln)
        {
            NewRule().ConditionMet(uln, "N").Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(1000000004, "4").Should().BeFalse();
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                ULNNullable = 1000000043,
            };

            var dd01Mock = new Mock<IDD01>();

            dd01Mock.Setup(dd => dd.Derive(1000000043)).Returns("Y");

            var rule = NewRule(dd01Mock.Object);

            rule.Validate(learner);
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                ULNNullable = 1000000042,
            };

            var dd01Mock = new Mock<IDD01>();

            dd01Mock.Setup(dd => dd.Derive(1000000042)).Returns("N");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ULN_04", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = new ULN_04Rule(dd01Mock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        private ULN_04Rule NewRule(IDD01 dd01 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_04Rule(dd01, validationErrorHandler);
        }
    }
}
