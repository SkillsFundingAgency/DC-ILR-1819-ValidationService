using System;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R112RuleTests : AbstractRuleTests<R112Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R112");
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT, "2018-06-01", "2018-06-01")]
        [InlineData(LearningDeliveryFAMTypeConstants.RES, null, "2018-06-01")]
        public void LearningDeliveryFAMTypeConditionMet_False(string learnDelFAMType, string learnDelFAMDateToString, string learnActEndDateString)
        {
            DateTime? learnDelFAMDateTo = string.IsNullOrEmpty(learnDelFAMDateToString) ? (DateTime?)null : DateTime.Parse(learnDelFAMDateToString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            var learningDeliveryFAM = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                LearnDelFAMDateToNullable = learnDelFAMDateTo
            };

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    learningDeliveryFAM,
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMType,
                        LearnDelFAMDateFromNullable = new DateTime(2010, 02, 26),
                        LearnDelFAMDateToNullable = learnDelFAMDateTo
                    }
                };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.GetLearningDeliveryFAMByTypeAndLatestByDateFrom(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learnDelFAMType == LearningDeliveryFAMTypeConstants.RES ? null : learningDeliveryFAM);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMTypeConditionMet(learningDeliveryFAMs, learnActEndDate).Should().BeFalse();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT, "2018-06-01", "2018-08-12")]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT, null, "2018-08-12")]
        public void LearningDeliveryFAMTypeConditionMet_True(string learnDelFAMType, string learnDelFAMDateToString, string learnActEndDateString)
        {
            DateTime? learnDelFAMDateTo = string.IsNullOrEmpty(learnDelFAMDateToString) ? (DateTime?)null : DateTime.Parse(learnDelFAMDateToString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            var learningDeliveryFAM = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                LearnDelFAMDateToNullable = learnDelFAMDateTo
            };

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    learningDeliveryFAM,
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMType,
                        LearnDelFAMDateFromNullable = new DateTime(2010, 02, 26),
                        LearnDelFAMDateToNullable = learnDelFAMDateTo
                    }
                };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.GetLearningDeliveryFAMByTypeAndLatestByDateFrom(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(learningDeliveryFAM);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMTypeConditionMet(learningDeliveryFAMs, learnActEndDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.RES, "2018-06-01", "2018-06-01")]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT, null, "2018-06-01")]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT, "2018-06-01", "2018-06-01")]
        public void ConditionMet_False(string learnDelFAMType, string learnActEndDateString, string learnDelFAMDateToString)
        {
            DateTime? learnDelFAMDateTo = string.IsNullOrEmpty(learnDelFAMDateToString) ? (DateTime?)null : DateTime.Parse(learnDelFAMDateToString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            var learningDeliveryFAM = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                LearnDelFAMDateToNullable = learnDelFAMDateTo
            };

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    learningDeliveryFAM,
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMType,
                        LearnDelFAMDateFromNullable = new DateTime(2010, 02, 26),
                        LearnDelFAMDateToNullable = learnDelFAMDateTo
                    }
                };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.GetLearningDeliveryFAMByTypeAndLatestByDateFrom(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learnDelFAMType == LearningDeliveryFAMTypeConstants.RES ? null : learningDeliveryFAM);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(learnActEndDate, learningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT, "2018-06-01", null)]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT, "2018-06-01", "2018-06-10")]
        public void ConditionMet_True(string learnDelFAMType, string learnActEndDateString, string learnDelFAMDateToString)
        {
            DateTime? learnDelFAMDateTo = string.IsNullOrEmpty(learnDelFAMDateToString) ? (DateTime?)null : DateTime.Parse(learnDelFAMDateToString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            var learningDeliveryFAM = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                LearnDelFAMDateToNullable = learnDelFAMDateTo
            };

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    learningDeliveryFAM,
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMType,
                        LearnDelFAMDateFromNullable = new DateTime(2010, 02, 26),
                        LearnDelFAMDateToNullable = learnDelFAMDateTo
                    }
                };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.GetLearningDeliveryFAMByTypeAndLatestByDateFrom(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFAM);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(learnActEndDate, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFAM = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                LearnDelFAMDateToNullable = new DateTime(2018, 06, 02)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2018, 06, 01),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            learningDeliveryFAM
                        }
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.GetLearningDeliveryFAMByTypeAndLatestByDateFrom(
                learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFAM);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDeliveryFAM = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                LearnDelFAMDateToNullable = new DateTime(2018, 06, 01)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = null,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            learningDeliveryFAM
                        }
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.GetLearningDeliveryFAMByTypeAndLatestByDateFrom(
                learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFAM);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "01/06/2018"));
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ACT));
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, null));

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 06, 01), LearningDeliveryFAMTypeConstants.ACT, null);
            validationErrorHandlerMock.Verify();
        }

        public R112Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new R112Rule(
                validationErrorHandler: validationErrorHandler,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
