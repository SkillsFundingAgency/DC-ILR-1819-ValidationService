﻿using System;
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
    public class R99RuleTests : AbstractRuleTests<R99Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R99");
        }

        [Fact]
        public void Validate_Null_LearningDeliveries()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(new TestLearner());
            }
        }

        [Fact]
        public void Validate_Fail_ClosedAimOverlapStartDate()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        AimSeqNumber = 1,
                        LearnActEndDateNullable = new DateTime(2018, 10, 10),
                        LearnStartDate = new DateTime(2017, 10, 10)
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = TypeOfAim.ProgrammeAim,
                        LearnActEndDateNullable = new DateTime(2018, 10, 10),
                        LearnStartDate = new DateTime(2017, 09, 10)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            }
        }

        [Fact]
        public void Validate_Pass_OpenAimAfterCloseDate()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        AimSeqNumber = 1,
                        LearnStartDate = new DateTime(2017, 10, 10),
                        LearnActEndDateNullable = new DateTime(2018, 10, 10),
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = TypeOfAim.ProgrammeAim,
                        LearnStartDate = new DateTime(2018, 10, 11),
                        LearnActEndDateNullable = new DateTime(2018, 12, 10),
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_Pass_No_NoOpenMainAim()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        AimSeqNumber = 1,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = TypeOfAim.ProgrammeAim,
                        LearnActEndDateNullable = new DateTime(2018, 10, 10)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            }
        }

        [Fact]
        public void Validate_Fail_MultipleOpenAims()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        AimSeqNumber = 1,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = TypeOfAim.ProgrammeAim,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 3,
                        AimType = TypeOfAim.ProgrammeAim,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(3));
            }
        }

        [Fact]
        public void Validate_Pass_OverlappingAimsOutsideEndDate()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        AimSeqNumber = 1,
                        LearnStartDate = new DateTime(2018, 10, 10)
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = TypeOfAim.ProgrammeAim,
                        LearnStartDate = new DateTime(2018, 09, 10),
                        LearnActEndDateNullable = new DateTime(2018, 10, 09),
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            }
        }

        [Fact]
        public void Validate_Fail_OverlappingAims()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        AimSeqNumber = 1,
                        LearnStartDate = new DateTime(2018, 10, 10)
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = TypeOfAim.ProgrammeAim,
                        LearnStartDate = new DateTime(2018, 09, 10),
                        LearnActEndDateNullable = new DateTime(2018, 11, 10),
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 3,
                        AimType = TypeOfAim.ProgrammeAim,
                        LearnStartDate = new DateTime(2018, 09, 11)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(2));
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.AimType, TypeOfAim.ProgrammeAim)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "10/10/2018")).Verifiable();

            var learningDelivery = new TestLearningDelivery()
            {
                AimType = TypeOfAim.ProgrammeAim,
                LearnStartDate = new DateTime(2017, 01, 01),
                LearnActEndDateNullable = new DateTime(2018, 10, 10)
            };

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(learningDelivery);

            validationErrorHandlerMock.Verify();
        }

        private R99Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R99Rule(validationErrorHandler);
        }
    }
}
