using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R75RuleTests : AbstractRuleTests<R75Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R75");
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            NewRule().ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AppFinRecordsNull()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 20,
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 20
                },
            };

            NewRule().ConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NotPMR()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 20,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinAmount = 10,
                            AFinType = "XYZ"
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinAmount = 5,
                            AFinType = "XYZ"
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 3,
                            AFinAmount = 20,
                            AFinType = "XYZ"
                        }
                    }
                }
            };

            NewRule().ConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void Validate_NoError_Null_StandardCode()
        {
            var learner = new TestLearner
            {
                LearnRefNumber = "12345",
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = null,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinAmount = 10,
                            AFinType = "PMR"
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinAmount = 10,
                            AFinType = "PMR"
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 3,
                            AFinAmount = 20,
                            AFinType = "PMR"
                        }
                    }
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = null,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinAmount = 5,
                            AFinType = "PMR"
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 3,
                            AFinAmount = 100,
                            AFinType = "PMR"
                        }
                    }
                }
            }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 20,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinAmount = 10,
                            AFinType = "PMR"
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinAmount = 10,
                            AFinType = "PMR"
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 3,
                            AFinAmount = 20,
                            AFinType = "PMR"
                        }
                    }
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 20,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinAmount = 5,
                            AFinType = "PMR"
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 3,
                            AFinAmount = 4,
                            AFinType = "PMR"
                        }
                    }
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 100,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinAmount = 1000,
                            AFinType = "PMR"
                        }
                    }
                }
            };

            NewRule().ConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 20,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinAmount = 10,
                            AFinType = "PMR"
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinAmount = 10,
                            AFinType = "PMR"
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 3,
                            AFinAmount = 20,
                            AFinType = "PMR"
                        }
                    }
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 20,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinAmount = 5,
                            AFinType = "PMR"
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 3,
                            AFinAmount = 26,
                            AFinType = "PMR"
                        }
                    }
                }
            };

            NewRule().ConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                LearnRefNumber = "12345",
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                        AimType = 1,
                        StdCodeNullable = 20,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 1,
                                AFinAmount = 10,
                                AFinType = "PMR"
                            },
                            new TestAppFinRecord()
                            {
                                AFinCode = 2,
                                AFinAmount = 10,
                                AFinType = "PMR"
                            },
                            new TestAppFinRecord()
                            {
                                AFinCode = 3,
                                AFinAmount = 20,
                                AFinType = "PMR"
                            }
                        }
                    },
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                        AimType = 1,
                        StdCodeNullable = 20,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 1,
                                AFinAmount = 5,
                                AFinType = "PMR"
                            },
                            new TestAppFinRecord()
                            {
                                AFinCode = 3,
                                AFinAmount = 26,
                                AFinType = "PMR"
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.Handle("R75", "12345", null, null));
            }
        }

        [Fact]
        public void Validate_NoError_CompAim()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                        AimType = 1,
                        StdCodeNullable = 20,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 1,
                                AFinAmount = 10,
                                AFinType = "PMR"
                            },
                            new TestAppFinRecord()
                            {
                                AFinCode = 2,
                                AFinAmount = 10,
                                AFinType = "PMR"
                            },
                            new TestAppFinRecord()
                            {
                                AFinCode = 3,
                                AFinAmount = 20,
                                AFinType = "PMR"
                            }
                        }
                    },
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                        AimType = 1,
                        StdCodeNullable = 20,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 1,
                                AFinAmount = 5,
                                AFinType = "PMR"
                            },
                            new TestAppFinRecord()
                            {
                                AFinCode = 3,
                                AFinAmount = 26,
                                AFinType = "PMR"
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_ProgAim()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                        FundModel = 35,
                        AimType = 3,
                        StdCodeNullable = 1
                    },
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 24,
                        FundModel = 35,
                        AimType = 3,
                        StdCodeNullable = 1
                    },
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                        FundModel = 35,
                        AimType = 1,
                        StdCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 1,
                                AFinAmount = 20,
                                AFinType = "PMR"
                            },
                            new TestAppFinRecord()
                            {
                                AFinCode = 1,
                                AFinAmount = 30,
                                AFinType = "PMR"
                            },
                            new TestAppFinRecord()
                            {
                                AFinCode = 3,
                                AFinAmount = 49,
                                AFinType = "PMR"
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        public R75Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R75Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
