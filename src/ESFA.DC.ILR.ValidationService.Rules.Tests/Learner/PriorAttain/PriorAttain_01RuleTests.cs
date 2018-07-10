using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PriorAttain
{
    public class PriorAttain_01RuleTests : AbstractRuleTests<PriorAttain_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PriorAttain_01");
        }

        [Fact]
        public void PriorAttainConditionMet_True()
        {
            NewRule().PriorAttainConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void PriorAttainConditionMet_False()
        {
            NewRule().PriorAttainConditionMet(97).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(99).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(25).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "108"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "108")).Returns(true);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMSConditionMet(99, learningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(81, "SOF", "108", true)]
        [InlineData(99, "LDM", "108", false)]
        [InlineData(99, "SOF", "110", false)]
        [InlineData(81, "SOF", "110", false)]
        public void LearningDeliveryFAMConditionMet_True(int fundModel, string famType, string famCode, bool mockBool)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "108")).Returns(mockBool);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMSConditionMet(fundModel, learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(97, 81, "SOF", "110", false)]
        [InlineData(null, 10, "SOF", "110", false)]
        [InlineData(null, 99, "SOF", "108", true)]
        public void ConditionMet_False(int? priorAttain, int fundModel, string famType, string famCode, bool mockBool)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "108")).Returns(mockBool);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(priorAttain, fundModel, learningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(null, 80, "SOF", "110", false)]
        [InlineData(null, 99, "SOF", "110", false)]
        [InlineData(null, 70, "SOF", "108", true)]
        public void ConditionMet_True(int? priorAttain, int fundModel, string famType, string famCode, bool mockBool)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "108")).Returns(mockBool);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(priorAttain, fundModel, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                 new TestLearningDeliveryFAM
                 {
                     LearnDelFAMType = "SOF",
                     LearnDelFAMCode = "108"
                 }
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 70,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "108")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                 new TestLearningDeliveryFAM
                 {
                     LearnDelFAMType = "SOF",
                     LearnDelFAMCode = "108"
                 }
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 99,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "108")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private PriorAttain_01Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PriorAttain_01Rule(learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
