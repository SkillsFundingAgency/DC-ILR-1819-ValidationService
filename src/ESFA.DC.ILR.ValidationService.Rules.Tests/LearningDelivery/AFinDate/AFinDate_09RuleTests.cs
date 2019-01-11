using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinDate
{
    public class AFinDate_09RuleTests : AbstractRuleTests<AFinDate_09Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinDate_09");
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(1, 3)]
        [InlineData(1, 20)]
        [InlineData(1, 21)]
        [InlineData(1, 22)]
        [InlineData(1, 23)]
        [InlineData(1, 25)]
        public void IsAppsStandardOrFramework_True(int aimType, int? progType)
        {
            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).IsAppsStandardOrFramework(aimType, progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(2, 2, true)]
        [InlineData(1, 99, false)]
        [InlineData(1, null, false)]
        public void IsAppsStandardOrFramework_False(int aimType, int? progType, bool mockValue)
        {
            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(mockValue);

            NewRule(dd07Mock.Object).IsAppsStandardOrFramework(aimType, progType).Should().BeFalse();
        }

        [Fact]
        public void AFinRecordWithDateLessThanLearnStartDate_ReturnsEntity()
        {
            var learnStartDate = new DateTime(2018, 9, 1);

            var appFinRecord = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 1,
                AFinDate = new DateTime(2017, 8, 31)
            };
            NewRule().AFinRecordWithDateLessThanLearnStartDate(learnStartDate, appFinRecord).Should().Be(appFinRecord);
        }

        [Theory]
        [InlineData("2018-08-01")]
        [InlineData("2018-10-01")]
        [InlineData("2017-08-01")]
        public void AFinRecordWithDateLessThanLearnStartDate_ReturnsNull(string aFinDateString)
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var appFinRecord = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 1,
                AFinDate = DateTime.Parse(aFinDateString)
            };

            NewRule().AFinRecordWithDateLessThanLearnStartDate(learnStartDate, appFinRecord).Should().BeNull();
        }

        [Fact]
        public void Validate_Error()
        {
            var progType = 2;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2015, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 4,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 11, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 4,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_MultipleDeliveries()
        {
            var progType = 2;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2015, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 4,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2015, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 4,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullLearningDeliveries()
        {
            var learner = new TestLearner()
            {
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullAppFinRecords()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                    },
                    new TestLearningDelivery
                    {
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppFinRecordsDateMisMatch()
        {
            var progType = 2;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 4,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_IsNotStandardOrFramework_AimType()
        {
            var progType = 2;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 2,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2015, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 2,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 4,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_IsNotStandardOrFramework_ProgType()
        {
            var progType = 90;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2015, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 4,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AFinDate_09Rule NewRule(IDD07 dd07 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinDate_09Rule(dd07, validationErrorHandler);
        }
    }
}
