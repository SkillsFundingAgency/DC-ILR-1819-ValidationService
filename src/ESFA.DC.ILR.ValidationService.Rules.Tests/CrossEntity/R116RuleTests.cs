using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R116RuleTests : AbstractRuleTests<R116Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R116");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(25).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(36).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(5).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Theory]
        [InlineData(38)]
        [InlineData(null)]
        public void ProgramTypeConditionMet_False(int? progType)
        {
            NewRule().ProgramTypeConditionMet(progType).Should().BeFalse();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(20)]
        [InlineData(21)]
        [InlineData(22)]
        [InlineData(23)]
        public void ProgramTypeConditionMet_True(int? progType)
        {
            NewRule().ProgramTypeConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(int.MaxValue, int.MaxValue)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(32000, 15000)]
        public void IsAFinCodeSumsDifferenceNegative_False(int aFinCode1And2, int aFinCode3)
        {
            NewRule().IsAFinCodeSumsDifferenceNegative(aFinCode1And2, aFinCode3).Should().BeFalse();
        }

        [Fact]
        public void IsAFinCodeSumsDifferenceNegative_True()
        {
            NewRule().IsAFinCodeSumsDifferenceNegative(3, 5).Should().BeTrue();
        }

        [Fact]
        public void GetSumsForAFinCode()
        {
            HashSet<int> aFinCodes1And2 = new HashSet<int> { 1, 2 };
            HashSet<int> aFinCodes3 = new HashSet<int> { 3 };

            var learningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "1A23456",
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
                                AFinAmount = 20
                            },
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 3,
                                AFinAmount = 5
                            },
                            new TestAppFinRecord()
                            {
                                AFinType = "TNP",
                                AFinCode = 3,
                                AFinAmount = 5
                            }
                        }
                    },
                        new TestLearningDelivery()
                        {
                            LearnAimRef = "2B852",
                            AppFinRecords = new TestAppFinRecord[]
                            {
                                new TestAppFinRecord()
                                {
                                    AFinType = "PMR",
                                    AFinCode = 2,
                                    AFinAmount = 30
                                }
                            }
                        }
                };

            NewRule().GetSumsForAFinCode(learningDeliveries, aFinCodes1And2).Should().Be(50);
            NewRule().GetSumsForAFinCode(learningDeliveries, aFinCodes3).Should().Be(5);
        }

        [Fact]
        public void GetSumsForAFinCode_NoMatch()
        {
            HashSet<int> aFinCodes1And2 = new HashSet<int> { 1, 2 };
            HashSet<int> aFinCodes3 = new HashSet<int> { 3 };

            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    LearnAimRef = "1A23456",
                    AppFinRecords = new TestAppFinRecord[]
                    {
                        new TestAppFinRecord()
                        {
                            AFinType = "TPR",
                            AFinCode = 1,
                            AFinAmount = 20
                        }
                    }
                }
            };

            NewRule().GetSumsForAFinCode(learningDeliveries, aFinCodes1And2).Should().Be(0);
            NewRule().GetSumsForAFinCode(learningDeliveries, aFinCodes3).Should().Be(0);
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "1A123456",
                        FundModel = 36,
                        AimType = 1,
                        ProgTypeNullable = 2,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1,
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
                                AFinAmount = 20
                            },
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 5,
                                AFinAmount = 5
                            },
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 3,
                                AFinAmount = 500
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "2C456789",
                        FundModel = 36,
                        AimType = 1,
                        ProgTypeNullable = 2,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1,
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 5,
                                AFinAmount = 30
                            },
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinAmount = 50
                            },
                            null
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "3C456789",
                        FundModel = 25,
                        AimType = 2,
                        ProgTypeNullable = 38,
                        FworkCodeNullable = 3,
                        PwayCodeNullable = 4,
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
                                AFinAmount = 20
                            },
                            new TestAppFinRecord()
                            {
                                AFinType = "TNP",
                                AFinCode = 2,
                                AFinAmount = 12000
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "4C456789",
                        FundModel = 45,
                        AimType = 5,
                        ProgTypeNullable = 38,
                        FworkCodeNullable = 3,
                        PwayCodeNullable = 4,
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "5C456789",
                        FundModel = 36,
                        AimType = 1,
                        ProgTypeNullable = 3,
                        FworkCodeNullable = 3,
                        PwayCodeNullable = 3,
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
                                AFinAmount = 300
                            },
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinAmount = 13000
                            },
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 3,
                                AFinAmount = 15000
                            }
                        }
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
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "1A123456",
                        FundModel = 36,
                        AimType = 1,
                        ProgTypeNullable = 2,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1,
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
                                AFinAmount = 20000
                            },
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 5,
                                AFinAmount = 5
                            },
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 3,
                                AFinAmount = 500
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "2C456789",
                        FundModel = 36,
                        AimType = 1,
                        ProgTypeNullable = 2,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1,
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 5,
                                AFinAmount = 30
                            },
                            new TestAppFinRecord()
                            {
                                AFinType = "PMR",
                                AFinCode = 2,
                                AFinAmount = 50
                            },
                            null
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_NoMatch()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "1A123456",
                        FundModel = 36,
                        AimType = 1,
                        ProgTypeNullable = 2,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1,
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = "TNP",
                                AFinCode = 4,
                                AFinAmount = 20000
                            },
                            new TestAppFinRecord()
                            {
                                AFinType = "TNP",
                                AFinCode = 6,
                                AFinAmount = 0
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "2A123456",
                        FundModel = 36,
                        AimType = 1,
                        ProgTypeNullable = 2,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1,
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "3A123456",
                        FundModel = 25,
                        AimType = 2,
                        ProgTypeNullable = 45,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(36)]
        [InlineData(25)]
        public void Validate_NoError_NoAppFinData(int fundModel)
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "1A123456",
                        FundModel = fundModel,
                        AimType = 1,
                        ProgTypeNullable = 2,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_NullCheck()
        {
            TestLearner testLearner = null;

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_LearningDeliveryNullCheck()
        {
            TestLearner testLearner = new TestLearner()
            {
                LearningDeliveries = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        public R116Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R116Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
