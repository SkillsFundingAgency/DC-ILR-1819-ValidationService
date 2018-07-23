﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_32RuleTests : AbstractRuleTests<DateOfBirth_32Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_32");
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(99).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2015, 10, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2016, 10, 1)).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_True()
        {
            var dateOfBirth = new DateTime(1990, 10, 1);
            var learnStartDate = new DateTime(2015, 10, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void DateOfBirthConditionMet_False()
        {
            var dateOfBirth = new DateTime(1992, 10, 1);
            var learnStartDate = new DateTime(2015, 10, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(23);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_False_Dob_Null()
        {
            var dateOfBirth = new DateTime(1990, 10, 1);
            var learnStartDate = new DateTime(2015, 10, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(null, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void LARSNotionalNVQL2ConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";
            var nvqLevels = new List<string> { "3", "4", "5", "6", "7", "8", "H" };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, nvqLevels)).Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object).LARSNotionalNVQL2ConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LARSNotionalNVQL2ConditionMet_False_LearnAimRef()
        {
            var learnAimRef = "LearnAimRef";
            var nvqLevels = new List<string> { "3", "4", "5", "6", "7", "8", "H" };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, nvqLevels)).Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object).LARSNotionalNVQL2ConditionMet("NotLearnAimRef").Should().BeFalse();
        }

        [Fact]
        public void LARSNotionalNVQL2ConditionMet_False_NVQLevels()
        {
            var learnAimRef = "LearnAimRef";
            var nvqLevels = new List<string> { "3", "4", "5", "6", "7", "8", "H" };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, nvqLevels)).Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object).LARSNotionalNVQL2ConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 24;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(25, true)]
        public void DD07ConditionMet_False(int? progType, bool mockBool)
        {
            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(mockBool);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var ldmCodes = new List<string> { "034", "346", "347", "339" };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "100"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", ldmCodes)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData("RES", "100", true, false)]
        [InlineData("LDM", "034", false, true)]
        [InlineData("LDM", "346", false, true)]
        [InlineData("LDM", "347", false, true)]
        [InlineData("LDM", "339", false, true)]
        public void LearningDeliveryFAMConditionMet_False(string famType, string famCode, bool resMock, bool ldmMock)
        {
            var ldmCodes = new List<string> { "034", "346", "347", "339" };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(resMock);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", ldmCodes)).Returns(ldmMock);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LARSNCategory_True_Category()
        {
            var learnAimRef = "LearnAimRef";
            var categoryRef = 19;

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, categoryRef)).Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object).LARSCategoryConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LARSNCategory_True_LearnAimRef()
        {
            var learnAimRef = "LearnAimRef";
            var categoryRef = 19;

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, categoryRef)).Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object).LARSCategoryConditionMet("NotLearnAimRef").Should().BeTrue();
        }

        [Fact]
        public void LARSNCategory_False()
        {
            var learnAimRef = "LearnAimRef";
            var categoryRef = 19;

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, categoryRef)).Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object).LARSCategoryConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void OrganisationTypeConditionMet_True_UKPRN()
        {
            var orgType = "USDC";
            var ukprn = 1;

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock.Setup(ds => ds.LegalOrgTypeMatchForUkprn(ukprn, orgType)).Returns(false);

            NewRule(organisationDataService: organisationDataServiceMock.Object).OrganisationTypeConditionMet(2).Should().BeTrue();
        }

        [Fact]
        public void OrganisationTypeConditionMet_True_OrgType()
        {
            var orgType = "USDC";
            var ukprn = 1;

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock.Setup(ds => ds.LegalOrgTypeMatchForUkprn(ukprn, orgType)).Returns(false);

            NewRule(organisationDataService: organisationDataServiceMock.Object).OrganisationTypeConditionMet(ukprn).Should().BeTrue();
        }

        [Fact]
        public void OrganisationTypeConditionMet_False()
        {
            var orgType = "USDC";
            var ukprn = 1;

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock.Setup(ds => ds.LegalOrgTypeMatchForUkprn(ukprn, orgType)).Returns(true);

            NewRule(organisationDataService: organisationDataServiceMock.Object).OrganisationTypeConditionMet(ukprn).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var ldmCodes = new List<string> { "034", "346", "347", "339" };
            var nvqLevels = new List<string> { "3", "4", "5", "6", "7", "8", "H" };
            var categoryRef = 19;
            var orgType = "USDC";

            var fundModel = 35;
            var progType = 24;
            var dateOfBirth = new DateTime(1990, 10, 1);
            var learnStartDate = new DateTime(2015, 10, 1);
            var learnAimRef = "LearnAimRef";
            var ukprn = 1;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "100"
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var fileDataCacheMock = new Mock<IFileDataCache>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);
            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, nvqLevels)).Returns(true);
            larsDataServiceMock.Setup(ds => ds.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, categoryRef)).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", ldmCodes)).Returns(false);
            organisationDataServiceMock.Setup(ds => ds.LegalOrgTypeMatchForUkprn(ukprn, orgType)).Returns(false);
            fileDataCacheMock.Setup(fc => fc.UKPRN).Returns(ukprn);

            NewRule(
                dd07Mock.Object,
                dateTimeQueryServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object,
                larsDataServiceMock.Object,
                organisationDataServiceMock.Object,
                fileDataCacheMock.Object).ConditionMet(fundModel, progType, learnStartDate, dateOfBirth, learnAimRef, learningDeliveryFAMs, ukprn).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var ldmCodes = new List<string> { "034", "346", "347", "339" };
            var nvqLevels = new List<string> { "3", "4", "5", "6", "7", "8", "H" };
            var categoryRef = 19;
            var orgType = "USDC";

            var fundModel = 99;
            var progType = 24;
            var dateOfBirth = new DateTime(1990, 10, 1);
            var learnStartDate = new DateTime(2015, 10, 1);
            var learnAimRef = "LearnAimRef";
            var ukprn = 1;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "100"
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var fileDataCacheMock = new Mock<IFileDataCache>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);
            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, nvqLevels)).Returns(true);
            larsDataServiceMock.Setup(ds => ds.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, categoryRef)).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", ldmCodes)).Returns(false);
            organisationDataServiceMock.Setup(ds => ds.LegalOrgTypeMatchForUkprn(ukprn, orgType)).Returns(false);
            fileDataCacheMock.Setup(fc => fc.UKPRN).Returns(ukprn);

            NewRule(
                dd07Mock.Object,
                dateTimeQueryServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object,
                larsDataServiceMock.Object,
                organisationDataServiceMock.Object,
                fileDataCacheMock.Object).ConditionMet(fundModel, progType, learnStartDate, dateOfBirth, learnAimRef, learningDeliveryFAMs, ukprn).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var ldmCodes = new List<string> { "034", "346", "347", "339" };
            var nvqLevels = new List<string> { "3", "4", "5", "6", "7", "8", "H" };
            var categoryRef = 19;
            var orgType = "USDC";

            var fundModel = 35;
            var progType = 24;
            var dateOfBirth = new DateTime(1990, 10, 1);
            var learnStartDate = new DateTime(2015, 10, 1);
            var learnAimRef = "LearnAimRef";
            var ukprn = 1;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "100"
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        LearnAimRef = learnAimRef,
                        LearnStartDate = learnStartDate,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var fileDataCacheMock = new Mock<IFileDataCache>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);
            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, nvqLevels)).Returns(true);
            larsDataServiceMock.Setup(ds => ds.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, categoryRef)).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", ldmCodes)).Returns(false);
            organisationDataServiceMock.Setup(ds => ds.LegalOrgTypeMatchForUkprn(ukprn, orgType)).Returns(false);
            fileDataCacheMock.Setup(fc => fc.UKPRN).Returns(ukprn);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    dd07Mock.Object,
                    dateTimeQueryServiceMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    larsDataServiceMock.Object,
                    organisationDataServiceMock.Object,
                    fileDataCacheMock.Object,
                    validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var ldmCodes = new List<string> { "034", "346", "347", "339" };
            var nvqLevels = new List<string> { "3", "4", "5", "6", "7", "8", "H" };
            var categoryRef = 19;
            var orgType = "USDC";

            var fundModel = 35;
            var progType = 24;
            var dateOfBirth = new DateTime(1990, 10, 1);
            var learnStartDate = new DateTime(2015, 10, 1);
            var learnAimRef = "LearnAimRef";
            var ukprn = 1;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "RES",
                    LearnDelFAMCode = "100"
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        LearnAimRef = learnAimRef,
                        LearnStartDate = learnStartDate,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var fileDataCacheMock = new Mock<IFileDataCache>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);
            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, nvqLevels)).Returns(true);
            larsDataServiceMock.Setup(ds => ds.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, categoryRef)).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", ldmCodes)).Returns(false);
            organisationDataServiceMock.Setup(ds => ds.LegalOrgTypeMatchForUkprn(ukprn, orgType)).Returns(false);
            fileDataCacheMock.Setup(fc => fc.UKPRN).Returns(ukprn);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dd07Mock.Object,
                    dateTimeQueryServiceMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    larsDataServiceMock.Object,
                    organisationDataServiceMock.Object,
                    fileDataCacheMock.Object,
                    validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("DateOfBirth", "01/01/2000")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2000, 01, 01), 1);

            validationErrorHandlerMock.Verify();
        }

        private DateOfBirth_32Rule NewRule(
            IDD07 dd07 = null,
            IDateTimeQueryService dateTimeQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            ILARSDataService larsDataService = null,
            IOrganisationDataService organisationDataService = null,
            IFileDataCache fileDataCache = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_32Rule(
                dd07,
                dateTimeQueryService,
                learningDeliveryFAMQueryService,
                larsDataService,
                organisationDataService,
                fileDataCache,
                validationErrorHandler);
        }
    }
}
