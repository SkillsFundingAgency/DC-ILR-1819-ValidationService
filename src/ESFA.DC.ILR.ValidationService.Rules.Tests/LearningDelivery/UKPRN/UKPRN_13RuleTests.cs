﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.UKPRN
{
    public class UKPRN_13RuleTests : AbstractRuleTests<UKPRN_13Rule>
    {
        private readonly IEnumerable<string> _fundingStreamPeriodCodes = new HashSet<string>()
        {
           FundingStreamPeriodCodeConstants.C1618_NLAP2018,
           FundingStreamPeriodCodeConstants.ANLAP2018
        };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("UKPRN_13");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("2018-08-01")]
        public void LearnActEndDateConditionMet_True(string learnActEndDateString)
        {
            DateTime? learnActEndDate = learnActEndDateString != null ? DateTime.Parse(learnActEndDateString) : (DateTime?)null;

            DateTime academicYear = new DateTime(2018, 8, 1);

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            if (learnActEndDate != null)
            {
                academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(false);
            }

            NewRule(academicYearQueryService: academicYearQueryServiceMock.Object).LearnActEndDateConditionMet(learnActEndDate, academicYear).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateConditionMet_False()
        {
            DateTime? learnActEndDate = new DateTime(2018, 5, 1);
            DateTime academicYear = new DateTime(2018, 8, 1);

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(true);

            NewRule(academicYearQueryService: academicYearQueryServiceMock.Object).LearnActEndDateConditionMet(learnActEndDate, academicYear).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(2).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2018, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2016, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "357"
                },
                 new TestLearningDeliveryFAM
                 {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "2"
                 }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "358")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT, "2")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES, "1")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, "358", true)]
        [InlineData(LearningDeliveryFAMTypeConstants.RES, "1", true)]
        public void LearningDeliveryFAMsConditionMet_False(string famType, string famCode, bool condition)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                },
                new TestLearningDeliveryFAM
                {
                   LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                   LearnDelFAMCode = "2"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "358")).Returns(condition);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT, "2")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES, "1")).Returns(condition);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void FCTFundingConditionMet_False()
        {
            IEnumerable<string> fundingStreamPeriodCodes = _fundingStreamPeriodCodes;

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(ds => ds.FundingRelationshipFCTExists(fundingStreamPeriodCodes)).Returns(true);

            NewRule(fcsDataService: fcsDataServiceMock.Object).FCTFundingConditionMet().Should().BeFalse();
        }

        [Fact]
        public void FCTFundingConditionMet_True()
        {
            IEnumerable<string> fundingStreamPeriodCodes = new HashSet<string>() { "ABC" };

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(ds => ds.FundingRelationshipFCTExists(fundingStreamPeriodCodes)).Returns(false);

            NewRule(fcsDataService: fcsDataServiceMock.Object).FCTFundingConditionMet().Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True()
        {
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime? learnActEndDate = new DateTime(2018, 8, 1);
            var aimType = 1;
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "357"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "2"
                }
            };

            var rule = NewRuleMock();

            rule.Setup(r => r.AimTypeConditionMet(1)).Returns(true);
            rule.Setup(r => r.LearnActEndDateConditionMet(learnActEndDate, academicYear)).Returns(true);
            rule.Setup(r => r.LearnStartDateConditionMet(startDate)).Returns(true);
            rule.Setup(r => r.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(true);
            rule.Setup(r => r.FCTFundingConditionMet()).Returns(true);

            rule.Object.ConditionMet(startDate, academicYear, learnActEndDate, aimType, learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, true, true, true, true)]
        [InlineData(true, false, true, true, true)]
        [InlineData(true, true, false, true, true)]
        [InlineData(true, true, true, false, true)]
        [InlineData(true, true, true, true, false)]
        [InlineData(true, true, false, false, false)]
        [InlineData(true, true, true, false, false)]
        [InlineData(false, false, true, true, true)]
        [InlineData(false, false, true, true, false)]
        [InlineData(false, false, false, false, false)]
        public void ConditionMet_False(bool condition1, bool condition2, bool condition3, bool condition4, bool condition5)
        {
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime? learnActEndDate = new DateTime(2018, 8, 1);
            var aimType = 1;
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "357"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "2"
                }
            };

            var rule = NewRuleMock();

            rule.Setup(r => r.LearnActEndDateConditionMet(learnActEndDate, academicYear)).Returns(condition1);
            rule.Setup(r => r.LearnStartDateConditionMet(startDate)).Returns(condition2);
            rule.Setup(r => r.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(condition3);
            rule.Setup(r => r.FCTFundingConditionMet()).Returns(condition4);
            rule.Setup(r => r.AimTypeConditionMet(aimType)).Returns(condition5);

            rule.Object.ConditionMet(startDate, academicYear, learnActEndDate, aimType, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            int ukprn = 12345678;
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime? learnActEndDate = new DateTime(2018, 8, 1);
            var aimType = 1;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "2"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = startDate,
                        LearnActEndDateNullable = learnActEndDate,
                        AimType = aimType,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            if (learnActEndDate != null)
            {
                academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(false);
            }

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(startDate);
            fileDataServiceMock.Setup(ds => ds.UKPRN()).Returns(ukprn);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "358")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT, "2")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES, "1")).Returns(false);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    academicYearDataServiceMock.Object,
                    academicYearQueryServiceMock.Object,
                    fcsDataServiceMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            int ukprn = 12345678;
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime? learnActEndDate = new DateTime(2018, 8, 1);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "2"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = startDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            if (learnActEndDate != null)
            {
                academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(true);
            }

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(startDate);
            fileDataServiceMock.Setup(ds => ds.UKPRN()).Returns(ukprn);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT, "2")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "358")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES, "1")).Returns(true);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    academicYearDataServiceMock.Object,
                    academicYearQueryServiceMock.Object,
                    fcsDataServiceMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var learnStartDate = new DateTime(2017, 9, 1);
            var ukprn = 12345678;
            var aimType = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/09/2017")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.UKPRN, 12345678)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ACT)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "2")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.AimType, 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnStartDate, ukprn, LearningDeliveryFAMTypeConstants.ACT, "2", aimType);

            validationErrorHandlerMock.Verify();
        }

        private UKPRN_13Rule NewRule(
            IFileDataService fileDataService = null,
            IAcademicYearDataService academicYearDataService = null,
            IAcademicYearQueryService academicYearQueryService = null,
            IFCSDataService fcsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new UKPRN_13Rule(fileDataService, academicYearDataService, academicYearQueryService, fcsDataService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
