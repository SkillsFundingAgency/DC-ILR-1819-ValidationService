using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LLDDHealthProb
{
    public class LLDDHealthProb_01RuleTests : AbstractRuleTests<LLDDHealthProb_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LLDDHealthProb_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var llddHealthProb = 5;

            NewRule().ConditionMet(llddHealthProb).Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(9)]
        public void ConditionMet_False(int llddHealthProb)
        {
            NewRule().ConditionMet(llddHealthProb).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                LLDDHealthProb = 5
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(9)]
        public void ValidateNoError(int llddHealthProb)
        {
            var learner = new TestLearner()
            {
                LLDDHealthProb = llddHealthProb
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LLDDHealthProb", 1)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private LLDDHealthProb_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LLDDHealthProb_01Rule(validationErrorHandler);
        }
    }
}
