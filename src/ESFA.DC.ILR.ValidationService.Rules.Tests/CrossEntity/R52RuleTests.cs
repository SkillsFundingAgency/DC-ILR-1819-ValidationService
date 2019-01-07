using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R52RuleTests : AbstractRuleTests<R52Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R52");
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT)]
        [InlineData(LearningDeliveryFAMTypeConstants.ALB)]
        [InlineData(LearningDeliveryFAMTypeConstants.LSF)]
        public void ConditionMet_False(string learnDelFAMType)
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 654,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = learnDelFAMType,
                                LearnDelFAMCode = "32"
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.NSA,
                                LearnDelFAMCode = "21"
                            }
                        }
                    }
                }
            };

            NewRule().ConditionMet(testLearner).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 654,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB,
                                LearnDelFAMCode = "32"
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.NSA,
                                LearnDelFAMCode = "21"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 655,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.NSA,
                                LearnDelFAMCode = "21"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 656
                    }
                }
            };

            NewRule().ConditionMet(testLearner).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 654,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB,
                                LearnDelFAMCode = "32"
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.NSA,
                                LearnDelFAMCode = "21"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 655,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.NSA,
                                LearnDelFAMCode = "21"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 656
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 654,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                                LearnDelFAMCode = "32"
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        public R52Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R52Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
