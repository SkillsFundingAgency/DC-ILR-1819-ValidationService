using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PriorAttain
{
    public class PriorAttain_02RuleTests : AbstractRuleTests<PriorAttain_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PriorAttain_02");
        }

        [Fact]
        public void PriorAttainConditionMet_True()
        {
            NewRule().PriorAttainConditionMet(97).Should().BeTrue();
        }

        [Fact]
        public void PriorAttainConditionMet_False_Null()
        {
            NewRule().PriorAttainConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void PriorAttainConditionMet_False_()
        {
            NewRule().PriorAttainConditionMet(100).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(99).Should().BeFalse();
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void LARSConditionMet_True(bool mockValueLevelTwo, bool mockValueLevelThree)
        {
            var learnAimRef = "LearnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.FullLevel2EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)).Returns(mockValueLevelTwo);
            larsDataServiceMock.Setup(ds => ds.FullLevel3EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)).Returns(mockValueLevelThree);

            NewRule(larsDataServiceMock.Object).LARSConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False()
        {
            var learnAimRef = "LearnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.FullLevel2EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)).Returns(false);
            larsDataServiceMock.Setup(ds => ds.FullLevel3EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)).Returns(false);

            NewRule(larsDataServiceMock.Object).LARSConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "347"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "347")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMSConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData("LDM", "108")]
        [InlineData("SOF", "347")]
        [InlineData("SOF", "110")]
        public void LearningDeliveryFAMConditionMet_True(string famType, string famCode)
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

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "347")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMSConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(97, 81, "LearnAimRef", "LDM", "110", false, true)]
        [InlineData(null, 81, "LearnAimRef", "LDM", "110", false, true)]
        [InlineData(97, 99, "LearnAimRef", "LDM", "110", false, true)]
        [InlineData(97, 81, "LearnAimRef", "LDM", "110", false, false)]
        [InlineData(97, 81, "LearnAimRef", "LDM", "347", true, true)]
        public void ConditionMet_False(int? priorAttain, int fundModel, string learnAimRef, string famType, string famCode, bool mockLARSbool, bool mockFAMBool)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "347")).Returns(mockFAMBool);
            larsDataServiceMock.Setup(ds => ds.FullLevel2EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)).Returns(mockLARSbool);
            larsDataServiceMock.Setup(ds => ds.FullLevel3EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)).Returns(mockLARSbool);

            NewRule(larsDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object).ConditionMet(priorAttain, fundModel, learnAimRef, learningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(97, 81, "LearnAimRef", "LDM", "110", false, true)]
        [InlineData(98, 81, "LearnAimRef", "LDM", "110", false, true)]
        [InlineData(98, 35, "LearnAimRef", "LDM", "110", false, true)]
        [InlineData(98, 70, "LearnAimRef", "LDM", "110", false, true)]
        [InlineData(98, 81, "LearnAimRef", "LDM", "110", false, true)]
        [InlineData(98, 81, "LearnAimRef", "LDM", "110", true, true)]
        [InlineData(98, 81, "LearnAimRef", "LDM", "110", true, false)]
        public void ConditionMet_True(int? priorAttain, int fundModel, string learnAimRef, string famType, string famCode, bool mockValueLevelTwo, bool mockValueLevelThree)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "347")).Returns(false);
            larsDataServiceMock.Setup(ds => ds.FullLevel2EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)).Returns(mockValueLevelTwo);
            larsDataServiceMock.Setup(ds => ds.FullLevel3EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)).Returns(mockValueLevelThree);

            NewRule(larsDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object).ConditionMet(priorAttain, fundModel, learnAimRef, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnAimRef = "LearnAimRef";

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
                PriorAttainNullable = 97,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = learnAimRef,
                        FundModel = 81,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "347")).Returns(false);
            larsDataServiceMock.Setup(ds => ds.FullLevel2EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)).Returns(true);
            larsDataServiceMock.Setup(ds => ds.FullLevel3EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnAimRef = "LearnAimRef";

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
                PriorAttainNullable = 97,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 81,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "347")).Returns(true);
            larsDataServiceMock.Setup(ds => ds.FullLevel2EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)).Returns(false);
            larsDataServiceMock.Setup(ds => ds.FullLevel3EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PriorAttain", 97)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(97);

            validationErrorHandlerMock.Verify();
        }

        private PriorAttain_02Rule NewRule(ILARSDataService larsDataService = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PriorAttain_02Rule(larsDataService, learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
