using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LearnFAMType
{
    public class LearnFAMType_14RuleTests : AbstractRuleTests<LearnFAMType_14Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnFAMType_14");
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(false, true, false)]
        [InlineData(false, false, false)]
        [InlineData(true, false, false)]
        public void ConditionMetMeetsExpectation(bool learnFamTypeSENExists, bool learnFamTypeEHCExists, bool expectation)
        {
            var learnerFAMsQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(It.IsAny<IEnumerable<ILearnerFAM>>(), LearnerFAMTypeConstants.SEN, 1)).Returns(learnFamTypeSENExists);
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(It.IsAny<IEnumerable<ILearnerFAM>>(), LearnerFAMTypeConstants.EHC, 1)).Returns(learnFamTypeEHCExists);

            NewRule(learnerFAMsQueryServiceMock.Object).ConditionMet(It.IsAny<IEnumerable<ILearnerFAM>>()).Should().Be(expectation);
        }

        [Fact]
        public void Validate_Error()
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
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.SEN,
                        LearnFAMCode = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.EHC,
                        LearnFAMCode = 1
                    }
                }
            };
            var learnerFAMsQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(It.IsAny<IEnumerable<ILearnerFAM>>(), LearnerFAMTypeConstants.SEN, 1)).Returns(true);
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(It.IsAny<IEnumerable<ILearnerFAM>>(), LearnerFAMTypeConstants.EHC, 1)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerFAMsQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        public void Validate_NoError()
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
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.SEN,
                        LearnFAMCode = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.DLA,
                        LearnFAMCode = 1
                    },
                }
            };
            var learnerFAMsQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(It.IsAny<IEnumerable<ILearnerFAM>>(), LearnerFAMTypeConstants.SEN, 1)).Returns(true);
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(It.IsAny<IEnumerable<ILearnerFAM>>(), LearnerFAMTypeConstants.EHC, 1)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerFAMsQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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
        public void BuilderErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, "1")).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.SEN)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearnerFAMTypeConstants.SEN, "1");

            validationErrorHandlerMock.Verify();
        }

        private LearnFAMType_14Rule NewRule(
            ILearnerFAMQueryService learnerFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnFAMType_14Rule(learnerFAMQueryService, validationErrorHandler);
        }
    }
}
