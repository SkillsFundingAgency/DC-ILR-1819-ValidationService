using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCFLDCS;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.PCFLDCS
{
    public class PCFLDCS_02RuleTests : AbstractRuleTests<PCFLDCS_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PCFLDCS_02");
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_True_LDHE_Null()
        {
            NewRule().LearningDeliveryHEConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_True_PCFLDCS_Null()
        {
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                NUMHUS = "AB1"
            };

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False()
        {
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                PCFLDCSNullable = 1.2m
            };

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void LARSLearningDeliveryConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef)).Returns(true);

            NewRule(larsDataServiceMock.Object).LARSLearningDeliveryConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LARSLearningDeliveryConditionMet_False()
        {
            var learnAimRef = "LearnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef)).Returns(false);

            NewRule(larsDataServiceMock.Object).LARSLearningDeliveryConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";
            var learningDeliveryHE = new TestLearningDeliveryHE();

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef)).Returns(true);

            NewRule(larsDataServiceMock.Object).ConditionMet(learningDeliveryHE, learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learnAimRef = "LearnAimRef";
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                PCFLDCSNullable = 1.2m
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef)).Returns(true);

            NewRule(larsDataServiceMock.Object).ConditionMet(learningDeliveryHE, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var learnAimRef = "LearnAimRef";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData("1.2", true)]
        [InlineData("1.2", false)]
        public void Validate_NoErrors(string pcfldcs, bool mockValue)
        {
            var learnAimRef = "LearnAimRef";
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                PCFLDCSNullable = decimal.Parse(pcfldcs)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LearningDeliveryHEEntity = learningDeliveryHE
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef)).Returns(mockValue);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private PCFLDCS_02Rule NewRule(ILARSDataService larsDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PCFLDCS_02Rule(larsDataService, validationErrorHandler);
        }
    }
}
