using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PriorLearnFundAdj;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.PriorLearnFundAdj
{
    public class PriorLearnFundAdj_01RuleTests : AbstractRuleTests<PriorLearnFundAdj_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PriorLearnFundAdj_01");
        }

        [Fact]
        public void PriorLearFu0dnAdjConditionMet_True()
        {
            NewRule().PriorLearnFundAdjConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void PriorLearFu0dnAdjConditionMet_False()
        {
            NewRule().PriorLearnFundAdjConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(25).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(35).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                 new TestLearningDeliveryFAM
                 {
                     LearnDelFAMType = "LDM",
                     LearnDelFAMCode = "001"
                 }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ADL")).Returns(false);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True_Null()
        {
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ADL")).Returns(false);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                 new TestLearningDeliveryFAM
                 {
                     LearnDelFAMType = "ADL",
                     LearnDelFAMCode = "001"
                 }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ADL")).Returns(true);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                 new TestLearningDeliveryFAM
                 {
                     LearnDelFAMType = "LDM",
                     LearnDelFAMCode = "001"
                 }
            };
            var fundModel = 25;
            int? priorLearnFundAdj = 100;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.PriorLearnFundAdjConditionMet(priorLearnFundAdj)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(true);

            ruleMock.Object.ConditionMet(priorLearnFundAdj, fundModel, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                 new TestLearningDeliveryFAM
                 {
                     LearnDelFAMType = "ADL",
                     LearnDelFAMCode = "001"
                 }
            };
            var fundModel = 25;
            int? priorLearnFundAdj = 100;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.PriorLearnFundAdjConditionMet(priorLearnFundAdj)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(false);

            ruleMock.Object.ConditionMet(priorLearnFundAdj, fundModel, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                        PriorLearnFundAdjNullable = 100,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ADL")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                        PriorLearnFundAdjNullable = 100,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ADL")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 25)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PriorLearnFundAdj", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", "ADL")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(25, 1, "ADL");

            validationErrorHandlerMock.Verify();
        }

        private PriorLearnFundAdj_01Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PriorLearnFundAdj_01Rule(learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
