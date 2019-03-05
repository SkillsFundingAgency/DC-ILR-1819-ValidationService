using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class ULN_04RuleTests : AbstractRuleTests<ULN_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ULN_04");
        }

        [Theory]
        [InlineData(1000000043)]
        [InlineData(null)]
        public void ConditionMet_True(long? uln)
        {
            NewRule().ConditionMet(uln, "N").Should().BeTrue();
        }

        [Theory]
        [InlineData(1000000043, "Y")]
        [InlineData(1000000043, "3")]
        public void ConditionMet_False(long? uln, string dd01)
        {
            NewRule().ConditionMet(uln, dd01).Should().BeFalse();
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                ULN = 1000000043,
            };

            var dd01Mock = new Mock<IDerivedData_01Rule>();

            dd01Mock.Setup(dd => dd.Derive(1000000043)).Returns("Y");

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd01Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                ULN = 1000000042,
            };

            var dd01Mock = new Mock<IDerivedData_01Rule>();

            dd01Mock.Setup(dd => dd.Derive(1000000042)).Returns("N");

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd01Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ULN", (long)1234567890)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1234567890);

            validationErrorHandlerMock.Verify();
        }

        private ULN_04Rule NewRule(IDerivedData_01Rule dd01 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_04Rule(dd01, validationErrorHandler);
        }
    }
}
