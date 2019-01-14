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
    public class LearnFAMType_15RuleTests : AbstractRuleTests<LearnFAMType_15Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnFAMType_15");
        }

        [Theory]
        [InlineData(true, false, true)]
        [InlineData(false, false, false)]
        [InlineData(false, true, false)]
        [InlineData(true, true, false)]
        public void ConditionMetMeetsExpectation(bool learnerFAMCodeForTypeExist, bool learnerFAMTypesExist, bool expectation)
        {
            TestLearner testLearner = new TestLearner
            {
                LearnerFAMs = new TestLearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMCode = 1,
                        LearnFAMType = LearnerFAMTypeConstants.ECF
                    }
                }
            };

            IEnumerable<string> learnerFAMTypes = new HashSet<string>() { LearnerFAMTypeConstants.SEN, LearnerFAMTypeConstants.EHC };
            var learnerFAMsQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(testLearner.LearnerFAMs, LearnerFAMTypeConstants.MCF, 1)).Returns(learnerFAMCodeForTypeExist);
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasAnyLearnerFAMTypes(testLearner.LearnerFAMs, learnerFAMTypes)).Returns(learnerFAMTypesExist);

            NewRule(learnerFAMQueryService: learnerFAMsQueryServiceMock.Object)
                .ConditionMet(testLearner.LearnerFAMs).Should().Be(expectation);
        }

        [Fact]
        public void Validate_Error()
        {
            TestLearner testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35
                    }
                },
                LearnerFAMs = new TestLearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMCode = 1,
                        LearnFAMType = LearnerFAMTypeConstants.MCF
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMCode = 1,
                        LearnFAMType = LearnerFAMTypeConstants.LSR
                    }
                }
            };

            IEnumerable<string> learnerFAMTypes = new HashSet<string>() { LearnerFAMTypeConstants.SEN, LearnerFAMTypeConstants.EHC };
            var learnerFAMsQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(testLearner.LearnerFAMs, LearnerFAMTypeConstants.MCF, 1)).Returns(true);
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasAnyLearnerFAMTypes(testLearner.LearnerFAMs, learnerFAMTypes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learnerFAMQueryService: learnerFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            TestLearner testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35
                    }
                },
                LearnerFAMs = new TestLearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMCode = 1,
                        LearnFAMType = LearnerFAMTypeConstants.MCF
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMCode = 2,
                        LearnFAMType = LearnerFAMTypeConstants.SEN
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMCode = 3,
                        LearnFAMType = LearnerFAMTypeConstants.EHC
                    }
                }
            };

            IEnumerable<string> learnerFAMTypes = new HashSet<string>() { LearnerFAMTypeConstants.SEN, LearnerFAMTypeConstants.EHC };
            var learnerFAMsQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(testLearner.LearnerFAMs, LearnerFAMTypeConstants.MCF, 1)).Returns(false);
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasAnyLearnerFAMTypes(testLearner.LearnerFAMs, learnerFAMTypes)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learnerFAMQueryService: learnerFAMsQueryServiceMock.Object).Validate(testLearner);
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
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, 1)).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.MCF)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearnerFAMTypeConstants.MCF, 1);

            validationErrorHandlerMock.Verify();
        }

        private LearnFAMType_15Rule NewRule(
            ILearnerFAMQueryService learnerFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnFAMType_15Rule(learnerFAMQueryService, validationErrorHandler);
        }
    }
}
