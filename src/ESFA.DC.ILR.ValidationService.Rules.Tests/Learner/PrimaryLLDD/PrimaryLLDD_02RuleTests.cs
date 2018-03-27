using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PrimaryLLDD
{
    public class PrimaryLLDD_02RuleTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public void ConditionMet_True(long? primaryLldValue)
        {
            var rule = NewRule();
            rule.ConditionMet(primaryLldValue).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public void ConditionMet_False(long? primaryLldValue)
        {
            var rule = NewRule();
            rule.ConditionMet(primaryLldValue).Should().BeFalse();
        }

        [Fact]
        public void Validate_True()
        {
            var validationErrorHandlerMock = SetupPrimaryLLdd(1);
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PrimaryLLDD_02", null, null, null);

            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        [Fact]
        public void ValidateFalse()
        {
            var validationErrorHandlerMock = SetupPrimaryLLdd(0);
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PrimaryLLDD_02", null, null, null);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        private Mock<IValidationErrorHandler> SetupPrimaryLLdd(long? primaryLlddValue)
        {
            var learner = new TestLearner()
            {
                LLDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
                {
                    new TestLLDDAndHealthProblem()
                    {
                        PrimaryLLDDNullable = primaryLlddValue
                    }
                }
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            return validationErrorHandlerMock;
        }

        private PrimaryLLDD_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PrimaryLLDD_02Rule(validationErrorHandler);
        }
    }
}