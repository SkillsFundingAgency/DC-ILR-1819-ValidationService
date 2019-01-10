using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R104RuleTests : AbstractRuleTests<R104Rule>
    {
        private readonly string _famTypeACT = LearningDeliveryFAMTypeConstants.ACT;

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R104");
        }

        [Fact]
        public void ACTCountConditionMet_True()
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
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "ACT"
                }
            };

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(learningDeliveryFams, _famTypeACT)).Returns(2);

            NewRule(learningeliveryFAMQueryServiceMock.Object).ACTCountConditionMet(learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void ACTCountConditionMet_False()
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
                    LearnDelFAMType = "ACT"
                }
            };

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(learningDeliveryFams, _famTypeACT)).Returns(1);

            NewRule(learningeliveryFAMQueryServiceMock.Object).ACTCountConditionMet(learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ACTCountConditionMet_False_NullFAMS()
        {
            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(null, _famTypeACT)).Returns(null);

            NewRule(learningeliveryFAMQueryServiceMock.Object).ACTCountConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ACTDateConditionMet_ReturnsFAM()
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
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 10),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 10),
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 30),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 10, 1),
                    LearnDelFAMType = "ACT"
                }
            };

            NewRule().ACTDateConditionMet(learningDeliveryFams).Should().Be(learningDeliveryFams[1]);
        }

        [Fact]
        public void ACTDateConditionMet_ReturnsFAM_Multiple()
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
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 10),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 10),
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 30),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 30),
                    LearnDelFAMType = "ACT"
                }
            };

            NewRule().ACTDateConditionMet(learningDeliveryFams).Should().Be(learningDeliveryFams[1]);
        }

        [Fact]
        public void ACTDateConditionMet_ReturnsNull_NoFAMS()
        {
            NewRule().ACTDateConditionMet(null).Should().BeNull();
        }

        [Fact]
        public void ACTDateConditionMet_ReturnsNull_ZeroACTTypes()
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
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 10),
                    LearnDelFAMType = "RES"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 10),
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 30),
                    LearnDelFAMType = "RES"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 10, 1),
                    LearnDelFAMType = "RES"
                }
            };

            NewRule().ACTDateConditionMet(learningDeliveryFams).Should().BeNull();
        }

        [Fact]
        public void ACTDateConditionMet_ReturnsNull_FamDateToNull()
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
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 10),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 10, 1),
                    LearnDelFAMType = "ACT"
                }
            };

            NewRule().ACTDateConditionMet(learningDeliveryFams).Should().BeNull();
        }

        [Fact]
        public void ACTDateConditionMet_ReturnsNull_FAMDatesCorrect()
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
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 10),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 11),
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 30),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 10, 1),
                    LearnDelFAMType = "ACT"
                }
            };

            NewRule().ACTDateConditionMet(learningDeliveryFams).Should().BeNull();
        }

        [Fact]
        public void ACTDateConditionMet_ReturnsNull_SingleFAM()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 10),
                    LearnDelFAMType = "ACT"
                }
            };

            NewRule().ACTDateConditionMet(learningDeliveryFams).Should().BeNull();
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
                        AimSeqNumber = 1,
                        LearnActEndDateNullable = new DateTime(2018, 10, 1),
                        LearnPlanEndDate = new DateTime(2018, 10, 1),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                                LearnDelFAMType = "LSF"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                                LearnDelFAMDateToNullable = new DateTime(2018, 9, 10),
                                LearnDelFAMType = "ACT"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 10),
                                LearnDelFAMDateToNullable = new DateTime(2018, 9, 30),
                                LearnDelFAMType = "ACT"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 10, 1),
                                LearnDelFAMType = "ACT"
                            }
                        }
                    }
                }
            };

            var ldFams = learner.LearningDeliveries.Select(ld => ld.LearningDeliveryFAMs).First();

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(ldFams, _famTypeACT)).Returns(3);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_MultipleDeliveries()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        LearnActEndDateNullable = new DateTime(2018, 10, 1),
                        LearnPlanEndDate = new DateTime(2018, 10, 1),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                                LearnDelFAMType = "LSF"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                                LearnDelFAMDateToNullable = new DateTime(2018, 9, 10),
                                LearnDelFAMType = "ACT"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 10),
                                LearnDelFAMDateToNullable = new DateTime(2018, 9, 30),
                                LearnDelFAMType = "ACT"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 10, 1),
                                LearnDelFAMType = "ACT"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        LearnActEndDateNullable = new DateTime(2018, 10, 1),
                        LearnPlanEndDate = new DateTime(2018, 10, 1),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                                LearnDelFAMType = "LSF"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                                LearnDelFAMDateToNullable = new DateTime(2018, 9, 10),
                                LearnDelFAMType = "ACT"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 11),
                                LearnDelFAMDateToNullable = new DateTime(2018, 9, 30),
                                LearnDelFAMType = "ACT"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2018, 10, 1),
                                LearnDelFAMType = "ACT"
                            }
                        }
                    }
                }
            };

            var ldFams = learner.LearningDeliveries.Select(ld => ld.LearningDeliveryFAMs).First();

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(ldFams, _famTypeACT)).Returns(3);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullLearningDeliveries()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
            };

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(null, _famTypeACT)).Returns(0);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_ACTCount()
        {
            var learningDeliveryFams = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "LSF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 10, 1),
                    LearnDelFAMType = "ACT"
                }
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        LearnActEndDateNullable = new DateTime(2018, 10, 1),
                        LearnPlanEndDate = new DateTime(2018, 10, 1),
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var ldFams = learner.LearningDeliveries.Select(ld => ld.LearningDeliveryFAMs).First();

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(ldFams, _famTypeACT)).Returns(1);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_ACTDates()
        {
            var learningDeliveryFams = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "LSF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 10),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 11),
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 30),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 10, 1),
                    LearnDelFAMType = "ACT"
                }
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        LearnActEndDateNullable = new DateTime(2018, 10, 1),
                        LearnPlanEndDate = new DateTime(2018, 10, 1),
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var ldFams = learner.LearningDeliveries.Select(ld => ld.LearningDeliveryFAMs).First();

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(ldFams, _famTypeACT)).Returns(3);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var learnPlanEndDate = new DateTime(2018, 8, 1);
            var learnActEndDate = new DateTime(2018, 8, 1);
            var learnDelFamDateFrom = new DateTime(2018, 8, 1);
            var learnDelFamDateTo = new DateTime(2018, 8, 1);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnPlanEndDate", "01/08/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/08/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", _famTypeACT)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMDateFrom", "01/08/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMDateTo", "01/08/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnPlanEndDate, learnActEndDate, _famTypeACT, learnDelFamDateFrom, learnDelFamDateTo);

            validationErrorHandlerMock.Verify();
        }

        public R104Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R104Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
