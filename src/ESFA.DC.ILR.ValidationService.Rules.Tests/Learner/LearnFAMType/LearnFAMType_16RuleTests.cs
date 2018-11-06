using System.Collections.Generic;
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
    public class LearnFAMType_16RuleTests : AbstractRuleTests<LearnFAMType_16Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnFAMType_16");
        }

        [Fact]
        public void LearnerFAMsConditionMet_False()
        {
            TestLearner testLearner = new TestLearner
            {
                LearnerFAMs = new TestLearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMCode = 2,
                        LearnFAMType = LearnerFAMTypeConstants.DLA
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

            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(testLearner.LearnerFAMs, LearnerFAMTypeConstants.ECF, 1)).Returns(false);
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasAnyLearnerFAMTypes(testLearner.LearnerFAMs, learnerFAMTypes)).Returns(true);

            NewRule(learnerFAMQueryService: learnerFAMsQueryServiceMock.Object).LearnerFAMsConditionMet(testLearner.LearnerFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearnerFAMsCondtionMet_Null()
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

            var learnerFAMsQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(testLearner.LearnerFAMs, LearnerFAMTypeConstants.ECF, 1)).Returns(false);

            NewRule(learnerFAMQueryService: learnerFAMsQueryServiceMock.Object).LearnerFAMsConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearnerFAMsConditionMet_True()
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

            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(testLearner.LearnerFAMs, LearnerFAMTypeConstants.ECF, 1)).Returns(true);
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasAnyLearnerFAMTypes(testLearner.LearnerFAMs, learnerFAMTypes)).Returns(false);
            NewRule(learnerFAMQueryService: learnerFAMsQueryServiceMock.Object).LearnerFAMsConditionMet(testLearner.LearnerFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            TestLearner testLearner = new TestLearner
            {
                LearnerFAMs = new TestLearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMCode = 2,
                        LearnFAMType = LearnerFAMTypeConstants.DLA
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

            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(testLearner.LearnerFAMs, LearnerFAMTypeConstants.ECF, 1)).Returns(false);
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasAnyLearnerFAMTypes(testLearner.LearnerFAMs, learnerFAMTypes)).Returns(true);

            NewRule(learnerFAMQueryService: learnerFAMsQueryServiceMock.Object).LearnerFAMsConditionMet(testLearner.LearnerFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_Null()
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

            var learnerFAMsQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(testLearner.LearnerFAMs, LearnerFAMTypeConstants.ECF, 1)).Returns(false);

            NewRule(learnerFAMQueryService: learnerFAMsQueryServiceMock.Object).LearnerFAMsConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
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

            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(testLearner.LearnerFAMs, LearnerFAMTypeConstants.ECF, 1)).Returns(true);
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasAnyLearnerFAMTypes(testLearner.LearnerFAMs, learnerFAMTypes)).Returns(false);

            NewRule(learnerFAMQueryService: learnerFAMsQueryServiceMock.Object).LearnerFAMsConditionMet(testLearner.LearnerFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
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

            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(testLearner.LearnerFAMs, LearnerFAMTypeConstants.ECF, 1)).Returns(true);
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
                LearnerFAMs = new TestLearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMCode = 1,
                        LearnFAMType = LearnerFAMTypeConstants.ECF
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

            learnerFAMsQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(testLearner.LearnerFAMs, LearnerFAMTypeConstants.ECF, 1)).Returns(false);
            learnerFAMsQueryServiceMock.Setup(dd => dd.HasAnyLearnerFAMTypes(testLearner.LearnerFAMs, learnerFAMTypes)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learnerFAMQueryService: learnerFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuilderErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, "1")).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.ECF)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearnerFAMTypeConstants.ECF, "1");

            validationErrorHandlerMock.Verify();
        }

        private LearnFAMType_16Rule NewRule(
            ILearnerFAMQueryService learnerFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnFAMType_16Rule(learnerFAMQueryService, validationErrorHandler);
        }
    }
}
