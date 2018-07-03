using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Postcode;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.Postcode
{
    public class Postcode_14RuleTests : AbstractRuleTests<Postcode_14Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Postcode_14");
        }

        [Fact]
        public void NullConditionMet_False_Null()
        {
            NewRule().NullConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void NullConditionMet_False_Whitespace()
        {
            NewRule().NullConditionMet(" ").Should().BeFalse();
        }

        [Fact]
        public void NullConditionMet_True()
        {
            NewRule().NullConditionMet("Postcode").Should().BeTrue();
        }

        [Fact]
        public void TemporaryPostcodeConditionMet_False()
        {
            NewRule().TemporaryPostcodeConditionMet("ZZ99 9ZZ").Should().BeFalse();
        }

        [Fact]
        public void TemporaryPostcodeConditionMet_True()
        {
            NewRule().TemporaryPostcodeConditionMet("Postcode").Should().BeTrue();
        }

        [Fact]
        public void PostcodeConditionMet_False()
        {
            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();

            postcodesDataServiceMock.Setup(ds => ds.PostcodeExists("Postcode")).Returns(true);

            NewRule(postcodesDataServiceMock.Object).PostcodeConditionMet("Postcode").Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionMet_True()
        {
            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();

            postcodesDataServiceMock.Setup(ds => ds.PostcodeExists("Postcode")).Returns(false);

            NewRule(postcodesDataServiceMock.Object).PostcodeConditionMet("Postcode").Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("ZZ99 9ZZ")]
        [InlineData("Postcode")]
        public void ConditionMet_False(string Postcode)
        {
            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();

            postcodesDataServiceMock.Setup(ds => ds.PostcodeExists("Postcode")).Returns(true);

            NewRule(postcodesDataServiceMock.Object).ConditionMet("Postcode").Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();

            postcodesDataServiceMock.Setup(ds => ds.PostcodeExists("Postcode")).Returns(false);

            NewRule(postcodesDataServiceMock.Object).ConditionMet("Postcode").Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                Postcode = "Postcode",
            };

            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();

            postcodesDataServiceMock.Setup(ds => ds.PostcodeExists("Postcode")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(postcodesDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                Postcode = "Postcode",
            };

            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();

            postcodesDataServiceMock.Setup(ds => ds.PostcodeExists("Postcode")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(postcodesDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("Postcode", "Postcode")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("Postcode");

            validationErrorHandlerMock.Verify();
        }

        private Postcode_14Rule NewRule(IPostcodesDataService postcodesDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new Postcode_14Rule(postcodesDataService, validationErrorHandler);
        }
    }
}
