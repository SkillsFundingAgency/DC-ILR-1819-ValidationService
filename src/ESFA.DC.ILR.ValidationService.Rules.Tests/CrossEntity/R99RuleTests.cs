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

        //public void Validate_Pass_ClosedNonFundedAims()
        //{
        //    var testLearner = new TestLearner()
        //    {
        //        LearnRefNumber = "123456789",
        //        LearningDeliveries = new TestLearningDelivery[]
        //        {
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 1,
        //                FundModel = TypeOfFunding.NotFundedByESFA,
        //                AimType = TypeOfAim.ProgrammeAim,
        //                LearnActEndDateNullable = new DateTime(2018, 10, 10),
        //                ProgTypeNullable = 1,
        //                FworkCodeNullable = 2,
        //                PwayCodeNullable = 3,
        //                StdCodeNullable = null,
        //            },
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 2,
        //                FundModel = 35,
        //                AimType = TypeOfAim.ComponentAimInAProgramme,
        //                ProgTypeNullable = 1,
        //                FworkCodeNullable = 2,
        //                PwayCodeNullable = 3,
        //                StdCodeNullable = null,
        //            },
        //        }
        //    };

        //    using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
        //    {
        //        NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        //        validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
        //    }
        //}

        //public void Validate_Pass_SameComponentAimFundModel()
        //{
        //    var testLearner = new TestLearner()
        //    {
        //        LearnRefNumber = "123456789",
        //        LearningDeliveries = new TestLearningDelivery[]
        //        {
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 1,
        //                FundModel = TypeOfFunding.NotFundedByESFA,
        //                AimType = TypeOfAim.ProgrammeAim,
        //                LearnActEndDateNullable = null,
        //                ProgTypeNullable = 1,
        //                FworkCodeNullable = 2,
        //                PwayCodeNullable = 3,
        //                StdCodeNullable = null,
        //            },
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 2,
        //                FundModel = TypeOfFunding.NotFundedByESFA,
        //                AimType = TypeOfAim.ComponentAimInAProgramme,
        //                ProgTypeNullable = 1,
        //                FworkCodeNullable = 2,
        //                PwayCodeNullable = 3,
        //                StdCodeNullable = null,
        //            },
        //        }
        //    };

        //    using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
        //    {
        //        NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        //        validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
        //    }
        //}

        //public void Validate_Fail_One_LearningDelivery()
        //{
        //    var testLearner = new TestLearner()
        //    {
        //        LearnRefNumber = "123456789",
        //        LearningDeliveries = new TestLearningDelivery[]
        //        {
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 1,
        //                FundModel = TypeOfFunding.NotFundedByESFA,
        //                AimType = TypeOfAim.ProgrammeAim,
        //                LearnActEndDateNullable = null,
        //                ProgTypeNullable = 1,
        //                FworkCodeNullable = 2,
        //                PwayCodeNullable = 3,
        //                StdCodeNullable = null,
        //            },
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 2,
        //                FundModel = TypeOfFunding.NotFundedByESFA,
        //                AimType = TypeOfAim.ProgrammeAim,
        //                LearnActEndDateNullable = null,
        //                ProgTypeNullable = 23,
        //                FworkCodeNullable = 24,
        //                PwayCodeNullable = 25,
        //                StdCodeNullable = null,
        //            },
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 3,
        //                FundModel = 35,
        //                AimType = TypeOfAim.ComponentAimInAProgramme,
        //                ProgTypeNullable = 1,
        //                FworkCodeNullable = 2,
        //                PwayCodeNullable = 3,
        //                StdCodeNullable = null,
        //            },
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 4,
        //                FundModel = TypeOfFunding.NotFundedByESFA,
        //                AimType = TypeOfAim.ComponentAimInAProgramme,
        //                ProgTypeNullable = 1,
        //                FworkCodeNullable = 2,
        //                PwayCodeNullable = 3,
        //                StdCodeNullable = null,
        //            },
        //        }
        //    };

        //    using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
        //    {
        //        NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        //        validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), 3, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Once);
        //        validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), 4, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
        //    }
        //}

        //[Theory]
        //[InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, null, null, null, null)]
        //[InlineData(TypeOfFunding.AdultSkills, null, null, null, null)]
        //[InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, null, null, null, null)]
        //[InlineData(TypeOfFunding.CommunityLearning, null, null, null, null)]
        //[InlineData(TypeOfFunding.EuropeanSocialFund, null, null, null, null)]
        //[InlineData(TypeOfFunding.Other16To19, null, null, null, null)]
        //[InlineData(TypeOfFunding.OtherAdult, null, null, null, null)]
        //[InlineData(TypeOfFunding.AdultSkills, 1, null, null, null)]
        //[InlineData(TypeOfFunding.AdultSkills, 1, 2, null, null)]
        //[InlineData(TypeOfFunding.AdultSkills, 1, 2, 3, null)]
        //[InlineData(TypeOfFunding.AdultSkills, null, null, null, 100)]
        //public void Validate_Fail(int fundModel, int? progType, int? frameworkCode, int? pathwayCode, int? standardCode)
        //{
        //    var testLearner = new TestLearner()
        //    {
        //        LearnRefNumber = "123456789",
        //        LearningDeliveries = new TestLearningDelivery[]
        //        {
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 1,
        //                FundModel = TypeOfFunding.NotFundedByESFA,
        //                AimType = TypeOfAim.ProgrammeAim,
        //                LearnActEndDateNullable = null,
        //                ProgTypeNullable = progType,
        //                FworkCodeNullable = frameworkCode,
        //                PwayCodeNullable = pathwayCode,
        //                StdCodeNullable = standardCode,
        //            },
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 2,
        //                FundModel = fundModel,
        //                AimType = TypeOfAim.ComponentAimInAProgramme,
        //                ProgTypeNullable = progType,
        //                FworkCodeNullable = frameworkCode,
        //                PwayCodeNullable = pathwayCode,
        //                StdCodeNullable = standardCode,
        //            },
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 3,
        //                FundModel = fundModel,
        //                AimType = TypeOfAim.ProgrammeAim,
        //                ProgTypeNullable = progType,
        //                FworkCodeNullable = frameworkCode,
        //                PwayCodeNullable = pathwayCode,
        //                StdCodeNullable = standardCode,
        //            }
        //        }
        //    };

        //    using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
        //    {
        //        NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        //        validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), 3, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
        //        validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), 2, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Once);
        //    }
        //}

        //[Theory]
        //[InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, null, null, null, null)]
        //[InlineData(TypeOfFunding.AdultSkills, null, null, null, null)]
        //[InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, null, null, null, null)]
        //[InlineData(TypeOfFunding.CommunityLearning, null, null, null, null)]
        //[InlineData(TypeOfFunding.EuropeanSocialFund, null, null, null, null)]
        //[InlineData(TypeOfFunding.Other16To19, null, null, null, null)]
        //[InlineData(TypeOfFunding.OtherAdult, null, null, null, null)]
        //[InlineData(TypeOfFunding.AdultSkills, 1, null, null, null)]
        //[InlineData(TypeOfFunding.AdultSkills, 1, 2, null, null)]
        //[InlineData(TypeOfFunding.AdultSkills, 1, 2, 3, null)]
        //[InlineData(TypeOfFunding.AdultSkills, null, null, null, 100)]
        //public void Validate_Pass_MismatchCourse(int fundModel, int? progType, int? frameworkCode, int? pathwayCode, int? standardCode)
        //{
        //    var testLearner = new TestLearner()
        //    {
        //        LearnRefNumber = "123456789",
        //        LearningDeliveries = new TestLearningDelivery[]
        //        {
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 1,
        //                FundModel = TypeOfFunding.NotFundedByESFA,
        //                AimType = TypeOfAim.ProgrammeAim,
        //                LearnActEndDateNullable = null,
        //                ProgTypeNullable = progType,
        //                FworkCodeNullable = frameworkCode,
        //                PwayCodeNullable = pathwayCode,
        //                StdCodeNullable = standardCode,
        //            },
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 2,
        //                FundModel = fundModel,
        //                AimType = TypeOfAim.ComponentAimInAProgramme,
        //                ProgTypeNullable = progType.HasValue ? progType + 1 : 1,
        //                FworkCodeNullable = frameworkCode.HasValue ? frameworkCode + 1 : 2,
        //                PwayCodeNullable = pathwayCode.HasValue ? pathwayCode + 1 : 3,
        //                StdCodeNullable = standardCode.HasValue ? standardCode + 1 : 1,
        //            },
        //            new TestLearningDelivery()
        //            {
        //                AimSeqNumber = 3,
        //                FundModel = fundModel,
        //                AimType = TypeOfAim.ProgrammeAim,
        //                ProgTypeNullable = progType,
        //                FworkCodeNullable = frameworkCode,
        //                PwayCodeNullable = pathwayCode,
        //                StdCodeNullable = standardCode,
        //            }
        //        }
        //    };

        //    using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
        //    {
        //        NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        //        validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
        //    }
        //}

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
