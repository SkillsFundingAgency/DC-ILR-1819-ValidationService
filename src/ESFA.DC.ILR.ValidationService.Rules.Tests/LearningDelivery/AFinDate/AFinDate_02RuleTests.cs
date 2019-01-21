using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AFindate_02RuleTests : AbstractRuleTests<AFinDate_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinDate_02");
        }

        [Fact]
        public void AFInDatesOneYearAfterProgramme_ReturnsSingleDate()
        {
            var learnPlanEndDate = new DateTime(2018, 8, 1);
            var aFinDate = new DateTime(2019, 10, 1);

            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = aFinDate
                },
                 new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 12, 1)
                }
            };

            var aFinDateResult = new List<DateTime> { aFinDate };

            NewRule().AFInDatesOneYearAfterProgramme(appFinRecords, learnPlanEndDate).Should().BeEquivalentTo(aFinDateResult);
        }

        [Fact]
        public void AFInDatesOneYearAfterProgramme_ReturnsMultipleDates()
        {
            var learnPlanEndDate = new DateTime(2018, 8, 1);
            var aFinDate1 = new DateTime(2019, 10, 1);
            var aFinDate2 = new DateTime(2019, 11, 1);

            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = aFinDate1
                },
                 new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = aFinDate2
                }
            };

            var aFinDateResult = new List<DateTime> { aFinDate1, aFinDate2 };

            NewRule().AFInDatesOneYearAfterProgramme(appFinRecords, learnPlanEndDate).Should().BeEquivalentTo(aFinDateResult);
        }

        [Fact]
        public void AFInDatesOneYearAfterProgramme_ReturnsEmptyList()
        {
            var learnPlanEndDate = new DateTime(2019, 8, 1);
            var aFinDate1 = new DateTime(2019, 10, 1);
            var aFinDate2 = new DateTime(2019, 11, 1);

            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = aFinDate1
                },
                 new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = aFinDate2
                }
            };

            NewRule().AFInDatesOneYearAfterProgramme(appFinRecords, learnPlanEndDate).Should().BeNullOrEmpty();
        }

        [Fact]
        public void Validate_NoError_NoDeliveries()
        {
            var learner = new TestLearner();

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoAppFinRecords()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                    },
                    new TestLearningDelivery()
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
        public void Validate_NoError_NullLearnPlanEndDate()
        {
            DateTime? dd19Result = null;

            var learner = new TestLearner
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
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                        }
                    },
                    new TestLearningDelivery()
                    {
                    }
                }
            };

            var dd19Mock = new Mock<IDerivedData_19Rule>();

            dd19Mock.Setup(dd => dd.Derive(learner.LearningDeliveries, learner.LearningDeliveries.FirstOrDefault())).Returns(dd19Result);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd19Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppFinRecordsCorrect()
        {
            DateTime? dd19Result = new DateTime(2019, 7, 31);

            var learner = new TestLearner
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
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                        }
                    },
                    new TestLearningDelivery()
                    {
                    }
                }
            };

            var dd19Mock = new Mock<IDerivedData_19Rule>();

            dd19Mock.Setup(dd => dd.Derive(learner.LearningDeliveries, learner.LearningDeliveries.FirstOrDefault())).Returns(dd19Result);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd19Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error()
        {
            DateTime? dd19Result = new DateTime(2017, 7, 31);

            var learner = new TestLearner
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
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                    }
                }
            };

            var dd19Mock = new Mock<IDerivedData_19Rule>();

            dd19Mock.Setup(dd => dd.Derive(learner.LearningDeliveries, learner.LearningDeliveries.FirstOrDefault())).Returns(dd19Result);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd19Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_MultupleAppFinRecords()
        {
            DateTime? dd19Result = new DateTime(2017, 7, 31);

            var learner = new TestLearner
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
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                             new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 8, 1)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                    }
                }
            };

            var dd19Mock = new Mock<IDerivedData_19Rule>();

            dd19Mock.Setup(dd => dd.Derive(learner.LearningDeliveries, learner.LearningDeliveries.FirstOrDefault())).Returns(dd19Result);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd19Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_MultupleDeliveries()
        {
            DateTime? dd19Result = new DateTime(2017, 7, 31);

            var learner = new TestLearner
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
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                             new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 8, 1)
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
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinDate = new DateTime(2018, 8, 1)
                            }
                        }
                    }
                }
            };

            var dd19Mock = new Mock<IDerivedData_19Rule>();

            dd19Mock.Setup(dd => dd.Derive(learner.LearningDeliveries, learner.LearningDeliveries.FirstOrDefault())).Returns(dd19Result);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd19Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AFinDate", "01/08/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("DD19", "DD19")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 8, 1), "DD19");

            validationErrorHandlerMock.Verify();
        }

        private AFinDate_02Rule NewRule(IDerivedData_19Rule dd19 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinDate_02Rule(dd19, validationErrorHandler);
        }
    }
}
