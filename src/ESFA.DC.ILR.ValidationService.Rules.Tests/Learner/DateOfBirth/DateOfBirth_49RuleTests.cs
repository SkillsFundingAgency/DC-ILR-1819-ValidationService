using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_49RuleTests : AbstractRuleTests<DateOfBirth_49Rule>
    {
        private readonly string[] _learningDeliveryFamCodes =
        {
            LearningDeliveryFAMCodeConstants.LDM_OLASS,
            LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy,
            LearningDeliveryFAMCodeConstants.LDM_SolentCity
        };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_49");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var nvqLevels = new List<string> { "3", "4", "5", "6", "7", "8", "H" };
            var categoryRef = 19;
            var orgType = LegalOrgTypeConstants.USDC;

            var fundModel = 35;
            var progType = 24;
            var dateOfBirth = new DateTime(1990, 10, 1);
            var learnStartDate = new DateTime(2016, 10, 1);
            var learnAimRef = "LearnAimRef";
            var ukprn = 1;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var fileDataServiceMock = new Mock<IFileDataService>();

            dateTimeQueryServiceMock.Setup(qs => qs.AgeAtGivenDate(dateOfBirth, learnStartDate)).Returns(25);
            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, nvqLevels)).Returns(true);

            NewRule(
                dateTimeQueryServiceMock.Object,
                dd07Mock.Object,
                fileDataServiceMock.Object,
                larsDataServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object,
                organisationDataServiceMock.Object).ConditionMet(dateOfBirth, learnStartDate, fundModel, learnAimRef).Should().BeTrue();
        }

        [Theory]
        [InlineData(true, false, false, false, true)]
        [InlineData(false, true, false, false, true)]
        [InlineData(false, false, true, false, true)]
        [InlineData(false, false, false, true, true)]
        [InlineData(false, false, false, false, false)]
        public void Excluded_Tests(bool isApprenticeship, bool hasLearningDeliveryFAMType, bool hasAnyLearningDeliveryFAMCodesForType, bool legalOrgTypeMatchForUkprn, bool expectedResult)
        {
            var orgType = LegalOrgTypeConstants.USDC;
            var progType = 24;
            var ukprn = 1;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "100"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(fc => fc.UKPRN()).Returns(ukprn);
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(isApprenticeship);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<List<TestLearningDeliveryFAM>>(), "RES")).Returns(hasLearningDeliveryFAMType);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<List<TestLearningDeliveryFAM>>(), "LDM", _learningDeliveryFamCodes)).Returns(hasAnyLearningDeliveryFAMCodesForType);
            organisationDataServiceMock.Setup(ds => ds.LegalOrgTypeMatchForUkprn(ukprn, orgType)).Returns(legalOrgTypeMatchForUkprn);

            NewRule(
                dateTimeQueryServiceMock.Object,
                dd07Mock.Object,
                fileDataServiceMock.Object,
                larsDataServiceMock.Object,
                learningDeliveryFAMQueryServiceMock.Object,
                organisationDataServiceMock.Object).Excluded(ukprn, progType, learningDeliveryFAMs).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("2016-07-31", false)]
        [InlineData("2016-08-01", true)]
        [InlineData("2017-01-01", true)]
        [InlineData("2017-07-31", true)]
        [InlineData("2017-08-01", false)]
        public void LearnStartDateConditionMet_Test(DateTime learnStartDate, bool expected)
        {
            NewRule().LearnStartDateConditionMet(learnStartDate).Should().Be(expected);
        }

        [Theory]
        [InlineData("1997-08-01", "2018-08-01", 24, true)]
        [InlineData("1997-08-02", "2018-08-01", 23, false)]
        public void DateOfBirthConditionMet_Test(DateTime learnStartDate, DateTime dateOfBirth, int ageAtGivenDate, bool expected)
        {
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.AgeAtGivenDate(dateOfBirth, learnStartDate)).Returns(ageAtGivenDate);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth, learnStartDate).Should().Be(expected);
        }

        [Theory]
        [InlineData(34, false)]
        [InlineData(35, true)]
        public void FundModelConditionMet_Test(int fundModel, bool expected)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(expected);
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
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("DateOfBirth", "01/01/2000")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/10/2016")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2000, 01, 01), new DateTime(2016, 10, 01), 1);

            validationErrorHandlerMock.Verify();
        }

        private DateOfBirth_49Rule NewRule(
            IDateTimeQueryService dateTimeQueryService = null,
            IDerivedData_07Rule dd07 = null,
            IFileDataService fileDataService = null,
            ILARSDataService larsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IOrganisationDataService organisationDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_49Rule(
                dateTimeQueryService,
                dd07,
                fileDataService,
                larsDataService,
                learningDeliveryFAMQueryService,
                organisationDataService,
                validationErrorHandler);
        }
    }
}
