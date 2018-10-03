using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PrimaryLLDD
{
    public class PrimaryLLDD_02RuleTests : AbstractRuleTests<PrimaryLLDD_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PrimaryLLDD_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(2).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public void ConditionMet_False(int? primaryLldValue)
        {
            NewRule().ConditionMet(primaryLldValue).Should().BeFalse();
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
                    PrimaryLLDDNullable = 2
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

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var primaryLLDD = 2;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PrimaryLLDD", primaryLLDD)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(primaryLLDD);

            validationErrorHandlerMock.Verify();
        }

        private PrimaryLLDD_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PrimaryLLDD_02Rule(validationErrorHandler);
        }
    }
}