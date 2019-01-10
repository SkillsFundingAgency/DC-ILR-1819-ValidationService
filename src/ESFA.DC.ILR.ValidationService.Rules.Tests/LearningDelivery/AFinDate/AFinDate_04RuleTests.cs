using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinDate
{
    public class AFinDate_04RuleTests : AbstractRuleTests<AFinDate_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinDate_04");
        }

        [Fact]
        public void LearnActEndDateIsKnown_True()
        {
            NewRule().LearnActEndDateIsKnown(new DateTime(2018, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateIsKnown_False()
        {
            NewRule().LearnActEndDateIsKnown(null).Should().BeFalse();
        }

        [Fact]
        public void TNPRecordAfterLearnActEndDate_Returns()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNPRecordAfterLearnActEndDate(new DateTime(2018, 8, 1), appFinRecords).Should().BeEquivalentTo(appFinRecords[0]);
        }

        [Fact]
        public void TNPRecordAfterLearnActEndDate_Multiple_Returns()
        {
            var appFinRecords = new List<TestAppFinRecord>
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
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNPRecordAfterLearnActEndDate(new DateTime(2018, 8, 1), appFinRecords).Should().BeEquivalentTo(appFinRecords[0]);
        }

        [Fact]
        public void TNPRecordAfterLearnActEndDate_Null_DateMisMatch()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 8, 1)
                }
            };

            NewRule().TNPRecordAfterLearnActEndDate(new DateTime(2018, 9, 1), appFinRecords).Should().BeNull();
        }

        [Fact]
        public void TNPRecordAfterLearnActEndDate_Null_AFinTypeMisMatch()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNPRecordAfterLearnActEndDate(new DateTime(2018, 8, 1), appFinRecords).Should().BeNull();
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
                        LearnActEndDateNullable = new DateTime(2018, 8, 1),
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
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
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
        public void Validate_NoError_NullLearnActEndDate()
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
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
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
        public void Validate_NoError_NullAppFinRecords()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2018, 8, 1)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AFinDate_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinDate_04Rule(validationErrorHandler);
        }
    }
}
