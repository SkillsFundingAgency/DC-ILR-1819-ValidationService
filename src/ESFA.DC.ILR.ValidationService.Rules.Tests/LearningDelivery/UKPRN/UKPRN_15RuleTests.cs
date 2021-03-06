﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.UKPRN
{
    public class UKPRN_15RuleTests : AbstractRuleTests<UKPRN_15Rule>
    {
        private readonly IEnumerable<string> _fundingStreamPeriodCodes = new HashSet<string>()
        {
            "16-18NLAP2018",
            "ANLAP2018"
        };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("UKPRN_15");
        }

        [Fact]
        public void LearnActEndDateConditionMet_True()
        {
            DateTime learnActEndDate = new DateTime(2018, 8, 1);

            DateTime academicYear = new DateTime(2018, 8, 1);

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate, academicYear)).Returns(false);

            NewRule(academicYearQueryService: academicYearQueryServiceMock.Object).LearnActEndDateConditionMet(learnActEndDate, academicYear).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateConditionMet_False()
        {
            DateTime learnActEndDate = new DateTime(2018, 5, 1);
            DateTime academicYear = new DateTime(2018, 8, 1);

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate, academicYear)).Returns(true);

            NewRule(academicYearQueryService: academicYearQueryServiceMock.Object).LearnActEndDateConditionMet(learnActEndDate, academicYear).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1"
                },
                 new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "358"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData("ACT", "2")]
        [InlineData("ASL", "1")]
        [InlineData("LDM", "1")]
        [InlineData("ACT", "358")]
        public void LearningDeliveryFAMsConditionMet_False(string famType, string famCode)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void FCTFundingConditionMet_True()
        {
            IEnumerable<string> fundingStreamPeriodCodes = _fundingStreamPeriodCodes;

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(ds => ds.FundingRelationshipFCTExists(fundingStreamPeriodCodes)).Returns(true);

            NewRule(fcsDataService: fcsDataServiceMock.Object).FCTFundingConditionMet().Should().BeTrue();
        }

        [Fact]
        public void FCTFundingConditionMet_False()
        {
            IEnumerable<string> fundingStreamPeriodCodes = new HashSet<string>() { "ABC" };

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(ds => ds.FundingRelationshipFCTExists(fundingStreamPeriodCodes)).Returns(false);

            NewRule(fcsDataService: fcsDataServiceMock.Object).FCTFundingConditionMet().Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime learnActEndDate = new DateTime(2018, 8, 1);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMCode = "2"
                }
            };

            var rule = NewRuleMock();

            rule.Setup(r => r.LearnActEndDateConditionMet(learnActEndDate, academicYear)).Returns(true);
            rule.Setup(r => r.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(true);
            rule.Setup(r => r.FCTFundingConditionMet()).Returns(true);

            rule.Object.ConditionMet(academicYear, learnActEndDate, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_NullCheck()
        {
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "2"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "358"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "ACT", "2")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "358")).Returns(true);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(true);

            NewRule(
                fcsDataService: fcsDataServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(academicYear, null, learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, false, true)]
        [InlineData(false, false, false)]
        public void ConditionMet_False(bool condition1, bool condition2, bool condition3)
        {
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime learnActEndDate = new DateTime(2018, 8, 1);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMCode = "2"
                }
            };

            var rule = NewRuleMock();

            rule.Setup(r => r.LearnActEndDateConditionMet(learnActEndDate, academicYear)).Returns(condition1);
            rule.Setup(r => r.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(condition2);
            rule.Setup(r => r.FCTFundingConditionMet()).Returns(condition3);

            rule.Object.ConditionMet(academicYear, learnActEndDate, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FAMsNullCheck()
        {
            List<TestLearningDeliveryFAM> learningDeliveryFAMs = null;

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(new DateTime(2018, 8, 1));
            academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(new DateTime(2018, 8, 1), new DateTime(2018, 8, 1))).Returns(false);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(false);

            NewRule(
                academicYearDataService: academicYearDataServiceMock.Object,
                academicYearQueryService: academicYearQueryServiceMock.Object,
                fcsDataService: fcsDataServiceMock.Object)
                .ConditionMet(new DateTime(2018, 08, 01), new DateTime(2018, 08, 01), learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            int ukprn = 12345678;
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime learnActEndDate = new DateTime(2018, 8, 1);

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        LearnStartDate = startDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "ACT",
                                LearnDelFAMCode = "1"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "358"
                            }
                        }
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate, academicYear)).Returns(false);

            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(startDate);
            fileDataServiceMock.Setup(ds => ds.UKPRN()).Returns(ukprn);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(true);

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

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        LearnStartDate = startDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "ACT",
                                LearnDelFAMCode = "1"
                            }
                        }
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
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(false);

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
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", "ACT")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMCode", "2")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("ACT", "2");

            validationErrorHandlerMock.Verify();
        }

        private UKPRN_15Rule NewRule(
            IFileDataService fileDataService = null,
            IAcademicYearDataService academicYearDataService = null,
            IAcademicYearQueryService academicYearQueryService = null,
            IFCSDataService fcsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new UKPRN_15Rule(fileDataService, academicYearDataService, academicYearQueryService, fcsDataService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
