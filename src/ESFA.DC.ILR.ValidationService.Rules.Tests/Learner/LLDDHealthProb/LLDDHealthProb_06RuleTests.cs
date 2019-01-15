using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LLDDHealthProb
{
    public class LLDDHealthProb_06RuleTests : AbstractRuleTests<LLDDHealthProb_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LLDDHealthProb_06");
        }

        [Fact]
        public void LLDDHealthProbConditionMet_True()
        {
            var llddHealthProb = 1;

            NewRule().LLDDHealthProbConditionMet(llddHealthProb).Should().BeTrue();
        }

        [Fact]
        public void LLDDHealthProbConditionMet_False()
        {
            var llddHealthProb = 0;

            NewRule().LLDDHealthProbConditionMet(llddHealthProb).Should().BeFalse();
        }

        [Fact]
        public void LLDDRecordConditionMet_True()
        {
            IEnumerable<TestLLDDAndHealthProblem> llddAndHealthProblems = null;

            NewRule().LLDDRecordConditionMet(llddAndHealthProblems).Should().BeTrue();
        }

        [Fact]
        public void LLDDRecordConditionMet_False()
        {
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>()
            {
                new TestLLDDAndHealthProblem()
            };

            NewRule().LLDDRecordConditionMet(llddAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void Excluded_TrueFundModel()
        {
            DateTime? dateOfBirth = null;
            var dd06Date = new DateTime(2000, 10, 01);

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModel = 10
                }
            };

            NewRule().Excluded(learningDeliveries, dateOfBirth, dd06Date).Should().BeTrue();
        }

        [Fact]
        public void Excluded_TrueDeliveryFam()
        {
            DateTime? dateOfBirth = null;
            var dd06Date = new DateTime(2000, 10, 01);
            var famType = "SOF";
            var famCode = "108";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModel = 99,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = famType,
                            LearnDelFAMCode = famCode
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), famType, famCode))
                .Returns(true);

            NewRule(learningDeliveryFamQueryServiceMock.Object).Excluded(learningDeliveries, dateOfBirth, dd06Date).Should().BeTrue();
        }

        [Fact]
        public void Excluded_TrueDOB()
        {
            var dateOfBirth = new DateTime(1992, 10, 01);
            var dd06Date = new DateTime(2018, 10, 01);
            var famType = "XXX";
            var famCode = "000";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModel = 0,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = famType,
                            LearnDelFAMCode = famCode
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), famType, famCode))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, dd06Date)).Returns(26);

            NewRule(learningDeliveryFamQueryServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object)
                .Excluded(learningDeliveries, dateOfBirth, dd06Date).Should().BeTrue();
        }

        [Fact]
        public void Excluded_TrueMultiple()
        {
            var dateOfBirth = new DateTime(1992, 10, 01);
            var dd06Date = new DateTime(2018, 10, 01);
            var famType = "SOF";
            var famCode = "108";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModel = 99,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = famType,
                            LearnDelFAMCode = famCode
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), famType, famCode))
                .Returns(true);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, dd06Date)).Returns(26);

            NewRule(learningDeliveryFamQueryServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object)
                .Excluded(learningDeliveries, dateOfBirth, dd06Date).Should().BeTrue();
        }

        [Fact]
        public void ExcludedFundModelConditionMet_True()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModel = 10
                }
            };

            NewRule().ExcludedFundModelConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void ExcludedFundModelConditionMet_False()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModel = 0
                },
                new TestLearningDelivery()
                {
                    FundModel = 5
                }
            };

            NewRule().ExcludedFundModelConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ExcludedDeliveryFAMConditionMet_True()
        {
            var famType = "SOF";
            var famCode = "108";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModel = 99,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = famType,
                            LearnDelFAMCode = famCode
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), famType, famCode))
                .Returns(true);

            NewRule(learningDeliveryFamQueryServiceMock.Object).ExcludedDeliveryFAMConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void ExcludedDeliveryFAMConditionMet_False()
        {
            var famType = "XXX";
            var famCode = "000";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModel = 99,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = famType,
                            LearnDelFAMCode = famCode
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), famType, famCode))
                .Returns(false);

            NewRule(learningDeliveryFamQueryServiceMock.Object).ExcludedDeliveryFAMConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ExcludedDOBConditionMet_True()
        {
            var dateOfBirth = new DateTime(1992, 10, 01);
            var dd06Date = new DateTime(2018, 10, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, dd06Date)).Returns(26);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).ExcludedDOBConditionMet(dateOfBirth, dd06Date).Should().BeTrue();
        }

        [Fact]
        public void ExcludedDOBConditionMet_False()
        {
            var dateOfBirth = new DateTime(1994, 10, 01);
            var dd06Date = new DateTime(2018, 10, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, dd06Date)).Returns(24);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).ExcludedDOBConditionMet(dateOfBirth, dd06Date).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var llddHealthProb = 1;
            IEnumerable<TestLLDDAndHealthProblem> llddAndHealthProblems = null;
            var dateOfBirth = new DateTime(1994, 10, 01);
            var dd06Date = new DateTime(2018, 10, 01);
            var famType = "XXX";
            var famCode = "000";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModel = 0,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = famType,
                            LearnDelFAMCode = famCode
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), famType, famCode))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, dd06Date)).Returns(24);

            NewRule(learningDeliveryFamQueryServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object)
                .ConditionMet(llddHealthProb, llddAndHealthProblems, learningDeliveries, dateOfBirth, dd06Date)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseLLDDHealthProb()
        {
            var llddHealthProb = 0;
            IEnumerable<TestLLDDAndHealthProblem> llddAndHealthProblems = null;
            var dateOfBirth = new DateTime(1994, 10, 01);
            var dd06Date = new DateTime(2018, 10, 01);
            var famType = "XXX";
            var famCode = "000";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModel = 0,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = famType,
                            LearnDelFAMCode = famCode
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), famType, famCode))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, dd06Date)).Returns(24);

            NewRule(learningDeliveryFamQueryServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object)
                .ConditionMet(llddHealthProb, llddAndHealthProblems, learningDeliveries, dateOfBirth, dd06Date)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseLLDDRecord()
        {
            var llddHealthProb = 1;

            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>()
            {
                new TestLLDDAndHealthProblem()
            };

            var dateOfBirth = new DateTime(1994, 10, 01);
            var dd06Date = new DateTime(2018, 10, 01);
            var famType = "XXX";
            var famCode = "000";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModel = 0,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = famType,
                            LearnDelFAMCode = famCode
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), famType, famCode))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, dd06Date)).Returns(24);

            NewRule(learningDeliveryFamQueryServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object)
                .ConditionMet(llddHealthProb, llddAndHealthProblems, learningDeliveries, dateOfBirth, dd06Date)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseExcluded()
        {
            var llddHealthProb = 1;
            IEnumerable<TestLLDDAndHealthProblem> llddAndHealthProblems = null;
            var dateOfBirth = new DateTime(1994, 10, 01);
            var dd06Date = new DateTime(2018, 10, 01);
            var famType = "XXX";
            var famCode = "000";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModel = 10,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = famType,
                            LearnDelFAMCode = famCode
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), famType, famCode))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, dd06Date)).Returns(24);

            NewRule(learningDeliveryFamQueryServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object)
                .ConditionMet(llddHealthProb, llddAndHealthProblems, learningDeliveries, dateOfBirth, dd06Date)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var llddHealthProb = 1;
            var dateOfBirth = new DateTime(1994, 10, 01);
            var dd06Date = new DateTime(2018, 10, 01);
            var famType = "XXX";
            var famCode = "000";

            var learner = new TestLearner()
            {
                LLDDHealthProb = llddHealthProb,
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 0,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = famType,
                                LearnDelFAMCode = famCode
                            }
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), famType, famCode))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, dd06Date)).Returns(24);

            var dd06Mock = new Mock<IDerivedData_06Rule>();
            dd06Mock.Setup(dm => dm.Derive(It.IsAny<IEnumerable<ILearningDelivery>>())).Returns(dd06Date);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, dd06Mock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var llddHealthProb = 1;
            var dateOfBirth = new DateTime(1992, 10, 01);
            var dd06Date = new DateTime(2018, 10, 01);
            var famType = "XXX";
            var famCode = "000";

            var learner = new TestLearner()
            {
                LLDDHealthProb = llddHealthProb,
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 0,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = famType,
                                LearnDelFAMCode = famCode
                            }
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), famType, famCode))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, dd06Date)).Returns(26);

            var dd06Mock = new Mock<IDerivedData_06Rule>();
            dd06Mock.Setup(dm => dm.Derive(It.IsAny<IEnumerable<ILearningDelivery>>())).Returns(dd06Date);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, dd06Mock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        private LLDDHealthProb_06Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IDerivedData_06Rule dd06 = null,
            IDateTimeQueryService dateTimeQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LLDDHealthProb_06Rule(learningDeliveryFamQueryService, dd06, dateTimeQueryService, validationErrorHandler);
        }
    }
}
