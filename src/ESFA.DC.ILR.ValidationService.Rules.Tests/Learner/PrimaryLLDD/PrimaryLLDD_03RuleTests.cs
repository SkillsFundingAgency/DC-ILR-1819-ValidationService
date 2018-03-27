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
    public class PrimaryLLDD_03RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var rule = NewRule();
            var lllddAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(1),
                SetupLlddHealthAndProblem(1)
            };
            rule.ConditionMet(lllddAndHealthProblems).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var rule = NewRule();
            var lllddAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(99),
                SetupLlddHealthAndProblem(1)
            };
            rule.ConditionMet(lllddAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            var rule = NewRule();
            rule.ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void Validate_True()
        {
            var validationErrorHandlerMock = SetupPrimaryLLdd(1, 99);
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PrimaryLLDD_03", null, null, null);

            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        [Fact]
        public void ValidateFalse()
        {
            var validationErrorHandlerMock = SetupPrimaryLLdd(1, 1);
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PrimaryLLDD_03", null, null, null);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        private Mock<IValidationErrorHandler> SetupPrimaryLLdd(long? primaryLlddValue1, long? primaryLlddValue2)
        {
            var learner = new TestLearner()
            {
                LLDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
                {
                    SetupLlddHealthAndProblem(primaryLlddValue1),
                    SetupLlddHealthAndProblem(primaryLlddValue2)
                }
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            return validationErrorHandlerMock;
        }

        private PrimaryLLDD_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PrimaryLLDD_03Rule(validationErrorHandler);
        }

        private TestLLDDAndHealthProblem SetupLlddHealthAndProblem(long? primarylldValue)
        {
            return new TestLLDDAndHealthProblem()
            {
                LLDDCatNullable = 10,
                PrimaryLLDDNullable = primarylldValue
            };
        }
    }
}