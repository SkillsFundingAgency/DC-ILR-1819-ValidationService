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
    public class DateOfBirth_12RuleTests : AbstractRuleTests<DateOfBirth_12Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_12");
        }

        [Fact]
        public void FundModelCondtionMet_True()
        {
            NewRule().FundModelConditionMet(10).Should().BeTrue();
        }

        [Fact]
        public void FundModelCondtionMet_False()
        {
            NewRule().FundModelConditionMet(99).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthCondtionMet_True()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);

            NewRule(dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void DateOfBirthConditionMet_False()
        {
            var dateOfBirth = new DateTime(1998, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);

            NewRule(dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_False_NullDOB()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);

            NewRule(dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(null, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var famCodes = new List<string> { "1", "2" };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ASL"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "ASL", famCodes)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            var famCodes = new List<string> { "1", "2" };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "3",
                    LearnDelFAMType = "ASL"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "ASL", famCodes)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var famCodes = new List<string> { "1", "2" };
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 31);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ASL"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 10,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "ASL", famCodes)).Returns(true);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object).ConditionMet(dateOfBirth, learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_DOB()
        {
            var famCodes = new List<string> { "1", "2" };
            var dateOfBirth = new DateTime(1990, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 31);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ASL"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 10,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(28);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "ASL", famCodes)).Returns(true);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object).ConditionMet(dateOfBirth, learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var famCodes = new List<string> { "1", "2" };
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 31);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ASL"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 25,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "ASL", famCodes)).Returns(true);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object).ConditionMet(dateOfBirth, learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LDFAMs()
        {
            var famCodes = new List<string> { "1", "2" };
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 31);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "SOF"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 10,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "ASL", famCodes)).Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object).ConditionMet(dateOfBirth, learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var famCodes = new List<string> { "1", "2" };
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 31);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ASL"
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                   FundModel = 10,
                   LearningDeliveryFAMs = learningDeliveryFAMs
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = learningDeliveries
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "ASL", famCodes)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var famCodes = new List<string> { "1", "2" };
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 31);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ASL"
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                   FundModel = 25,
                   LearningDeliveryFAMs = learningDeliveryFAMs
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = learningDeliveries
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "ASL", famCodes)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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

        private DateOfBirth_12Rule NewRule(IDateTimeQueryService dateTimeQueryService = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_12Rule(dateTimeQueryService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
