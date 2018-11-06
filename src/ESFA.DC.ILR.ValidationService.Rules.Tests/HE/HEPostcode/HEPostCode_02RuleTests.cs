using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.HEPostcode;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.HEPostcode
{
    public class HEPostCode_02RuleTests : AbstractRuleTests<HEPostCode_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("HEPostCode_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var postcode = "AA1AA";

            var deliveryHE = new TestLearningDeliveryHE
            {
                HEPostCode = postcode
            };

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();
            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcode)).Returns(false);

            NewRule(postcodeQueryServiceMock.Object).ConditionMet(deliveryHE).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var postcode = "AA1 1AA";

            var deliveryHE = new TestLearningDeliveryHE
            {
                HEPostCode = postcode
            };

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();
            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcode)).Returns(true);

            NewRule(postcodeQueryServiceMock.Object).ConditionMet(deliveryHE).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseNull()
        {
            NewRule().ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var postcode = "AA1AA";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            HEPostCode = postcode
                        }
                    }
                }
            };

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();
            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcode)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(postcodeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var postcode = "AA1 1AA";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            HEPostCode = postcode
                        }
                    }
                }
            };

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();
            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcode)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(postcodeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var postcode = "A11AA";

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.HEPostcode, postcode)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(postcode);

            validationErrorHandlerMock.Verify();
        }

        private HEPostCode_02Rule NewRule(
            IPostcodeQueryService postcodeQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new HEPostCode_02Rule(postcodeQueryService, validationErrorHandler);
        }
    }
}
