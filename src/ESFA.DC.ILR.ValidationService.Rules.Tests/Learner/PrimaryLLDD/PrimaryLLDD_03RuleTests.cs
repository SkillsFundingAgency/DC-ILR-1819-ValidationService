using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PrimaryLLDD
{
    public class PrimaryLLDD_03RuleTests : AbstractRuleTests<PrimaryLLDD_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PrimaryLLDD_03");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20,
                    PrimaryLLDDNullable = 1
                },
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 98,
                    PrimaryLLDDNullable = 1
                }
            };

            NewRule().ConditionMet(llddAndHealthProblems).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20,
                    PrimaryLLDDNullable = 1
                },
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 98
                }
            };

            NewRule().ConditionMet(llddAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            NewRule().ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var llddHealthProb = 1;
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20,
                    PrimaryLLDDNullable = 1
                },
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 98,
                    PrimaryLLDDNullable = 1
                }
            };

            var learner = new TestLearner
            {
                LLDDHealthProb = llddHealthProb,
                LLDDAndHealthProblems = llddAndHealthProblems
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var llddHealthProb = 1;
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20,
                    PrimaryLLDDNullable = 1
                },
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 98
                }
            };

            var learner = new TestLearner
            {
                LLDDHealthProb = llddHealthProb,
                LLDDAndHealthProblems = llddAndHealthProblems
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private PrimaryLLDD_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PrimaryLLDD_03Rule(validationErrorHandler);
        }
    }
}