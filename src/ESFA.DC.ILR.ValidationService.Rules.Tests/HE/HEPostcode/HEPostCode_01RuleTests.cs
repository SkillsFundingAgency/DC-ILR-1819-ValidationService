using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.HEPostcode;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.HEPostcode
{
    public class HEPostCode_01RuleTests : AbstractRuleTests<HEPostCode_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("HEPostCode_01");
        }

        [Fact]
        public void NullConditionMet_True()
        {
            var heEntity = new TestLearningDeliveryHE()
            {
                HEPostCode = "AA1 1AA"
            };

            NewRule().NullConditionMet(heEntity).Should().BeTrue();
        }

        [Fact]
        public void NullConditionMet_FalseNullEntity()
        {
            NewRule().NullConditionMet(null).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void NullConditionMet_FalseNullPostcode(string postcode)
        {
            var heEntity = new TestLearningDeliveryHE()
            {
                HEPostCode = postcode
            };

            NewRule().NullConditionMet(heEntity).Should().BeFalse();
        }

        [Fact]
        public void TemporaryPostcodeConditionMet_True()
        {
            var postcode = "AA1 1AA";

            NewRule().TemporaryPostcodeConditionMet(postcode).Should().BeTrue();
        }

        [Fact]
        public void TemporaryPostcodeConditionMet_False()
        {
            var postcode = "ZZ99 9ZZ";

            NewRule().TemporaryPostcodeConditionMet(postcode).Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionMet_True()
        {
            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();
            postcodesDataServiceMock.Setup(pdsm => pdsm.PostcodeExists(It.IsAny<string>())).Returns(false);

            NewRule(postcodesDataService: postcodesDataServiceMock.Object).PostcodeConditionMet(It.IsAny<string>()).Should().BeTrue();
        }

        [Fact]
        public void PostcodeConditionMet_False()
        {
            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();
            postcodesDataServiceMock.Setup(pdsm => pdsm.PostcodeExists(It.IsAny<string>())).Returns(true);

            NewRule(postcodesDataService: postcodesDataServiceMock.Object).PostcodeConditionMet(It.IsAny<string>()).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var heEntity = new TestLearningDeliveryHE()
            {
                HEPostCode = "XXXXXX"
            };

            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();
            postcodesDataServiceMock.Setup(pdsm => pdsm.PostcodeExists(It.IsAny<string>())).Returns(false);

            NewRule(postcodesDataService: postcodesDataServiceMock.Object).ConditionMet(heEntity).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var heEntity = new TestLearningDeliveryHE()
            {
                HEPostCode = "AA1 1AA"
            };

            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();
            postcodesDataServiceMock.Setup(pdsm => pdsm.PostcodeExists(It.IsAny<string>())).Returns(true);

            NewRule(postcodesDataService: postcodesDataServiceMock.Object).ConditionMet(heEntity).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            HEPostCode = "XXX XXX"
                        }
                    }
                }
            };

            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();
            postcodesDataServiceMock.Setup(pdsm => pdsm.PostcodeExists(It.IsAny<string>())).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, postcodesDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            HEPostCode = "AA1 1AA"
                        }
                    }
                }
            };

            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();
            postcodesDataServiceMock.Setup(pdsm => pdsm.PostcodeExists(It.IsAny<string>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, postcodesDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("HEPostCode", "abc")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("abc");

            validationErrorHandlerMock.Verify();
        }

        private HEPostCode_01Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IPostcodesDataService postcodesDataService = null)
        {
            return new HEPostCode_01Rule(validationErrorHandler, postcodesDataService);
        }
    }
}
