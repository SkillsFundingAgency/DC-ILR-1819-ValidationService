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
    public class PrimaryLLDD_04RuleTests : AbstractRuleTests<PrimaryLLDD_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PrimaryLLDD_04");
        }

        [Fact]
        public void LLDDHealthProbConditionMet_True()
        {
            NewRule().LLDDHealthProbConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void LLDDHealthProbConditionMet_False()
        {
            NewRule().LLDDHealthProbConditionMet(20).Should().BeFalse();
        }

        [Fact]
        public void LLDDConditionMet_True()
        {
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20
                },
            };

            NewRule().LLDDConditionMet(llddAndHealthProblems).Should().BeTrue();
        }

        [Fact]
        public void LLDDConditionMet_False_PrimaryLLDD()
        {
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20,
                    PrimaryLLDDNullable = 1
                }
            };

            NewRule().LLDDConditionMet(llddAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void LLDDConditionMet_False_Count()
        {
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20
                },
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 10
                }
            };

            NewRule().LLDDConditionMet(llddAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void LLDDConditionMet_False_Excluded()
        {
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 98
                }
            };

            NewRule().LLDDConditionMet(llddAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void LLDDConditionMet_False_NullRecords()
        {
            NewRule().LLDDConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var lldHealthProb = 1;

            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20,
                }
            };

            NewRule().ConditionMet(lldHealthProb, llddAndHealthProblems).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NoLLDDHealthProb()
        {
            var lldHealthProb = 2;

            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20,
                }
            };

            NewRule().ConditionMet(lldHealthProb, llddAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LLDDHealthProblems()
        {
            var lldHealthProb = 1;

            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 98,
                }
            };

            NewRule().ConditionMet(lldHealthProb, llddAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var llddHealthProb = 1;
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20
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

        private PrimaryLLDD_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PrimaryLLDD_04Rule(validationErrorHandler);
        }
    }
}