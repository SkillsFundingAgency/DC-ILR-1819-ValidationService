using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R61RuleTests : AbstractRuleTests<R61Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R61");
        }

        [Fact]
        public void GetLatestFam_Test_Null()
        {
            NewRule().GetLatestLSFFam(null).Should().BeNull();
        }

        [Fact]
        public void GetLatestFam_Test()
        {
            var fams = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "ABC",
                    LearnDelFAMDateFromNullable = null,
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMDateFromNullable = new DateTime(2017, 10, 10),
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMDateFromNullable = new DateTime(2017, 11, 10),
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "ABC",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 10, 11),
                }
            };

            var latestFam = NewRule().GetLatestLSFFam(fams);
            latestFam.Should().NotBeNull();
            latestFam.LearnDelFAMDateFromNullable.Should().NotBeNull();
            latestFam.LearnDelFAMDateFromNullable.Should().Be(new DateTime(2017, 11, 10));
            latestFam.LearnDelFAMType.Should().Be("LSF");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fams = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMDateFromNullable = new DateTime(2017, 10, 11),
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMDateToNullable = new DateTime(2017, 10, 10),
                }
            };

            var latestFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2017, 10, 09),
            };

            NewRule().ConditionMet(fams, latestFam).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NullLatestFam()
        {
            NewRule().ConditionMet(null, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var fams = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMDateToNullable = new DateTime(2017, 10, 10),
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMDateToNullable = null,
                }
            };

            var latestFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2017, 10, 10),
            };

            NewRule().ConditionMet(fams, latestFam).Should().BeFalse();
        }

        [Fact]
        public void Validate_False_NullLearningDelivery()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(new TestLearner());
            }
        }

        [Fact]
        public void Validate_Pass()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new ILearningDelivery[]
                {
                    new TestLearningDelivery() { },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "ABC",
                                LearnDelFAMDateFromNullable = null,
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "LSF",
                                LearnDelFAMDateFromNullable = new DateTime(2017, 10, 10),
                                LearnDelFAMDateToNullable = new DateTime(2017, 11, 10),
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "LSF",
                                LearnDelFAMDateFromNullable = new DateTime(2017, 11, 11),
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "ABC",
                                LearnDelFAMDateFromNullable = new DateTime(2017, 10, 09),
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
        public void Validate_Fail()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new ILearningDelivery[]
                {
                    new TestLearningDelivery() { },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "ABC",
                                LearnDelFAMDateFromNullable = null,
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "LSF",
                                LearnDelFAMDateFromNullable = new DateTime(2017, 10, 10),
                                LearnDelFAMDateToNullable = new DateTime(2017, 11, 10),
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "LSF",
                                LearnDelFAMDateFromNullable = new DateTime(2017, 11, 09),
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "ABC",
                                LearnDelFAMDateFromNullable = new DateTime(2017, 10, 09),
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
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, "LSF")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateFrom, "01/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, "01/01/2018")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMDateFromNullable = new DateTime(2017, 01, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 01, 01)
                });

            validationErrorHandlerMock.Verify();
        }

        public R61Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R61Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
