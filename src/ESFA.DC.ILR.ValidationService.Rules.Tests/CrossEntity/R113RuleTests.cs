using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class R113RuleTests : AbstractRuleTests<R113Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R113");
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False()
        {
            string learnDelFAMType = string.Empty;
            DateTime? learnDelFAMDateTo = null;
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 07, 01),
                    LearnDelFAMDateToNullable = null,
                    LearnDelFAMCode = "213"
                }
            };

            NewRule().LearningDeliveryFAMsConditionMet(testLearningDeliveryFAMs, out learnDelFAMType, out learnDelFAMDateTo).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False_NullCheck()
        {
            string learnDelFAMType = string.Empty;
            DateTime? learnDelFAMDateTo = null;
            TestLearningDeliveryFAM[] testLearningDeliveryFAMs = null;

            NewRule().LearningDeliveryFAMsConditionMet(testLearningDeliveryFAMs, out learnDelFAMType, out learnDelFAMDateTo).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            string learnDelFAMType = string.Empty;
            DateTime? learnDelFAMDateTo = null;
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 07, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 09, 01),
                    LearnDelFAMCode = "213"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 09, 02),
                    LearnDelFAMDateToNullable = new DateTime(2018, 11, 01),
                    LearnDelFAMCode = "213"
                },
            };

            NewRule().LearningDeliveryFAMsConditionMet(testLearningDeliveryFAMs, out learnDelFAMType, out learnDelFAMDateTo).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            DateTime? learnActEndDate = new DateTime(2018, 07, 01);
            string learnDelFAMType = string.Empty;
            DateTime? learnDelFAMDateTo = null;
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 07, 01),
                    LearnDelFAMDateToNullable = null,
                    LearnDelFAMCode = "213"
                }
            };

            NewRule().ConditionMet(learnActEndDate, testLearningDeliveryFAMs, out learnDelFAMType, out learnDelFAMDateTo).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NullCheck()
        {
            DateTime? learnActEndDate = new DateTime(2018, 07, 01);
            string learnDelFAMType = string.Empty;
            DateTime? learnDelFAMDateTo = null;
            TestLearningDeliveryFAM[] testLearningDeliveryFAMs = null;

            NewRule().ConditionMet(learnActEndDate, testLearningDeliveryFAMs, out learnDelFAMType, out learnDelFAMDateTo).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            DateTime? learnActEndDate = null;
            string learnDelFAMType = string.Empty;
            DateTime? learnDelFAMDateTo = null;
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 07, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 09, 01),
                    LearnDelFAMCode = "213"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 09, 02),
                    LearnDelFAMDateToNullable = new DateTime(2018, 11, 01),
                    LearnDelFAMCode = "213"
                }
            };

            NewRule().ConditionMet(learnActEndDate, testLearningDeliveryFAMs, out learnDelFAMType, out learnDelFAMDateTo).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 07, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 09, 01),
                    LearnDelFAMCode = "213"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 09, 02),
                    LearnDelFAMDateToNullable = new DateTime(2018, 11, 01),
                    LearnDelFAMCode = "213"
                }
            };
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = null,
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    },
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = null,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                                LearnDelFAMDateFromNullable = new DateTime(2018, 11, 02),
                                LearnDelFAMDateToNullable = new DateTime(2019, 01, 01),
                                LearnDelFAMCode = "213"
                            }
                        }
                    }
                },
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 07, 01),
                    LearnDelFAMDateToNullable = null,
                    LearnDelFAMCode = "213"
                }
            };
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2018, 07, 01),
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    }
                },
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
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "05/01/2018")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ACT)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, "05/01/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(new DateTime(2018, 01, 05), LearningDeliveryFAMTypeConstants.ACT, new DateTime(2018, 01, 05));

            validationErrorHandlerMock.Verify();
        }

        public R113Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R113Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
