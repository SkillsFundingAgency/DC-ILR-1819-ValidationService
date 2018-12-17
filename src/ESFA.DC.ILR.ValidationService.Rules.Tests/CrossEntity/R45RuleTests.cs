using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R45RuleTests : AbstractRuleTests<R47Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R45");
        }

        [Fact]
        public void ValidationPasses()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var testLearner = new TestLearner
            {
                LLDDAndHealthProblems = new List<ILLDDAndHealthProblem>
                {
                    new TestLLDDAndHealthProblem
                    {
                        LLDDCat = 1
                    },
                    new TestLLDDAndHealthProblem
                    {
                        LLDDCat = 2
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();
            var testLearner = new TestLearner
            {
                LLDDAndHealthProblems = new List<ILLDDAndHealthProblem>
                {
                    new TestLLDDAndHealthProblem
                    {
                        LLDDCat = 1
                    },
                    new TestLLDDAndHealthProblem
                    {
                        LLDDCat = 1
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()));
        }

        private R45Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R45Rule(validationErrorHandler);
        }
    }
}
