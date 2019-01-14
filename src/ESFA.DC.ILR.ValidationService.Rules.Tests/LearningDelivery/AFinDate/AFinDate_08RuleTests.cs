using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinDate
{
    public class AFinDate_08RuleTests : AbstractRuleTests<AFinDate_08Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinDate_08");
        }

        [Theory]
        [InlineData(4, 2)]
        [InlineData(2, 4)]
        public void TNP2And4Exists_True(int aFinCode1, int aFinCode2)
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = aFinCode1,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = aFinCode2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNP2And4Exists(appFinRecords).Should().BeTrue();
        }

        [Theory]
        [InlineData("TNP", "TNP", 2, 2)]
        [InlineData("TNP", "TNP", 4, 4)]
        [InlineData("TNP", "TNP", 2, 3)]
        [InlineData("TNP", "PMR", 2, 4)]
        [InlineData("TNP", "PMR", 4, 2)]
        [InlineData("PMR", "TNP", 2, 4)]
        [InlineData("PMR", "TNP", 4, 2)]
        [InlineData("PMR", "PMR", 4, 2)]
        public void TNP2And4Exists_False(string aFinType1, string aFinType2, int aFinCode1, int aFinCode2)
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = aFinType1,
                    AFinCode = aFinCode1,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = aFinType2,
                    AFinCode = aFinCode2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNP2And4Exists(appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void TNP2And4Exists_False_Null()
        {
            NewRule().TNP2And4Exists(null).Should().BeFalse();
        }

        [Fact]
        public void TNP2DateEqualToTNP4Date_ReturnsEntity()
        {
            var tnp2Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 2,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var tnp4Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp2Entity,
                tnp4Entity
            };

            NewRule().TNP2DateEqualToTNP4Date(appFinRecords).Should().Be(tnp2Entity);
        }

        [Fact]
        public void TNP2DateEqualToTNP4Date_MultipleTNP4ReturnsNull()
        {
            var tnp2Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 2,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var tnp4EntityOne = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var tnp4EntityTwo = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 11, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp2Entity,
                tnp4EntityOne,
                tnp4EntityTwo
            };

            NewRule().TNP2DateEqualToTNP4Date(appFinRecords).Should().BeNull();
        }

        [Fact]
        public void TNP2DateEqualToTNP4Date_MultipleTNP4ReturnsEntity()
        {
            var tnp2Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 2,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var tnp4EntityOne = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var tnp4EntityTwo = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp2Entity,
                tnp4EntityOne,
                tnp4EntityTwo
            };

            NewRule().TNP2DateEqualToTNP4Date(appFinRecords).Should().Be(tnp2Entity);
        }

        [Fact]
        public void TNP2DateEqualToTNP4Date_MisMatchTypesReturnsNull()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNP2DateEqualToTNP4Date(appFinRecords).Should().BeNull();
        }

        [Fact]
        public void TNP2DateEqualToTNP4Date_MisMatchDatesReturnsNull()
        {
            var appFinRecords = new List<TestAppFinRecord>
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
                }
            };

            NewRule().TNP2DateEqualToTNP4Date(appFinRecords).Should().BeNull();
        }

        [Fact]
        public void TNP2DateEqualToTNP4Date_NoTNP4ReturnsNull()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNP2DateEqualToTNP4Date(appFinRecords).Should().BeNull();
        }

        [Fact]
        public void TNP2DateEqualToTNP4Date_NoEntitiesReturnsNull()
        {
            NewRule().TNP2DateEqualToTNP4Date(null).Should().BeNull();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
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

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_MultipleDeliveriesTrigger()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 4,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
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
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 10, 1)
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

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
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
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
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
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppFinRecordsNoTNP4()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
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
                                AFinType = "PMR",
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

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppFinRecordsDateMisMatch()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
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

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AFinDate_08Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinDate_08Rule(validationErrorHandler);
        }
    }
}
