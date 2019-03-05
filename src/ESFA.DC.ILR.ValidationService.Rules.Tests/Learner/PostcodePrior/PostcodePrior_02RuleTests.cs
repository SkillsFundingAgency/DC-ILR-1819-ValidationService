using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PostcodePrior;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PostcodePrior
{
    public class PostcodePrior_02RuleTests : AbstractRuleTests<PostcodePrior_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PostcodePrior_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var postcodePrior = "abc";

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();

            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcodePrior)).Returns(false);

            NewRule(postcodeQueryServiceMock.Object).ConditionMet(postcodePrior).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var postcodePrior = "AA1 1AA";

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();

            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcodePrior)).Returns(true);

            NewRule(postcodeQueryServiceMock.Object).ConditionMet(postcodePrior).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var postcodePrior = "abc";

            var learner = new TestLearner()
            {
                PostcodePrior = postcodePrior
            };

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();

            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcodePrior)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(postcodeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var postcodePrior = "AA1 1AA";

            var learner = new TestLearner()
            {
                PostcodePrior = postcodePrior
            };

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();

            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcodePrior)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(postcodeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PostcodePrior", "abc")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("abc");

            validationErrorHandlerMock.Verify();
        }

        private PostcodePrior_02Rule NewRule(IPostcodeQueryService postcodeQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PostcodePrior_02Rule(postcodeQueryService, validationErrorHandler);
        }
    }
}
