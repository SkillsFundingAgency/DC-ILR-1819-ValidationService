using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R115RuleTests : AbstractRuleTests<R115Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R115");
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
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
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type1", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type2", AFinCode = 2, AFinDate = new DateTime(2018, 1, 1) },
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
        public void ConditionMet_True()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimType = TypeOfAim.ProgrammeAim,
                    FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                    ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                    }
                },
                new TestLearningDelivery()
                {
                    AimType = TypeOfAim.ProgrammeAim,
                    FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                    ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                    }
                }
            };

            NewRule().ConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimType = TypeOfAim.ProgrammeAim,
                    FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                    ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "Type1", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                    }
                },
                new TestLearningDelivery()
                {
                    AimType = TypeOfAim.ProgrammeAim,
                    FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                    ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "Type2", AFinCode = 2, AFinDate = new DateTime(2018, 1, 1) },
                    }
                }
            };

            NewRule().ConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfLearningProgramme.AdvancedLevelApprenticeship)]
        [InlineData(TypeOfLearningProgramme.IntermediateLevelApprenticeship)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel4)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel5)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel6)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus)]
        public void IsApprenticeshipProgrammeAim_True(int progType)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                AimType = TypeOfAim.ProgrammeAim,
                FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                ProgTypeNullable = progType
            };

            NewRule().IsApprenticeshipProgrammeAim(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void IsApprenticeshipProgrammeAim_False_AimType()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                AimType = TypeOfAim.AimNotPartOfAProgramme,
                FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship
            };

            NewRule().IsApprenticeshipProgrammeAim(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void IsApprenticeshipProgrammeAim_False_FundModel()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                AimType = TypeOfAim.ProgrammeAim,
                FundModel = TypeOfFunding.AdultSkills,
                ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship
            };

            NewRule().IsApprenticeshipProgrammeAim(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void IsApprenticeshipProgrammeAim_False_ProgType()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                AimType = TypeOfAim.ProgrammeAim,
                FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                ProgTypeNullable = TypeOfLearningProgramme.Traineeship,
            };

            NewRule().IsApprenticeshipProgrammeAim(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void HasDuplicateApprenticeshipFinancialRecord_True()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                    }
                },
                new TestLearningDelivery()
                {
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                    }
                },
            };

            NewRule().HasDuplicateApprenticeshipFinancialRecord(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void HasDuplicateApprenticeshipFinancialRecord_True_MixedCase()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                    }
                },
                new TestLearningDelivery()
                {
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "tYPE", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                    }
                },
            };

            NewRule().HasDuplicateApprenticeshipFinancialRecord(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void HasDuplicateApprenticeshipFinancialRecord_True_NullAppFinRecords()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                    }
                },
                new TestLearningDelivery()
                {
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                    }
                },
                new TestLearningDelivery()
                {
                    AppFinRecords = null
                },
            };

            NewRule().HasDuplicateApprenticeshipFinancialRecord(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void HasDuplicateApprenticeshipFinancialRecord_True_SameLearningDelivery()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                    }
                },
                new TestLearningDelivery()
                {
                    AppFinRecords = new List<IAppFinRecord>()
                },
            };

            NewRule().HasDuplicateApprenticeshipFinancialRecord(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void HasDuplicateApprenticeshipFinancialRecord_False_NoAppFinRecords()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AppFinRecords = new List<IAppFinRecord>()
                },
                new TestLearningDelivery()
                {
                    AppFinRecords = new List<IAppFinRecord>()
                },
            };

            NewRule().HasDuplicateApprenticeshipFinancialRecord(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void HasDuplicateApprenticeshipFinancialRecord_False()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "Type1", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                    }
                },
                new TestLearningDelivery()
                {
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord() { AFinType = "Type2", AFinCode = 2, AFinDate = new DateTime(2019, 1, 1) },
                    }
                },
            };

            NewRule().HasDuplicateApprenticeshipFinancialRecord(learningDeliveries).Should().BeFalse();
        }

        private R115Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R115Rule(validationErrorHandler);
        }
    }
}
