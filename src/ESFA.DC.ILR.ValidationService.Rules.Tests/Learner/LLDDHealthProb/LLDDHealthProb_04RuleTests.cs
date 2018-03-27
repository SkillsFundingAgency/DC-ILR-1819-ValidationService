using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LLDDHealthProb
{
    public class LLDDHealthProb_04RuleTests
    {
        [Fact]
        public void ConditionMet_True_Null()
        {
            var rule = NewRule();
            rule.ConditionMet(2, null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_Empty()
        {
            var rule = NewRule();
            rule.ConditionMet(2,  new List<ILLDDAndHealthProblem>()).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var rule = NewRule();
            var llddAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                new TestLLDDAndHealthProblem() { }
            };
            rule.ConditionMet(2, llddAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                LLDDHealthProbNullable = 2
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LLDDHealthProb_04", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_True()
        {
            var learner = new TestLearner()
            {
                LLDDHealthProbNullable = 2,
                LLDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
                {
                    new TestLLDDAndHealthProblem() { }
                }
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LLDDHealthProb_04", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private LLDDHealthProb_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LLDDHealthProb_04Rule(validationErrorHandler);
        }
    }
}
