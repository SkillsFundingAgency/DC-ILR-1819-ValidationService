using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_20RuleTests : AbstractRuleTests<DateOfBirth_20Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_20");
        }

        [Fact]
        public void ProgTypeConditionMet_True()
        {
            NewRule().ProgTypeConditionMet(25).Should().BeTrue();
        }

        [Fact]
        public void ProgTypeConditionMet_True_Null()
        {
            NewRule().ProgTypeConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void ProgTypeConditionMet_False()
        {
            NewRule().ProgTypeConditionMet(24).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(25).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(99).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_True()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(18);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth).Should().BeTrue();
        }

        [Fact]
        public void DateOfBirthConditionMet_False()
        {
            var dateOfBirth = new DateTime(1990, 01, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(28);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_False_Null()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst()));

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(null).Should().BeFalse();
        }

        [Theory]
        [InlineData("SOF", "108", "LDM", "108")]
        [InlineData("SOF", "347", "SOF", "108")]
        public void LearningDeliveryFAMConditionMet_True(string famType1, string famCode1, string famType2, string famCode2)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType1,
                    LearnDelFAMCode = famCode1
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType2,
                    LearnDelFAMCode = famCode2
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "107")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData("SOF", "108", "SOF", "107", true, true)]
        [InlineData("LDM", "347", "SOF", "107", true, true)]
        [InlineData("LDM", "108", "LDM", "107", false, false)]
        public void LearningDeliveryFAMConditionMet_False(string famType1, string famCode1, string famType2, string famCode2, bool mock1, bool mock2)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType1,
                    LearnDelFAMCode = famCode1
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType2,
                    LearnDelFAMCode = famCode2
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(mock1);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "107")).Returns(mock2);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(24, 82, "2000-01-01", 18, "SOF", "108", "SOF", "109", true, false)]
        [InlineData(1, 70, "2000-01-01", 18, "SOF", "108", "SOF", "109", true, false)]
        [InlineData(1, 82, "1990-01-01", 28, "SOF", "108", "SOF", "109", true, false)]
        [InlineData(1, 82, null, 0, "SOF", "108", "SOF", "109", true, false)]
        [InlineData(1, 82, "2000-01-01", 18, "SOF", "108", "SOF", "107", true, true)]
        [InlineData(1, 82, "2000-01-01", 18, "LDM", "347", "SOF", "107", true, true)]
        [InlineData(1, 82, "2000-01-01", 18, "LDM", "108", "LDM", "107", false, false)]
        public void ConditionMet_False(int progType, int fundModel, string dob, int dtqsReturn, string famType1, string famCode1, string famType2, string famCode2, bool mock1, bool mock2)
        {
            DateTime dateOfBirthMock = dob == null ? new DateTime(1900, 01, 01) : DateTime.Parse(dob);
            DateTime? dateOfBirth = dob == null ? (DateTime?)null : DateTime.Parse(dob);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType1,
                    LearnDelFAMCode = famCode1
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType2,
                    LearnDelFAMCode = famCode2
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirthMock, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(dtqsReturn);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(mock1);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "107")).Returns(mock2);

            NewRule(
                academicYearDataServiceMock.Object,
                dateTimeQueryServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(progType, fundModel, dateOfBirth, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = 1;
            var fundModel = 82;
            var dateOfBirth = new DateTime(2000, 01, 01);

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

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst()));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "107")).Returns(false);

            NewRule(
                academicYearDataServiceMock.Object,
                dateTimeQueryServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(progType, fundModel, dateOfBirth, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
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
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 82,
                        ProgTypeNullable = 1,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(18);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "107")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                academicYearDataServiceMock.Object,
                dateTimeQueryServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object,
                validationErrorHandlerMock.Object)
                .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
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
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 82,
                        ProgTypeNullable = 1,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicYearDataServiceMock.Setup(qs => qs.AugustThirtyFirst()).Returns(new DateTime(2018, 08, 31));
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, academicYearDataServiceMock.Object.AugustThirtyFirst())).Returns(18);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "107")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                academicYearDataServiceMock.Object,
                dateTimeQueryServiceMock.Object,
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

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2000, 01, 01));

            validationErrorHandlerMock.Verify();
        }

        private DateOfBirth_20Rule NewRule(IAcademicYearDataService academicYearDataService = null, IDateTimeQueryService dateTimeQueryService = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_20Rule(academicYearDataService, dateTimeQueryService, learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
