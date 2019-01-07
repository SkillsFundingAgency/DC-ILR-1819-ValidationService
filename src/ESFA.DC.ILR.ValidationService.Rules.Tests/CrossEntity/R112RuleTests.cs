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

        [Fact]
        public void Validate_EndDateNotEqualDateTo_Error()
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
                learner.LearningDeliveries.ElementAt(0).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFAM);

            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.HasLearningDeliveryFAMType(
                learner.LearningDeliveries.ElementAt(0).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NullDateTo_Error()
        {
            var learningDeliveryFAM = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                LearnDelFAMDateToNullable = null
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
                learner.LearningDeliveries.ElementAt(0).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFAM);

            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.HasLearningDeliveryFAMType(
                learner.LearningDeliveries.ElementAt(0).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(true);

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
            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.HasLearningDeliveryFAMType(
                learner.LearningDeliveries.ElementAt(0).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_MultipleLearningDeliveriesACTDateChanges_NoError()
        {
            var learningDeliveryFAMAim1 = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMDateFromNullable = new DateTime(2017, 09, 06),
                LearnDelFAMDateToNullable = new DateTime(2018, 09, 07)
            };

            var learningDeliveryFAMAim2 = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMDateFromNullable = new DateTime(2018, 01, 10),
                LearnDelFAMDateToNullable = new DateTime(2018, 09, 07)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2018, 09, 19),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            learningDeliveryFAMAim1
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2018, 09, 07),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            learningDeliveryFAMAim2
                        }
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.GetLearningDeliveryFAMByTypeAndLatestByDateFrom(
                learner.LearningDeliveries.ElementAt(0).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFAMAim1);
            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.HasLearningDeliveryFAMType(
                learner.LearningDeliveries.ElementAt(0).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(true);
            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.GetLearningDeliveryFAMByTypeAndLatestByDateFrom(
                learner.LearningDeliveries.ElementAt(1).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFAMAim2);
            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.HasLearningDeliveryFAMType(
                learner.LearningDeliveries.ElementAt(1).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_MultipleLearningDeliveriesACTDateToNull_Error()
        {
            var learningDeliveryFAMAim1 = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMDateFromNullable = new DateTime(2017, 09, 06),
                LearnDelFAMDateToNullable = new DateTime(2018, 09, 07)
            };

            var learningDeliveryFAMAim2 = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMDateFromNullable = new DateTime(2018, 01, 10),
                LearnDelFAMDateToNullable = null
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2018, 09, 19),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            learningDeliveryFAMAim1
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2018, 09, 07),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            learningDeliveryFAMAim2
                        }
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.GetLearningDeliveryFAMByTypeAndLatestByDateFrom(
                learner.LearningDeliveries.ElementAt(0).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFAMAim1);
            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.HasLearningDeliveryFAMType(
                learner.LearningDeliveries.ElementAt(0).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(true);

            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.GetLearningDeliveryFAMByTypeAndLatestByDateFrom(
                learner.LearningDeliveries.ElementAt(1).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFAMAim2);
            learningDeliveryFAMsQueryServiceMock.Setup(fam => fam.HasLearningDeliveryFAMType(
                learner.LearningDeliveries.ElementAt(1).LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
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
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, "02/06/2018"));

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 06, 01), LearningDeliveryFAMTypeConstants.ACT, new DateTime(2018, 06, 02));
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
