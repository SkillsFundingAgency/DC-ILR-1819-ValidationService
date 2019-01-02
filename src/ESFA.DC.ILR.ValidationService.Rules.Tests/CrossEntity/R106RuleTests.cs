using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R106RuleTests : AbstractRuleTests<R106Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R106");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "LSF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                    LearnDelFAMType = "LSF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "ACT"
                }
            };

            NewRule().ConditionMet(learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FAMType()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                    LearnDelFAMType = "LSF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "ACT"
                }
            };

            NewRule().ConditionMet(learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DateFrom()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2015, 8, 1),
                    LearnDelFAMType = "LSF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2015, 9, 1),
                    LearnDelFAMType = "LSF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2015, 8, 1),
                    LearnDelFAMType = "ACT"
                }
            };

            NewRule().ConditionMet(learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            NewRule().ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                                LearnDelFAMType = "LSF"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                                LearnDelFAMType = "ACT"
                            }
                        },
                    },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                                LearnDelFAMType = "LSF"
                            },
                        },
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                                LearnDelFAMType = "RES"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                                LearnDelFAMType = "ACT"
                            }
                        },
                    },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                                LearnDelFAMType = "LSF"
                            },
                        },
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoFAMs()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        public R106Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R106Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
