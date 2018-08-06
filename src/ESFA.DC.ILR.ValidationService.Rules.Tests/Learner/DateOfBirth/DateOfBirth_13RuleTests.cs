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
    public class DateOfBirth_13RuleTests : AbstractRuleTests<DateOfBirth_13Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_13");
        }

        [Fact]
        public void FundModelCondtionMet_True()
        {
            NewRule().FundModelConditionMet(99).Should().BeTrue();
        }

        [Fact]
        public void FundModelCondtionMet_False()
        {
            NewRule().FundModelConditionMet(25).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthCondtionMet_True()
        {
            var dateOfBirth = new DateTime(2005, 01, 01);
            var julyThirtyFirst = new DateTime(2019, 07, 31);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.JulyThirtyFirst()).Returns(julyThirtyFirst);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, julyThirtyFirst)).Returns(13);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth).Should().BeTrue();
        }

        [Fact]
        public void DateOfBirthConditionMet_False()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var julyThirtyFirst = new DateTime(2019, 07, 31);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.JulyThirtyFirst()).Returns(julyThirtyFirst);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, julyThirtyFirst)).Returns(19);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_False_NullDOB()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var julyThirtyFirst = new DateTime(2019, 07, 31);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.JulyThirtyFirst()).Returns(julyThirtyFirst);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, julyThirtyFirst)).Returns(19);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "SOF"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "1")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "120",
                    LearnDelFAMType = "SOF"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "1")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_Null()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "1")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var dateOfBirth = new DateTime(2005, 01, 01);
            var julyThirtyFirst = new DateTime(2019, 07, 31);
            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 99,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = "1",
                        LearnDelFAMType = "SOF"
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.JulyThirtyFirst()).Returns(julyThirtyFirst);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, julyThirtyFirst)).Returns(13);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "SOF", "1")).Returns(true);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object).ConditionMet(dateOfBirth, learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_DatOfBirth()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var julyThirtyFirst = new DateTime(2019, 07, 31);
            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 99,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = "1",
                        LearnDelFAMType = "SOF"
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.JulyThirtyFirst()).Returns(julyThirtyFirst);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, julyThirtyFirst)).Returns(19);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "SOF", "1")).Returns(true);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object).ConditionMet(dateOfBirth, learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var dateOfBirth = new DateTime(2005, 01, 01);
            var julyThirtyFirst = new DateTime(2019, 07, 31);
            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 25,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = "1",
                        LearnDelFAMType = "SOF"
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.JulyThirtyFirst()).Returns(julyThirtyFirst);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, julyThirtyFirst)).Returns(19);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "SOF", "1")).Returns(true);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object).ConditionMet(dateOfBirth, learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FAMS()
        {
            var dateOfBirth = new DateTime(2005, 01, 01);
            var julyThirtyFirst = new DateTime(2019, 07, 31);
            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 99,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = "120",
                        LearnDelFAMType = "SOF"
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.JulyThirtyFirst()).Returns(julyThirtyFirst);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, julyThirtyFirst)).Returns(13);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "SOF", "1")).Returns(false);

            NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object).ConditionMet(dateOfBirth, learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(2005, 01, 01);
            var julyThirtyFirst = new DateTime(2019, 07, 31);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "SOF"
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                   FundModel = 99,
                   LearningDeliveryFAMs = learningDeliveryFAMs
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = learningDeliveries
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.JulyThirtyFirst()).Returns(julyThirtyFirst);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, julyThirtyFirst)).Returns(13);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "1")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var dateOfBirth = new DateTime(1990, 01, 01);
            var julyThirtyFirst = new DateTime(2019, 07, 31);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "SOF"
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                   FundModel = 99,
                   LearningDeliveryFAMs = learningDeliveryFAMs
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = learningDeliveries
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicYearDataServiceMock.Setup(ds => ds.JulyThirtyFirst()).Returns(julyThirtyFirst);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, julyThirtyFirst)).Returns(19);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "1")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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

        private DateOfBirth_13Rule NewRule(IAcademicYearDataService academicYearDataService = null, IDateTimeQueryService dateTimeQueryService = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_13Rule(academicYearDataService, dateTimeQueryService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
