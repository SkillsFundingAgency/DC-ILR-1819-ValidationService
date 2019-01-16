using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_30RuleTests : AbstractRuleTests<DateOfBirth_30Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_30");
        }

        [Theory]
        [InlineData(25)]
        [InlineData(35)]
        [InlineData(81)]
        [InlineData(82)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(99).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(25)).Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(25).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_True_Null()
        {
            int? progType = null;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(24)).Returns(true);

            NewRule(dd07Mock.Object).DD07ConditionMet(24).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_True()
        {
            var dateOfBirth = new DateTime(1998, 01, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(20);

            NewRule(
                academicYearDataService: academicYearDataServiceMock.Object,
                dateTimeQueryService: dateTimeQueryServiceMock.Object)
                .DateOfBirthConditionMet(dateOfBirth).Should().BeTrue();
        }

        [Theory]
        [InlineData("2000-01-01", 18)]
        [InlineData("1990-01-01", 28)]
        public void DateOfBirthConditionMet_False(string dobString, int mockAge)
        {
            var dateOfBirth = DateTime.Parse(dobString);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(mockAge);

            NewRule(
               academicYearDataService: academicYearDataServiceMock.Object,
               dateTimeQueryService: dateTimeQueryServiceMock.Object)
               .DateOfBirthConditionMet(dateOfBirth).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_False_Null()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst()));

            NewRule(
               academicYearDataService: academicYearDataServiceMock.Object,
               dateTimeQueryService: dateTimeQueryServiceMock.Object)
               .DateOfBirthConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearnerFAMConditionMet_True()
        {
            var learnerFAMs = new List<TestLearnerFAM>
            {
                new TestLearnerFAM
                {
                    LearnFAMType = "EHC",
                    LearnFAMCode = 1
                },
                new TestLearnerFAM
                {
                    LearnFAMType = "LDM",
                    LearnFAMCode = 2
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMQueryServiceMock.Setup(qs => qs.HasLearnerFAMType(learnerFAMs, "EHC")).Returns(true);

            NewRule(learnerFAMQueryService: learnerFAMQueryServiceMock.Object).LearnerFAMConditionMet(learnerFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearnerFAMConditionMet_False()
        {
            var learnerFAMs = new List<TestLearnerFAM>
            {
                new TestLearnerFAM
                {
                    LearnFAMType = "SOF",
                    LearnFAMCode = 1
                },
                new TestLearnerFAM
                {
                    LearnFAMType = "LDM",
                    LearnFAMCode = 2
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMQueryServiceMock.Setup(qs => qs.HasLearnerFAMType(learnerFAMs, "EHC")).Returns(false);

            NewRule(learnerFAMQueryService: learnerFAMQueryServiceMock.Object).LearnerFAMConditionMet(learnerFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "100"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "107")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_SOF()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "107")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_SOF107()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "107"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "107")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_LDM034()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "034"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "100"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = 24;
            var fundModel = 35;
            var dateOfBirth = new DateTime(1998, 01, 01);

            var learnerFAMs = new List<TestLearnerFAM>
            {
                new TestLearnerFAM
                {
                    LearnFAMType = "EHC",
                    LearnFAMCode = 1
                }
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "108"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "300"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(20);
            learnerFAMQueryServiceMock.Setup(qs => qs.HasLearnerFAMType(learnerFAMs, "EHC")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            NewRule(
                dd07Mock.Object,
                academicYearDataServiceMock.Object,
                dateTimeQueryServiceMock.Object,
                learnerFAMQueryServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, dateOfBirth, learnerFAMs, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var progType = 24;
            var fundModel = 99;
            var dateOfBirth = new DateTime(1998, 01, 01);

            var learnerFAMs = new List<TestLearnerFAM>
            {
                new TestLearnerFAM
                {
                    LearnFAMType = "EHC",
                    LearnFAMCode = 1
                }
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "108"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "300"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(20);
            learnerFAMQueryServiceMock.Setup(qs => qs.HasLearnerFAMType(learnerFAMs, "EHC")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            NewRule(
                dd07Mock.Object,
                academicYearDataServiceMock.Object,
                dateTimeQueryServiceMock.Object,
                learnerFAMQueryServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, dateOfBirth, learnerFAMs, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DD07()
        {
            var progType = 25;
            var fundModel = 35;
            var dateOfBirth = new DateTime(1998, 01, 01);

            var learnerFAMs = new List<TestLearnerFAM>
            {
                new TestLearnerFAM
                {
                    LearnFAMType = "EHC",
                    LearnFAMCode = 1
                }
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "108"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "300"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(20);
            learnerFAMQueryServiceMock.Setup(qs => qs.HasLearnerFAMType(learnerFAMs, "EHC")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            NewRule(
                dd07Mock.Object,
                academicYearDataServiceMock.Object,
                dateTimeQueryServiceMock.Object,
                learnerFAMQueryServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, dateOfBirth, learnerFAMs, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DOB()
        {
            var progType = 24;
            var fundModel = 35;
            var dateOfBirth = new DateTime(2000, 01, 01);

            var learnerFAMs = new List<TestLearnerFAM>
            {
                new TestLearnerFAM
                {
                    LearnFAMType = "EHC",
                    LearnFAMCode = 1
                }
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "108"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "300"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(18);
            learnerFAMQueryServiceMock.Setup(qs => qs.HasLearnerFAMType(learnerFAMs, "EHC")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            NewRule(
                dd07Mock.Object,
                academicYearDataServiceMock.Object,
                dateTimeQueryServiceMock.Object,
                learnerFAMQueryServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, dateOfBirth, learnerFAMs, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnerFAM()
        {
            var progType = 24;
            var fundModel = 35;
            var dateOfBirth = new DateTime(1998, 01, 01);

            var learnerFAMs = new List<TestLearnerFAM>
            {
                new TestLearnerFAM
                {
                    LearnFAMType = "LDM",
                    LearnFAMCode = 1
                }
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "108"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "300"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(20);
            learnerFAMQueryServiceMock.Setup(qs => qs.HasLearnerFAMType(learnerFAMs, "EHC")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            NewRule(
                dd07Mock.Object,
                academicYearDataServiceMock.Object,
                dateTimeQueryServiceMock.Object,
                learnerFAMQueryServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, dateOfBirth, learnerFAMs, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearningDeliveryFAM()
        {
            var progType = 24;
            var fundModel = 35;
            var dateOfBirth = new DateTime(1998, 01, 01);

            var learnerFAMs = new List<TestLearnerFAM>
            {
                new TestLearnerFAM
                {
                    LearnFAMType = "EHC",
                    LearnFAMCode = 1
                }
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "108"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "034"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(20);
            learnerFAMQueryServiceMock.Setup(qs => qs.HasLearnerFAMType(learnerFAMs, "EHC")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(true);

            NewRule(
                dd07Mock.Object,
                academicYearDataServiceMock.Object,
                dateTimeQueryServiceMock.Object,
                learnerFAMQueryServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, dateOfBirth, learnerFAMs, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(1990, 01, 01);
            var fundModel = 35;
            var progType = 24;

            var learnerFAMs = new List<TestLearnerFAM>
            {
                new TestLearnerFAM
                {
                    LearnFAMType = "EHC",
                    LearnFAMCode = 1
                }
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "108"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "300"
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearnerFAMs = learnerFAMs,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(20);
            learnerFAMQueryServiceMock.Setup(qs => qs.HasLearnerFAMType(learnerFAMs, "EHC")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    dd07Mock.Object,
                    academicYearDataServiceMock.Object,
                    dateTimeQueryServiceMock.Object,
                    learnerFAMQueryServiceMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var progType = 25;

            var learnerFAMs = new List<TestLearnerFAM>
            {
                new TestLearnerFAM
                {
                    LearnFAMType = "EHC",
                    LearnFAMCode = 1
                }
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "107"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "300"
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearnerFAMs = learnerFAMs,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 35,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(18);
            learnerFAMQueryServiceMock.Setup(qs => qs.HasLearnerFAMType(learnerFAMs, "EHC")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dd07Mock.Object,
                    academicYearDataServiceMock.Object,
                    dateTimeQueryServiceMock.Object,
                    learnerFAMQueryServiceMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                .Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("DateOfBirth", "01/01/2000")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 35)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2000, 01, 01), 35);

            validationErrorHandlerMock.Verify();
        }

        private DateOfBirth_30Rule NewRule(IDerivedData_07Rule dd07 = null, IAcademicYearDataService academicYearDataService = null, IDateTimeQueryService dateTimeQueryService = null, ILearnerFAMQueryService learnerFAMQueryService = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_30Rule(dd07, academicYearDataService, dateTimeQueryService, learnerFAMQueryService, learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
