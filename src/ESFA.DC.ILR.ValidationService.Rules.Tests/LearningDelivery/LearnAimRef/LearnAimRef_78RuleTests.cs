using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_78RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_78Rule(null, service.Object, derivedData07.Object, fileData.Object, orgData.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_78Rule(handler.Object, null, derivedData07.Object, fileData.Object, orgData.Object));
        }

        /// <summary>
        /// New rule with null derived data 07 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedData07Throws()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_78Rule(handler.Object, service.Object, null, fileData.Object, orgData.Object));
        }

        /// <summary>
        /// New rule with null file data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullFileDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_78Rule(handler.Object, service.Object, derivedData07.Object, null, orgData.Object));
        }

        /// <summary>
        /// New rule with null organisation data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullOrgDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_78Rule(handler.Object, service.Object, derivedData07.Object, fileData.Object, null));
        }

        /// <summary>
        /// Rule name 1, matches a literal.
        /// </summary>
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("LearnAimRef_78", result);
        }

        /// <summary>
        /// Rule name 2, matches the constant.
        /// </summary>
        [Fact]
        public void RuleName2()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal(LearnAimRef_78Rule.Name, result);
        }

        /// <summary>
        /// Rule name 3 test, account for potential false positives.
        /// </summary>
        [Fact]
        public void RuleName3()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        /// <summary>
        /// Validate with null learner throws.
        /// </summary>
        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act/assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// First viable date meets expectation.
        /// </summary>
        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.FirstViableDate;

            // assert
            Assert.Equal(DateTime.Parse("2016-08-01"), result);
        }

        /// <summary>
        /// Last viable date meets expectation.
        /// </summary>
        [Fact]
        public void LastViableDateMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.LastViableDate;

            // assert
            Assert.Equal(DateTime.Parse("2017-07-31"), result);
        }

        /// <summary>
        /// Is restart meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoan, false)]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, false)]
        [InlineData(Monitoring.Delivery.Types.ApprenticeshipContract, false)]
        [InlineData(Monitoring.Delivery.Types.CommunityLearningProvision, false)]
        [InlineData(Monitoring.Delivery.Types.EligibilityForEnhancedApprenticeshipFunding, false)]
        [InlineData(Monitoring.Delivery.Types.FamilyEnglishMathsAndLanguage, false)]
        [InlineData(Monitoring.Delivery.Types.FullOrCoFunding, false)]
        [InlineData(Monitoring.Delivery.Types.HEMonitoring, false)]
        [InlineData(Monitoring.Delivery.Types.HouseholdSituation, false)]
        [InlineData(Monitoring.Delivery.Types.Learning, false)]
        [InlineData(Monitoring.Delivery.Types.LearningSupportFunding, false)]
        [InlineData(Monitoring.Delivery.Types.NationalSkillsAcademy, false)]
        [InlineData(Monitoring.Delivery.Types.PercentageOfOnlineDelivery, false)]
        [InlineData(Monitoring.Delivery.Types.Restart, true)]
        [InlineData(Monitoring.Delivery.Types.SourceOfFunding, false)]
        [InlineData(Monitoring.Delivery.Types.WorkProgrammeParticipation, false)]
        public void IsRestartMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDeliveryFAM>();
            mockDelivery
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            // act
            var result = sut.IsRestart(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is learner in custody meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, true)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.CoFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        [InlineData(Monitoring.Delivery.ESFA16To19Funding, false)]
        [InlineData(Monitoring.Delivery.ESFAAdultFunding, false)]
        [InlineData(Monitoring.Delivery.HigherEducationFundingCouncilEngland, false)]
        [InlineData(Monitoring.Delivery.LocalAuthorityCommunityLearningFunds, false)]
        [InlineData(Monitoring.Delivery.FinancedByAdvancedLearnerLoans, false)]
        public void IsLearnerInCustodyMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.IsLearnerInCustody(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is steel worker redundancy training meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.CoFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, true)]
        [InlineData(Monitoring.Delivery.ESFA16To19Funding, false)]
        [InlineData(Monitoring.Delivery.ESFAAdultFunding, false)]
        [InlineData(Monitoring.Delivery.HigherEducationFundingCouncilEngland, false)]
        [InlineData(Monitoring.Delivery.LocalAuthorityCommunityLearningFunds, false)]
        [InlineData(Monitoring.Delivery.FinancedByAdvancedLearnerLoans, false)]
        public void IsSteelWorkerRedundancyTrainingMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.IsSteelWorkerRedundancyTraining(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// In apprenticeship meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void InApprenticeshipMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            derivedData07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(expectation);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, derivedData07.Object, fileData.Object, orgData.Object);

            // act
            var result = sut.InApprenticeship(mockItem.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            fileData.VerifyAll();
            orgData.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is specialist designated college meets expectation
        /// </summary>
        /// <param name="ukprn">The ukprn.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(1004, false)]
        [InlineData(1005, true)]
        public void IsSpecialistDesignatedCollegeMeetsExpectation(int ukprn, bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(ukprn);

            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            orgData
                .Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, "USDC"))
                .Returns(expectation);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, derivedData07.Object, fileData.Object, orgData.Object);

            // act
            var result = sut.IsSpecialistDesignatedCollege();

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            fileData.VerifyAll();
            orgData.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is qualifying funding meets expectation
        /// </summary>
        /// <param name="funding">The funding.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        public void IsQualifyingFundingMeetsExpectation(int funding, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);

            // act
            var result = sut.IsQualifyingFunding(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is viable start unemployed maximum start meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2016-02-28", false)]
        [InlineData("2016-07-31", false)]
        [InlineData("2016-08-01", true)]
        [InlineData("2017-03-01", true)]
        [InlineData("2017-07-31", true)]
        [InlineData("2017-08-01", false)]
        [InlineData("2017-12-01", false)]
        public void IsViableStartMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            // act
            var result = sut.IsViableStart(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is qualifying notional NVQ meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(LARSNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.HigherLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level1, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level1_2, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level2, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level3, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level4, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level5, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level6, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level7, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level8, false)]
        [InlineData(LARSNotionalNVQLevelV2.MixedLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.NotKnown, false)]
        public void IsQualifyingNotionalNVQMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery
                .SetupGet(y => y.NotionalNVQLevelv2)
                .Returns(candidate);

            // act
            var result = sut.IsQualifyingNotionalNVQ(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying notional NVQ meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("testAim1")]
        [InlineData("testAim2")]
        public void HasQualifyingNotionalNVQMeetsExpectation(string candidate)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveriesFor(candidate))
                .Returns(Collection.EmptyAndReadOnly<ILARSLearningDelivery>());

            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, derivedData07.Object, fileData.Object, orgData.Object);

            // act
            var result = sut.HasQualifyingNotionalNVQ(mockDelivery.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            fileData.VerifyAll();
            orgData.VerifyAll();

            Assert.False(result);
        }

        /// <summary>
        /// Is qualifying category meets expectation
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfLARSCategory.LegalEntitlementLevel2, false)]
        [InlineData(TypeOfLARSCategory.OnlyForLegalEntitlementAtLevel3, true)]
        [InlineData(TypeOfLARSCategory.WorkPlacementSFAFunded, false)]
        [InlineData(TypeOfLARSCategory.WorkPreparationSFATraineeships, false)]
        [InlineData(36, false)]
        [InlineData(39, false)]
        public void IsQualifyingCategoryMeetsExpectation(int category, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockValidity = new Mock<ILARSLearningCategory>();
            mockValidity
                .SetupGet(x => x.CategoryRef)
                .Returns(category);

            // act
            var result = sut.IsQualifyingCategory(mockValidity.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("testAim1")]
        [InlineData("testAim2")]
        public void HasQualifyingCategoryMeetsExpectation(string candidate)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveriesFor(candidate))
                .Returns(Collection.EmptyAndReadOnly<ILARSLearningDelivery>());

            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, derivedData07.Object, fileData.Object, orgData.Object);

            // act
            var result = sut.HasQualifyingCategory(mockDelivery.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            fileData.VerifyAll();
            orgData.VerifyAll();

            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse("2016-08-01");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(TypeOfFunding.AdultSkills);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("LearnAimRef_78", learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", TypeOfFunding.AdultSkills))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            // it's this value that triggers the rule
            var mockCategory = new Mock<ILARSLearningCategory>();
            mockCategory
                .SetupGet(x => x.CategoryRef)
                .Returns(TypeOfLARSCategory.LegalEntitlementLevel2);

            var larsCategories = Collection.Empty<ILARSLearningCategory>();
            larsCategories.Add(mockCategory.Object);

            var mockLars = new Mock<ILARSLearningDelivery>();
            mockLars
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level3);
            mockLars
                .SetupGet(x => x.LearningDeliveryCategories)
                .Returns(larsCategories.AsSafeReadOnlyList());

            var larsItems = Collection.Empty<ILARSLearningDelivery>();
            larsItems.Add(mockLars.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveriesFor(learnAimRef))
                .Returns(larsItems.AsSafeReadOnlyList());

            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            derivedData07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(false);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(1004);

            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            orgData
                .Setup(x => x.LegalOrgTypeMatchForUkprn(1004, "USDC"))
                .Returns(false);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, derivedData07.Object, fileData.Object, orgData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            fileData.VerifyAll();
            orgData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse("2016-08-01");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(TypeOfFunding.AdultSkills);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // it's this value that triggers the rule
            var mockCategory = new Mock<ILARSLearningCategory>();
            mockCategory
                .SetupGet(x => x.CategoryRef)
                .Returns(TypeOfLARSCategory.OnlyForLegalEntitlementAtLevel3);

            var larsCategories = Collection.Empty<ILARSLearningCategory>();
            larsCategories.Add(mockCategory.Object);

            var mockLars = new Mock<ILARSLearningDelivery>();
            mockLars
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level3);
            mockLars
                .SetupGet(x => x.LearningDeliveryCategories)
                .Returns(larsCategories.AsSafeReadOnlyList());

            var larsItems = Collection.Empty<ILARSLearningDelivery>();
            larsItems.Add(mockLars.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveriesFor(learnAimRef))
                .Returns(larsItems.AsSafeReadOnlyList());

            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            derivedData07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(false);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(1004);

            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            orgData
                .Setup(x => x.LegalOrgTypeMatchForUkprn(1004, "USDC"))
                .Returns(false);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, derivedData07.Object, fileData.Object, orgData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            fileData.VerifyAll();
            orgData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnAimRef_78Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            return new LearnAimRef_78Rule(handler.Object, service.Object, derivedData07.Object, fileData.Object, orgData.Object);
        }
    }
}
