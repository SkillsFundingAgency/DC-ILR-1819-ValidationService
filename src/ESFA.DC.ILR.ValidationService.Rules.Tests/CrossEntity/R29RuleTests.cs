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
    public class R29RuleTests : AbstractRuleTests<R29Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R29");
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
        public void Validate_Pass_No_OpenComponentAims()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        LearnActEndDateNullable = new DateTime(2018, 10, 10)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(null, null, null, null)]
        [InlineData(null, 2, null, null)]
        [InlineData(null, 2, 3, null)]
        [InlineData(null, 2, 3, 4)]
        [InlineData(null, 2, null, 4)]
        [InlineData(null, null, 3, 4)]
        [InlineData(1, null, 3, 4)]
        [InlineData(1, 2, 3, 4)]
        [InlineData(1, null, null, 4)]
        public void ConditionMet_True(int? progType, int? frameworkCode, int? pathwayCode, int? standardCode)
        {
            var mainAims = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimType = TypeOfAim.ProgrammeAim,
                    ProgTypeNullable = progType,
                    StdCodeNullable = standardCode,
                    FworkCodeNullable = frameworkCode,
                    PwayCodeNullable = pathwayCode
                },
                new TestLearningDelivery()
                {
                    AimType = TypeOfAim.ProgrammeAim,
                    ProgTypeNullable = progType,
                    StdCodeNullable = 9,
                    FworkCodeNullable = 100,
                    PwayCodeNullable = pathwayCode
                },
            };

            var componentAim = new TestLearningDelivery()
            {
                AimType = TypeOfAim.ComponentAimInAProgramme,
                ProgTypeNullable = progType.HasValue ? progType + 1 : 1,
                StdCodeNullable = standardCode.HasValue ? standardCode + 1 : 1,
                FworkCodeNullable = frameworkCode.HasValue ? frameworkCode + 1 : 1,
                PwayCodeNullable = pathwayCode.HasValue ? pathwayCode + 1 : 1,
            };

            NewRule().ConditionMet(mainAims, componentAim).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_NoOpenProgrammeAims()
        {
            var mainAims = new TestLearningDelivery[]
            {
            };

            var componentAim = new TestLearningDelivery()
            {
                AimType = TypeOfAim.ComponentAimInAProgramme,
                ProgTypeNullable = 1,
                StdCodeNullable = 2,
                FworkCodeNullable = 3,
                PwayCodeNullable = 4,
            };

            NewRule().ConditionMet(mainAims, componentAim).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var mainAims = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimType = TypeOfAim.ProgrammeAim,
                    ProgTypeNullable = 1,
                    StdCodeNullable = 2,
                    FworkCodeNullable = 3,
                    PwayCodeNullable = 4
                },
                new TestLearningDelivery()
                {
                    AimType = TypeOfAim.ProgrammeAim,
                    ProgTypeNullable = 30,
                    StdCodeNullable = 9,
                    FworkCodeNullable = 100,
                    PwayCodeNullable = 50
                },
            };

            var componentAim = new TestLearningDelivery()
            {
                AimType = TypeOfAim.ComponentAimInAProgramme,
                ProgTypeNullable = 1,
                StdCodeNullable = 2,
                FworkCodeNullable = 3,
                PwayCodeNullable = 4,
            };

            NewRule().ConditionMet(mainAims, componentAim).Should().BeFalse();
        }

        [Fact]
        public void Validate_Fail()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = 1,
                        StdCodeNullable = 2,
                        PwayCodeNullable = 4
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = 1,
                        StdCodeNullable = 9,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 5,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = 1,
                        StdCodeNullable = 1,
                        FworkCodeNullable = 1,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R29, It.IsAny<string>(), 5, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Once);
            }
        }

        [Fact]
        public void Validate_Pass()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 4
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = 1,
                        StdCodeNullable = 9,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 5,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 4,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R29, It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.AimType, TypeOfAim.ComponentAimInAProgramme)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ProgType, TypeOfLearningProgramme.AdvancedLevelApprenticeship)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FworkCode, 2)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.PwayCode, 3)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.StdCode, null)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "10/10/2017")).Verifiable();

            var learningDelivery = new TestLearningDelivery()
            {
                AimType = TypeOfAim.ComponentAimInAProgramme,
                ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                FworkCodeNullable = 2,
                PwayCodeNullable = 3,
                StdCodeNullable = null,
                FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                LearnActEndDateNullable = new DateTime(2017, 10, 10)
            };

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(learningDelivery);

            validationErrorHandlerMock.Verify();
        }

        private R29Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R29Rule(validationErrorHandler);
        }
    }
}
