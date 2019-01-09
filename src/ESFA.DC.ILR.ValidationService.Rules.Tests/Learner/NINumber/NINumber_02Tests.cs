using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.NiNumber;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.NINumber
{
    public class NINumber_02Tests : AbstractRuleTests<NINumber_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("NINumber_02");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void NiNumberConditionMet_True(string niNumber)
        {
            NewRule().NiNumberConditionMet(niNumber).Should().BeTrue();
        }

        [Fact]
        public void NiNumberConditionMet_False()
        {
            var niNumber = "AA123456A";

            NewRule().NiNumberConditionMet(niNumber).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var famType = "ACT";
            var famCode = "1";

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, famType, famCode))
                .Returns(true);

            NewRule(learningDeliveryFamQueryServiceMock.Object).ConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var famType = "XXX";
            var famCode = "5";

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, famType, famCode))
                .Returns(false);

            NewRule(learningDeliveryFamQueryServiceMock.Object).ConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var famType = "ACT";
            var famCode = "1";

            var learner = new TestLearner()
            {
                NINumber = " ",
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = famType,
                                LearnDelFAMCode = famCode
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMs = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, famType, famCode))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var famType = "XXX";
            var famCode = "1";

            var learner = new TestLearner()
            {
                NINumber = "AA123456A",
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = famType,
                                LearnDelFAMCode = famCode
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMs = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, famType, famCode))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("NINumber", " ")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(" ");

            validationErrorHandlerMock.Verify();
        }

        private NINumber_02Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new NINumber_02Rule(learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
