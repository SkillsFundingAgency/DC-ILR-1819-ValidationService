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
    public class PrimaryLLDD_04RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var rule = NewRule();
            var lllddAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(null)
            };
            rule.ConditionMet(lllddAndHealthProblems).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var rule = NewRule();
            var lllddAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(1)
            };
            rule.ConditionMet(lllddAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_MultipleRecords()
        {
            var rule = NewRule();
            var lllddAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(99),
                SetupLlddHealthAndProblem(20)
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
        public void Exclude_False()
        {
            var rule = NewRule(null);

            var llDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(9999),
                SetupLlddHealthAndProblem(100),
                SetupLlddHealthAndProblem(null)
            };
            rule.Exclude(llDDAndHealthProblems).Should().BeFalse();
        }

        [Theory]
        [InlineData(99)]
        [InlineData(98)]
        public void Exclude_True(long? excludeCatValue)
        {
            var rule = NewRule(null);

            var llDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(null, excludeCatValue),
            };
            rule.Exclude(llDDAndHealthProblems).Should().BeTrue();
        }

        [Fact]
        public void Validate_True()
        {
            var validationErrorHandlerMock = SetupPrimaryLLdd(1);
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PrimaryLLDD_04", null, null, null);

            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        [Fact]
        public void ValidateFalse()
        {
            var validationErrorHandlerMock = SetupPrimaryLLdd(null);
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PrimaryLLDD_04", null, null, null);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        private Mock<IValidationErrorHandler> SetupPrimaryLLdd(long? primaryLlddValue)
        {
            var learner = new TestLearner()
            {
                LLDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
                {
                    SetupLlddHealthAndProblem(primaryLlddValue),
                }
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            return validationErrorHandlerMock;
        }

        private PrimaryLLDD_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PrimaryLLDD_04Rule(validationErrorHandler);
        }

        private TestLLDDAndHealthProblem SetupLlddHealthAndProblem(long? primarylldValue, long? lldCatValue = 10)
        {
            return new TestLLDDAndHealthProblem()
            {
                LLDDCatNullable = lldCatValue,
                PrimaryLLDDNullable = primarylldValue
            };
        }
    }
}