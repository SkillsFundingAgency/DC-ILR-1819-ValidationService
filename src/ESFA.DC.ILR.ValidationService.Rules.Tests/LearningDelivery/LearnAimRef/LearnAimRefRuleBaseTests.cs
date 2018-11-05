using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using LearnAimRefRuleBase = ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef.LearnAimRefRuleBase;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRefRuleBaseTests
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
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleBaseTestRule(null, service.Object, derivedData07.Object, derivedData11.Object));
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
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleBaseTestRule(handler.Object, null, derivedData07.Object, derivedData11.Object));
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
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleBaseTestRule(handler.Object, service.Object, null, derivedData11.Object));
        }

        /// <summary>
        /// New rule with null derived data 11 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedData11Throws()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleBaseTestRule(handler.Object, service.Object, derivedData07.Object, null));
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
            Assert.Equal("LearnAimRefRuleBaseTestRule", result);
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
            Assert.Equal(sut.GetName(), result);
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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void InReceiptOfBenefitsAtStartMeetsExpectation(bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            var employments = Collection.EmptyAndReadOnly<ILearnerEmploymentStatus>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            derivedData11
                .Setup(x => x.IsAdultFundedOnBenefitsAtStartOfAim(mockDelivery.Object, employments))
                .Returns(expectation);

            var sut = new LearnAimRefRuleBaseTestRule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object);

            // act
            var result = sut.InReceiptOfBenefitsAtStart(mockDelivery.Object, employments);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();

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
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleBaseTestRule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object);

            // act
            var result = sut.InApprenticeship(mockItem.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(TypeOfLearningProgramme.AdvancedLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, true)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel4, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel5, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel6, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, false)]
        [InlineData(TypeOfLearningProgramme.IntermediateLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.Traineeship, false)]
        public void InStandardApprenticeshipMeetsExpectation(int? progType, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(progType);

            // act
            var result = sut.InStandardApprenticeship(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfAim.AimNotPartOfAProgramme, false)]
        [InlineData(TypeOfAim.ComponentAimInAProgramme, true)]
        [InlineData(TypeOfAim.CoreAim16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfAim.ProgrammeAim, false)]
        public void IsComponentOfAProgramMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(candidate);

            // act
            var result = sut.IsComponentOfAProgram(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.CommunityLearning, TypeOfFunding.CommunityLearning, true)]
        [InlineData(TypeOfFunding.Other16To19, TypeOfFunding.OtherAdult, false)]
        public void HasQualifyingFundingMeetsExpectation(int funding, int desiredFunding, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);

            // act
            var result = sut.HasQualifyingFunding(mockDelivery.Object, desiredFunding);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Determines whether [is viable start apprenticeship minimum start meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2011-02-28", false)]
        [InlineData("2011-07-31", false)]
        [InlineData("2011-08-01", true)]
        [InlineData("2013-08-01", true)]
        public void IsViableStart_ApprenticeshipMinimumStart_MeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            // act
            var result = sut.IsViableStart(mockDelivery.Object, LearnAimRefRuleBase.ApprenticeshipMinimumStart, DateTime.Today);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Determines whether [is viable start unemployed maximum start meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2016-02-28", false)]
        [InlineData("2016-07-31", false)]
        [InlineData("2016-08-01", true)]
        [InlineData("2017-03-01", true)]
        public void IsViableStart_UnemployedMaximumStart_MeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            // act
            var result = sut.IsViableStart(mockDelivery.Object, LearnAimRefRuleBase.UnemployedMaximumStart, DateTime.MaxValue);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying category meets expectation
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfLARSValidity.AdultSkills, TypeOfLARSValidity.AdultSkills, true)]
        [InlineData(TypeOfLARSValidity.AdvancedLearnerLoan, TypeOfLARSValidity.Any, false)]
        [InlineData(TypeOfLARSValidity.Any, TypeOfLARSValidity.EFA16To19, false)]
        [InlineData(TypeOfLARSValidity.Apprenticeships, TypeOfLARSValidity.Apprenticeships, true)]
        [InlineData(TypeOfLARSValidity.CommunityLearning, TypeOfLARSValidity.EFA16To19, false)]
        [InlineData(TypeOfLARSValidity.EFA16To19, TypeOfLARSValidity.EFA16To19, true)]
        [InlineData(TypeOfLARSValidity.EFAConFundEnglish, TypeOfLARSValidity.EFAConFundEnglish, true)]
        [InlineData(TypeOfLARSValidity.EFAConFundMaths, TypeOfLARSValidity.EuropeanSocialFund, false)]
        [InlineData(TypeOfLARSValidity.EuropeanSocialFund, TypeOfLARSValidity.EuropeanSocialFund, true)]
        [InlineData(TypeOfLARSValidity.OLASSAdult, TypeOfLARSValidity.OLASSAdult, true)]
        [InlineData(TypeOfLARSValidity.Unemployed, TypeOfLARSValidity.Unemployed, true)]
        public void HasQualifyingCategoryMeetsExpectation(string category, string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockValidity = new Mock<ILARSValidity>();
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);

            // act
            var result = sut.HasQualifyingCategory(mockValidity.Object, candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has valid learning aim meets expectation
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="desiredCategory">The desired category.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfLARSValidity.AdultSkills, TypeOfLARSValidity.AdultSkills, true)]
        [InlineData(TypeOfLARSValidity.AdvancedLearnerLoan, TypeOfLARSValidity.Any, false)]
        [InlineData(TypeOfLARSValidity.Any, TypeOfLARSValidity.EFA16To19, false)]
        [InlineData(TypeOfLARSValidity.Apprenticeships, TypeOfLARSValidity.Apprenticeships, true)]
        [InlineData(TypeOfLARSValidity.CommunityLearning, TypeOfLARSValidity.EFA16To19, false)]
        [InlineData(TypeOfLARSValidity.EFA16To19, TypeOfLARSValidity.EFA16To19, true)]
        [InlineData(TypeOfLARSValidity.EFAConFundEnglish, TypeOfLARSValidity.EFAConFundEnglish, true)]
        [InlineData(TypeOfLARSValidity.EFAConFundMaths, TypeOfLARSValidity.EuropeanSocialFund, false)]
        [InlineData(TypeOfLARSValidity.EuropeanSocialFund, TypeOfLARSValidity.EuropeanSocialFund, true)]
        [InlineData(TypeOfLARSValidity.OLASSAdult, TypeOfLARSValidity.OLASSAdult, true)]
        [InlineData(TypeOfLARSValidity.Unemployed, TypeOfLARSValidity.Unemployed, true)]
        public void HasValidLearningAimMeetsExpectation(string category, string desiredCategory, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var mockValidity = new Mock<ILARSValidity>();
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);

            var larsValidities = Collection.Empty<ILARSValidity>();
            larsValidities.Add(mockValidity.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleBaseTestRule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object);

            // act
            var result = sut.HasQualifyingCategory(mockDelivery.Object, desiredCategory);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is advanced learner loan meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoan, true)]
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
        [InlineData(Monitoring.Delivery.Types.Restart, false)]
        [InlineData(Monitoring.Delivery.Types.SourceOfFunding, false)]
        [InlineData(Monitoring.Delivery.Types.WorkProgrammeParticipation, false)]
        public void IsAdvancedLearnerLoanMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDeliveryFAM>();
            mockDelivery
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            // act
            var result = sut.IsAdvancedLearnerLoan(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
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
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="funding">The funding.</param>
        /// <param name="category">The category.</param>
        /// <param name="monitor">The monitor.</param>
        /// <param name="dd07Return">if set to <c>true</c> [DD07 return].</param>
        /// <param name="dd11Return">if set to <c>true</c> [DD11 return].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLARSValidity.AdultSkills, Monitoring.Delivery.FullyFundedLearningAim, false, null)] // adult skills
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLARSValidity.Apprenticeships, Monitoring.Delivery.FullyFundedLearningAim, true, null)] // apprenticeships
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLARSValidity.Apprenticeships, Monitoring.Delivery.FullyFundedLearningAim, true, null)] // apprenticeships
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLARSValidity.Unemployed, Monitoring.Delivery.FullyFundedLearningAim, false, true)] // unemployed
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19, Monitoring.Delivery.FullyFundedLearningAim, null, null)]
        [InlineData(TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19, Monitoring.Delivery.FullyFundedLearningAim, null, null)]
        [InlineData(TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning, Monitoring.Delivery.FullyFundedLearningAim, null, null)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLARSValidity.OLASSAdult, Monitoring.Delivery.OLASSOffendersInCustody, null, null)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.AdvancedLearnerLoan, Monitoring.Delivery.FinancedByAdvancedLearnerLoans, null, null)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any, Monitoring.Delivery.FullyFundedLearningAim, null, null)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any, Monitoring.Delivery.FullyFundedLearningAim, null, null)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLARSValidity.EuropeanSocialFund, Monitoring.Delivery.FullyFundedLearningAim, null, null)]
        public void InvalidItemRaisesValidationMessage(int funding, string category, string monitor, bool? dd07Return, bool? dd11Return)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(monitor.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(monitor.Substring(3));

            var fams = Collection.Empty<ILearningDeliveryFAM>();
            fams.Add(mockItem.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(TypeOfAim.ComponentAimInAProgramme);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2014-08-01"));
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams.AsSafeReadOnlyList());

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
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == "LearnAimRefRuleBaseTestRule"),
                    Moq.It.Is<string>(y => y == learnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == LearnAimRefRuleBase.MessagePropertyName),
                    learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            // we just need to get a 'valid' category to get through the restrictions
            var mockValidity = new Mock<ILARSValidity>();
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.MinValue);
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.MaxValue);

            var larsValidities = Collection.Empty<ILARSValidity>();
            larsValidities.Add(mockValidity.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            if (dd07Return != null)
            {
                derivedData07
                    .Setup(x => x.IsApprenticeship(null))
                    .Returns(dd07Return.Value);
            }

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            if (dd11Return != null)
            {
                derivedData11
                    .Setup(x => x.IsAdultFundedOnBenefitsAtStartOfAim(mockDelivery.Object, null))
                    .Returns(dd11Return.Value);
            }

            var sut = new LearnAimRefRuleBaseTestRule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="funding">The funding.</param>
        /// <param name="category">The category.</param>
        /// <param name="monitor">The monitor.</param>
        /// <param name="dd07Return">The DD07 return.</param>
        /// <param name="dd11Return">The DD11 return.</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLARSValidity.AdultSkills, Monitoring.Delivery.FullyFundedLearningAim, false, null)] // adult skills
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLARSValidity.Apprenticeships, Monitoring.Delivery.FullyFundedLearningAim, true, null)] // apprenticeships
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLARSValidity.Apprenticeships, Monitoring.Delivery.FullyFundedLearningAim, true, null)] // apprenticeships
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLARSValidity.Unemployed, Monitoring.Delivery.FullyFundedLearningAim, false, true)] // unemployed
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19, Monitoring.Delivery.FullyFundedLearningAim, null, null)]
        [InlineData(TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19, Monitoring.Delivery.FullyFundedLearningAim, null, null)]
        [InlineData(TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning, Monitoring.Delivery.FullyFundedLearningAim, null, null)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLARSValidity.OLASSAdult, Monitoring.Delivery.OLASSOffendersInCustody, null, null)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.AdvancedLearnerLoan, Monitoring.Delivery.FinancedByAdvancedLearnerLoans, null, null)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any, Monitoring.Delivery.FullyFundedLearningAim, null, null)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any, Monitoring.Delivery.FullyFundedLearningAim, null, null)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLARSValidity.EuropeanSocialFund, Monitoring.Delivery.FullyFundedLearningAim, null, null)]
        public void ValidItemDoesNotRaiseValidationMessage(int funding, string category, string monitor, bool? dd07Return, bool? dd11Return)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(monitor.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(monitor.Substring(3));

            var fams = Collection.Empty<ILearningDeliveryFAM>();
            fams.Add(mockItem.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(TypeOfAim.ComponentAimInAProgramme);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2014-08-01"));
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams.AsSafeReadOnlyList());

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

            // we just need to get a 'valid' category to get through the restrictions
            var mockValidity = new Mock<ILARSValidity>();
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.MinValue);
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.MaxValue);

            var larsValidities = Collection.Empty<ILARSValidity>();
            larsValidities.Add(mockValidity.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            if (dd07Return != null)
            {
                derivedData07
                    .Setup(x => x.IsApprenticeship(null))
                    .Returns(dd07Return.Value);
            }

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            if (dd11Return != null)
            {
                derivedData11
                    .Setup(x => x.IsAdultFundedOnBenefitsAtStartOfAim(mockDelivery.Object, null))
                    .Returns(dd11Return.Value);
            }

            var sut = new LearnAimRefRuleBaseTestRule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object);

            // we have to ensure the secondary conditions check generate a 'pass' value
            sut.SetPassValue(true);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnAimRefRuleBaseTestRule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            return new LearnAimRefRuleBaseTestRule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object);
        }
    }
}
