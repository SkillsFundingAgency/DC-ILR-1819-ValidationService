using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LearnFAMType
{
    public class LearnFAMType_09RuleTests : AbstractRuleTests<LearnFAMType_09Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnFAMType_09");
        }

        [Theory]
        [InlineData(LearnerFAMTypeConstants.HNS, 0, false)]
        [InlineData(LearnerFAMTypeConstants.HNS, 1, false)]
        [InlineData(LearnerFAMTypeConstants.EHC, 1, false)]
        [InlineData(LearnerFAMTypeConstants.DLA, 1, false)]
        [InlineData(LearnerFAMTypeConstants.LSR, 4, false)]
        [InlineData(LearnerFAMTypeConstants.SEN, 1, false)]
        [InlineData(LearnerFAMTypeConstants.NLM, 2, false)]
        [InlineData(LearnerFAMTypeConstants.EDF, 2, false)]
        [InlineData(LearnerFAMTypeConstants.MCF, 1, false)]
        [InlineData(LearnerFAMTypeConstants.ECF, 1, false)]
        [InlineData(LearnerFAMTypeConstants.FME, 1, false)]
        [InlineData(LearnerFAMTypeConstants.PPE, 1, false)]
        [InlineData("ABC", 2, false)]
        [InlineData(null, 1, false)]
        [InlineData(LearnerFAMTypeConstants.HNS, 2, true)]
        [InlineData(LearnerFAMTypeConstants.EHC, 2, true)]
        [InlineData(LearnerFAMTypeConstants.DLA, 3, true)]
        [InlineData(LearnerFAMTypeConstants.SEN, 4, true)]
        [InlineData(LearnerFAMTypeConstants.MCF, 4, true)]
        [InlineData(LearnerFAMTypeConstants.ECF, 2, true)]
        [InlineData(LearnerFAMTypeConstants.FME, 2, true)]
        public void ConditionMetMeetsExpectation(string learnFamType, int count, bool expectation)
        {
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMQueryServiceMock.Setup(s => s.GetLearnerFAMsCountByFAMType(It.IsAny<List<ILearnerFAM>>(), learnFamType)).Returns(count);

            NewRule(learnerFAMQueryServiceMock.Object).ConditionMet(learnFamType, It.IsAny<List<ILearnerFAM>>()).Should().Be(expectation);
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.DLA
                    }
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(x => x.GetLearnerFAMsCountByFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(2);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_WithNoLearnerFAMS_Returns_NoError()
        {
            var learner = new TestLearner();

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(x => x.GetLearnerFAMsCountByFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(1);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner
            {
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.DLA
                    }
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(x => x.GetLearnerFAMsCountByFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(1);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_CalledOnceForEachFamType()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35
                    }
                },
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.HNS },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.HNS },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.EHC },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.EHC },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.EHC },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.DLA },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.DLA },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.SEN },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.SEN },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.MCF },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.MCF },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.ECF },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.ECF },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.ECF }
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(x => x.GetLearnerFAMsCountByFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(2);

            var validationErrorHandlerMock = new ValidationErrorHandlerMock(true);
            NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);

            validationErrorHandlerMock.Verify(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.HNS), Times.Once);
            validationErrorHandlerMock.Verify(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.EHC), Times.Once);
            validationErrorHandlerMock.Verify(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.DLA), Times.Once);
            validationErrorHandlerMock.Verify(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.SEN), Times.Once);
            validationErrorHandlerMock.Verify(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.MCF), Times.Once);
            validationErrorHandlerMock.Verify(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.ECF), Times.Once);
            validationErrorHandlerMock.Verify(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.FME), Times.Never);
        }

        [Fact]
        public void BuilderErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.ECF)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearnerFAMTypeConstants.ECF);

            validationErrorHandlerMock.Verify();
        }

        private LearnFAMType_09Rule NewRule(
            ILearnerFAMQueryService learnerFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnFAMType_09Rule(learnerFAMQueryService, validationErrorHandler);
        }
    }
}
