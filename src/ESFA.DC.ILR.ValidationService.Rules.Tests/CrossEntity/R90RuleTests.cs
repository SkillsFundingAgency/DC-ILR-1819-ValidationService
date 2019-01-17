﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R90RuleTests : AbstractRuleTests<R90Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R90");
        }

        [Fact]
        public void Validate_Null_LearningDeliveries()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(new TestLearner());
            }
        }

        [Theory]
        [InlineData(null, null, null, null)]
        [InlineData(null, 2, null, null)]
        [InlineData(null, null, 3, null)]
        [InlineData(null, null, null, 4)]
        [InlineData(1, null, null, null)]
        [InlineData(1, 2, null, null)]
        [InlineData(1, 2, 3, null)]
        [InlineData(1, 2, 3, 4)]
        [InlineData(null, 2, 3, 4)]
        [InlineData(null, null, 3, 4)]
        [InlineData(1, null, 3, 4)]
        [InlineData(1, 2, null, 4)]
        [InlineData(1, null, null, 4)]
        public void ConditionMet_True(int? progType, int? frameworkCode, int? pathwayCode, int? standardCode)
        {
            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 1,
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    LearnActEndDateNullable = null,
                    ProgTypeNullable = progType,
                    FworkCodeNullable = frameworkCode,
                    PwayCodeNullable = pathwayCode,
                    StdCodeNullable = standardCode,
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    ProgTypeNullable = 7,
                    FworkCodeNullable = 8,
                    PwayCodeNullable = 5,
                    StdCodeNullable = 6,
                },
            };

            var mainAim = new TestLearningDelivery()
            {
                AimSeqNumber = 3,
                AimType = TypeOfAim.ProgrammeAim,
                ProgTypeNullable = progType,
                FworkCodeNullable = frameworkCode,
                PwayCodeNullable = pathwayCode,
                StdCodeNullable = standardCode,
                LearnActEndDateNullable = new DateTime(2017, 10, 10)
            };

            NewRule().ConditionMet(mainAim, learningDeliveries).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, null, null, null)]
        [InlineData(null, 2, null, null)]
        [InlineData(null, null, 3, null)]
        [InlineData(null, null, null, 4)]
        [InlineData(1, null, null, null)]
        [InlineData(1, 2, null, null)]
        [InlineData(1, 2, 3, null)]
        [InlineData(1, 2, 3, 4)]
        [InlineData(null, 2, 3, 4)]
        [InlineData(null, null, 3, 4)]
        [InlineData(1, null, 3, 4)]
        [InlineData(1, 2, null, 4)]
        [InlineData(1, null, null, 4)]
        public void ConditionMet_False_NoMatchingProgramme(int? progType, int? frameworkCode, int? pathwayCode, int? standardCode)
        {
            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 1,
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    LearnActEndDateNullable = null,
                    ProgTypeNullable = progType,
                    FworkCodeNullable = frameworkCode,
                    PwayCodeNullable = pathwayCode,
                    StdCodeNullable = standardCode,
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    ProgTypeNullable = 7,
                    FworkCodeNullable = 8,
                    PwayCodeNullable = 5,
                    StdCodeNullable = 6,
                },
            };

            var mainAim = new TestLearningDelivery()
            {
                AimSeqNumber = 3,
                AimType = TypeOfAim.ProgrammeAim,
                ProgTypeNullable = 99,
                FworkCodeNullable = 100,
                PwayCodeNullable = 200,
                StdCodeNullable = 300,
                LearnActEndDateNullable = new DateTime(2017, 10, 10)
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object).ConditionMet(mainAim, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ClosedComponentAim()
        {
            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    ProgTypeNullable = 99,
                    FworkCodeNullable = 100,
                    PwayCodeNullable = 200,
                    StdCodeNullable = 300,
                    LearnActEndDateNullable = new DateTime(2017, 09, 10)
                },
            };

            var mainAim = new TestLearningDelivery()
            {
                AimSeqNumber = 3,
                AimType = TypeOfAim.ProgrammeAim,
                ProgTypeNullable = 99,
                FworkCodeNullable = 100,
                PwayCodeNullable = 200,
                StdCodeNullable = 300,
                LearnActEndDateNullable = new DateTime(2017, 10, 10)
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(true);
            NewRule(larsDataService:larsDataServiceMock.Object).ConditionMet(mainAim, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NoComponentAims()
        {
            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    AimType = TypeOfAim.ProgrammeAim,
                    ProgTypeNullable = 99,
                    FworkCodeNullable = 100,
                    PwayCodeNullable = 200,
                    StdCodeNullable = 300,
                },
            };

            var mainAim = new TestLearningDelivery()
            {
                AimSeqNumber = 3,
                AimType = TypeOfAim.ProgrammeAim,
                ProgTypeNullable = 99,
                FworkCodeNullable = 100,
                PwayCodeNullable = 200,
                StdCodeNullable = 300,
                LearnActEndDateNullable = new DateTime(2017, 10, 10)
            };

            NewRule().ConditionMet(mainAim, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void Exclude_True()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), It.IsAny<string>()))
                .Returns(true);

            NewRule(larsDataService:larsDataServiceMock.Object).Excluded("learnRef1").Should().BeTrue();
        }

        [Fact]
        public void Exclude_False()
        {
            NewRule().Excluded("learnRef1").Should().BeFalse();
        }

        [Fact]
        public void Validate_Succsess_NoClosedMainAim()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    AimType = TypeOfAim.ProgrammeAim,
                    ProgTypeNullable = 99,
                    FworkCodeNullable = 100,
                    PwayCodeNullable = 200,
                    StdCodeNullable = 300,
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    ProgTypeNullable = 99,
                    FworkCodeNullable = 100,
                    PwayCodeNullable = 200,
                    StdCodeNullable = 300,
                    LearnActEndDateNullable = new DateTime(2017, 10, 10)
                }
            }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_Succsess_NoMatchingOpenComponentAim()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = 99,
                        FworkCodeNullable = 100,
                        PwayCodeNullable = 200,
                        StdCodeNullable = 300,
                        LearnActEndDateNullable = new DateTime(2017, 10, 10)
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        StdCodeNullable = 300,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_Fail()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = 99,
                        FworkCodeNullable = 100,
                        PwayCodeNullable = 200,
                        StdCodeNullable = 300,
                        LearnActEndDateNullable = new DateTime(2017, 10, 10)
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 3,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = 99,
                        FworkCodeNullable = 100,
                        PwayCodeNullable = 200,
                        StdCodeNullable = 300,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.AimType, TypeOfAim.ProgrammeAim)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "10/10/2018")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, "aimRef")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ProgType, 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.PwayCode, 2)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FworkCode, 3)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.StdCode, 4)).Verifiable();

            var learningDelivery = new TestLearningDelivery()
            {
                AimType = TypeOfAim.ProgrammeAim,
                LearnActEndDateNullable = new DateTime(2018, 10, 10),
                LearnAimRef = "aimRef",
                ProgTypeNullable = 1,
                PwayCodeNullable = 2,
                FworkCodeNullable = 3,
                StdCodeNullable = 4
            };

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(learningDelivery);

            validationErrorHandlerMock.Verify();
        }

        private R90Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILARSDataService larsDataService = null)
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(false);

            return new R90Rule(validationErrorHandler, larsDataService ?? larsDataServiceMock.Object);
        }
    }
}
