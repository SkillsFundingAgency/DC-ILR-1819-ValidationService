﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_71RuleTests : AbstractRuleTests<LearnAimRef_71Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_71");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(TypeOfFunding.AdultSkills).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(TypeOfFunding.EuropeanSocialFund).Should().BeTrue();
        }

        [Theory]
        [InlineData(ValidationConstants.ZESF0001)]
        [InlineData("zesf0001")]
        public void LearnAimRefConditionMet_False(string learnAimRef)
        {
            NewRule().LearnAimRefConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LearnAimRefConditionMet_True()
        {
            NewRule().LearnAimRefConditionMet("ESF7890").Should().BeTrue();
        }

        [Fact]
        public void EsfSectorSubjectAreaLevelConditionMet_False()
        {
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(m => m.IsSectorSubjectAreaCodeExistsForContract("ESF223344")).Returns(false);

            NewRule(fCSDataService: fcsDataServiceMock.Object).EsfSectorSubjectAreaLevelConditionMet("ESF998877").Should().BeFalse();
        }

        [Fact]
        public void EsfSectorSubjectAreaLevelConditionMet_True()
        {
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(m => m.IsSectorSubjectAreaCodeExistsForContract("ESF0002")).Returns(true);

            NewRule(fCSDataService: fcsDataServiceMock.Object).EsfSectorSubjectAreaLevelConditionMet("ESF0002").Should().BeTrue();
        }

        [Theory]
        [InlineData("12.1", "13.1")]
        [InlineData("12.1", "15.1")]
        [InlineData("15.1", "13.1")]
        [InlineData("15.1", null)]
        [InlineData(null, "13.1")]
        public void LARSConditionMet_False(string sectorSubjectArea1, string sectorSubjectArea2)
        {
            string learnAimRef = ValidationConstants.ZESF0001;
            string conRefNumber = "ESF223344";

            var esfEligibilityRuleSectorSubjectAreaLevels = new IEsfEligibilityRuleSectorSubjectAreaLevel[]
                {
                    new EsfEligibilityRuleSectorSubjectAreaLevel()
                    {
                        TenderSpecReference = "tt_2976",
                        LotReference = "A1",
                        SectorSubjectAreaCode = 13.1M,
                    },
                    new EsfEligibilityRuleSectorSubjectAreaLevel()
                    {
                        TenderSpecReference = "tt_2978",
                        LotReference = "A3",
                        SectorSubjectAreaCode = 15.1M,
                    }
                };
            var learningDeliveries = new ILARSLearningDelivery[]
            {
                new Data.External.LARS.Model.LearningDelivery()
                {
                    LearnAimRef = learnAimRef,
                    SectorSubjectAreaTier1 = string.IsNullOrEmpty(sectorSubjectArea1) ? (decimal?)null : decimal.Parse(sectorSubjectArea1),
                    SectorSubjectAreaTier2 = string.IsNullOrEmpty(sectorSubjectArea2) ? (decimal?)null : decimal.Parse(sectorSubjectArea2)
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            fcsDataServiceMock.Setup(m => m.GetSectorSubjectAreaLevelsFor(conRefNumber)).Returns(esfEligibilityRuleSectorSubjectAreaLevels);
            larsDataServiceMock.Setup(l => l.GetDeliveriesFor(learnAimRef)).Returns(learningDeliveries);

            NewRule(
                fCSDataService: fcsDataServiceMock.Object,
                lARSDataService: larsDataServiceMock.Object)
                .LARSConditionMet(conRefNumber, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            string learnAimRef = "ZESF0002";
            string conRefNumber = "ESF7890";
            var esfEligibilityRuleSectorSubjectAreaLevels = new IEsfEligibilityRuleSectorSubjectAreaLevel[]
                {
                    new EsfEligibilityRuleSectorSubjectAreaLevel()
                    {
                        TenderSpecReference = "tt_2976",
                        LotReference = "01",
                        SectorSubjectAreaCode = 13.2M,
                    },
                };
            var learningDeliveries = new ILARSLearningDelivery[]
            {
                new Data.External.LARS.Model.LearningDelivery()
                {
                    LearnAimRef = learnAimRef,
                    SectorSubjectAreaTier1 = 14.2M,
                    SectorSubjectAreaTier2 = 15.2M
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            fcsDataServiceMock.Setup(m => m.GetSectorSubjectAreaLevelsFor(conRefNumber)).Returns(esfEligibilityRuleSectorSubjectAreaLevels);
            larsDataServiceMock.Setup(l => l.GetDeliveriesFor(learnAimRef)).Returns(learningDeliveries);

            NewRule(
                fCSDataService: fcsDataServiceMock.Object,
                lARSDataService: larsDataServiceMock.Object)
                .LARSConditionMet(conRefNumber, learnAimRef).Should().BeTrue();
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, ValidationConstants.ZESF0001, "ESF223456", "12.1", "13.1")]
        [InlineData(TypeOfFunding.AdultSkills, ValidationConstants.ZESF0001, "ESF223456", "12.1", "15.1")]
        [InlineData(TypeOfFunding.AdultSkills, ValidationConstants.ZESF0001, "ESF223456", "15.1", "13.1")]
        [InlineData(TypeOfFunding.AdultSkills, ValidationConstants.ZESF0001, "ESF223456", "15.1", null)]
        [InlineData(TypeOfFunding.AdultSkills, ValidationConstants.ZESF0001, "ESF223456", null, "13.1")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, ValidationConstants.ZESF0001, "ESF223456", null, "13.1")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, "ZESF0002", "ESF223456", null, "13.1")]
        public void ConditionMet_False(int fundModel, string conRefNumber, string learnAimRef, string sectorSubjectArea1, string sectorSubjectArea2)
        {
            var esfEligibilityRuleSectorSubjectAreaLevels = new IEsfEligibilityRuleSectorSubjectAreaLevel[]
                {
                    new EsfEligibilityRuleSectorSubjectAreaLevel()
                    {
                        TenderSpecReference = "tt_2976",
                        LotReference = "A1",
                        SectorSubjectAreaCode = 13.1M,
                    },
                    new EsfEligibilityRuleSectorSubjectAreaLevel()
                    {
                        TenderSpecReference = "tt_2978",
                        LotReference = "A3",
                        SectorSubjectAreaCode = 15.1M,
                    }
                };
            var learningDeliveries = new ILARSLearningDelivery[]
            {
                new Data.External.LARS.Model.LearningDelivery()
                {
                    LearnAimRef = learnAimRef,
                    SectorSubjectAreaTier1 = string.IsNullOrEmpty(sectorSubjectArea1) ? (decimal?)null : decimal.Parse(sectorSubjectArea1),
                    SectorSubjectAreaTier2 = string.IsNullOrEmpty(sectorSubjectArea2) ? (decimal?)null : decimal.Parse(sectorSubjectArea2)
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            fcsDataServiceMock.Setup(m => m.GetSectorSubjectAreaLevelsFor(conRefNumber)).Returns(esfEligibilityRuleSectorSubjectAreaLevels);
            fcsDataServiceMock.Setup(m => m.IsSectorSubjectAreaCodeExistsForContract("ESF223344")).Returns(false);
            larsDataServiceMock.Setup(l => l.GetDeliveriesFor(learnAimRef)).Returns(learningDeliveries);

            NewRule(
                fCSDataService: fcsDataServiceMock.Object,
                lARSDataService: larsDataServiceMock.Object)
                .ConditionMet(fundModel, conRefNumber, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            string learnAimRef = "ZESF0002";
            string conRefNumber = "ESF7890";
            var esfEligibilityRuleSectorSubjectAreaLevels = new IEsfEligibilityRuleSectorSubjectAreaLevel[]
                {
                    new EsfEligibilityRuleSectorSubjectAreaLevel()
                    {
                        TenderSpecReference = "tt_2976",
                        LotReference = "01",
                        SectorSubjectAreaCode = 13.2M,
                    },
                };
            var learningDeliveries = new ILARSLearningDelivery[]
            {
                new Data.External.LARS.Model.LearningDelivery()
                {
                    LearnAimRef = learnAimRef,
                    SectorSubjectAreaTier1 = 14.2M,
                    SectorSubjectAreaTier2 = 15.2M
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            fcsDataServiceMock.Setup(m => m.IsSectorSubjectAreaCodeExistsForContract(conRefNumber)).Returns(true);
            fcsDataServiceMock.Setup(m => m.GetSectorSubjectAreaLevelsFor(conRefNumber)).Returns(esfEligibilityRuleSectorSubjectAreaLevels);
            larsDataServiceMock.Setup(l => l.GetDeliveriesFor(learnAimRef)).Returns(learningDeliveries);

            NewRule(
                fCSDataService: fcsDataServiceMock.Object,
                lARSDataService: larsDataServiceMock.Object)
                .ConditionMet(TypeOfFunding.EuropeanSocialFund, conRefNumber, learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            string learnAimRef = "ZESF0002";
            string conRefNumber = "ESF7890";

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        ConRefNumber = conRefNumber,
                        FundModel = TypeOfFunding.EuropeanSocialFund
                    }
                }
            };

            var esfEligibilityRuleSectorSubjectAreaLevels = new IEsfEligibilityRuleSectorSubjectAreaLevel[]
                {
                    new EsfEligibilityRuleSectorSubjectAreaLevel()
                    {
                        TenderSpecReference = "tt_2976",
                        LotReference = "01",
                        SectorSubjectAreaCode = 13.2M,
                    },
                };
            var learningDeliveries = new ILARSLearningDelivery[]
            {
                new Data.External.LARS.Model.LearningDelivery()
                {
                    LearnAimRef = learnAimRef,
                    SectorSubjectAreaTier1 = 14.2M,
                    SectorSubjectAreaTier2 = 15.2M
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            fcsDataServiceMock.Setup(m => m.IsSectorSubjectAreaCodeExistsForContract(conRefNumber)).Returns(true);
            fcsDataServiceMock.Setup(m => m.GetSectorSubjectAreaLevelsFor(conRefNumber)).Returns(esfEligibilityRuleSectorSubjectAreaLevels);
            larsDataServiceMock.Setup(l => l.GetDeliveriesFor(learnAimRef)).Returns(learningDeliveries);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    fCSDataService: fcsDataServiceMock.Object,
                    lARSDataService: larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            string learnAimRef = "ZESF0002";
            string conRefNumber = "ESF7890";

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        ConRefNumber = conRefNumber,
                        FundModel = TypeOfFunding.AdultSkills
                    }
                }
            };

            var esfEligibilityRuleSectorSubjectAreaLevels = new IEsfEligibilityRuleSectorSubjectAreaLevel[]
                {
                    new EsfEligibilityRuleSectorSubjectAreaLevel()
                    {
                        TenderSpecReference = "tt_2976",
                        LotReference = "01",
                        SectorSubjectAreaCode = 13.2M,
                    },
                };
            var learningDeliveries = new ILARSLearningDelivery[]
            {
                new Data.External.LARS.Model.LearningDelivery()
                {
                    LearnAimRef = learnAimRef,
                    SectorSubjectAreaTier1 = 14.2M,
                    SectorSubjectAreaTier2 = 15.2M
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            fcsDataServiceMock.Setup(m => m.GetSectorSubjectAreaLevelsFor(conRefNumber)).Returns(esfEligibilityRuleSectorSubjectAreaLevels);
            fcsDataServiceMock.Setup(m => m.IsSectorSubjectAreaCodeExistsForContract("ESF223344")).Returns(false);
            larsDataServiceMock.Setup(l => l.GetDeliveriesFor(learnAimRef)).Returns(learningDeliveries);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    fCSDataService: fcsDataServiceMock.Object,
                    lARSDataService: larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(e => e.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.EuropeanSocialFund)).Verifiable();
            validationErrorHandlerMock.Setup(e => e.BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, "ESF-123456")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(TypeOfFunding.EuropeanSocialFund, "ESF-123456");

            validationErrorHandlerMock.Verify();
        }

        private LearnAimRef_71Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IFCSDataService fCSDataService = null,
            ILARSDataService lARSDataService = null)
        {
            return new LearnAimRef_71Rule(
                validationErrorHandler: validationErrorHandler,
                fCSDataService: fCSDataService,
                lARSDataService: lARSDataService);
        }
    }
}
