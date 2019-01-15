using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Sex;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.Sex
{
    public class Sex_01RuleTests : AbstractRuleTests<Sex_01Rule>
    {
        [Theory]
        [InlineData("A")]
        public void ConditionMet_True(string sex)
        {
            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(LookupCodedKey.Sex, sex)).Returns(false);
            NewRule(provideLookupDetails: provideLookupDetails.Object).ConditionMet(sex).Should().BeTrue();
        }

        [Theory]
        [InlineData("F")]
        [InlineData("M")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void ConditionMet_False(string sex)
        {
            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(LookupCodedKey.Sex, sex)).Returns(true);
            NewRule(provideLookupDetails: provideLookupDetails.Object).ConditionMet(sex).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                Sex = "X"
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(LookupCodedKey.Sex, learner.Sex)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                Sex = "F"
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(LookupCodedKey.Sex, learner.Sex)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.Sex, "A")).Verifiable();
            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("A");
            validationErrorHandlerMock.Verify();
        }

        private Sex_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IProvideLookupDetails provideLookupDetails = null)
        {
            return new Sex_01Rule(validationErrorHandler, provideLookupDetails);
        }
    }
}