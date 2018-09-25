using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// </summary>
    public class LearnDelFAMType_62RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var mockService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_62Rule(null, mockService.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_62Rule(mockHandler.Object, null, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 07 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule07Throws()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_62Rule(mockHandler.Object, mockService.Object, null, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 21 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule21Throws()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_62Rule(mockHandler.Object, mockService.Object, mockDDRule07.Object, null, mockDDRule28.Object, mockDDRule29.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 28 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule28Throws()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_62Rule(mockHandler.Object, mockService.Object, mockDDRule07.Object, mockDDRule21.Object, null, mockDDRule29.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 29 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule29Throws()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_62Rule(mockHandler.Object, mockService.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, null));
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
            Assert.Equal("LearnDelFAMType_62", result);
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
            Assert.Equal(LearnDelFAMType_62Rule.Name, result);
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
        /// Is learner in custody with learning delivery fam meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(DeliveryMonitoring.OLASSOffendersInCustody, true)]
        [InlineData(DeliveryMonitoring.FullyFundedLearningAim, false)]
        [InlineData(DeliveryMonitoring.CoFundedLearningAim, false)]
        [InlineData(DeliveryMonitoring.InReceiptOfLowWages, false)]
        [InlineData(DeliveryMonitoring.MandationToSkillsTraining, false)]
        [InlineData(DeliveryMonitoring.ReleasedOnTemporaryLicence, false)]
        [InlineData(DeliveryMonitoring.SteelIndustriesRedundancyTraining, false)]
        public void IsLearnerInCustodyWithLearningDeliveryFAMMeetsExpectation(string candidate, bool expectation)
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
        /// Is released on temporary licence with learning delivery fam meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(DeliveryMonitoring.OLASSOffendersInCustody, false)]
        [InlineData(DeliveryMonitoring.FullyFundedLearningAim, false)]
        [InlineData(DeliveryMonitoring.CoFundedLearningAim, false)]
        [InlineData(DeliveryMonitoring.InReceiptOfLowWages, false)]
        [InlineData(DeliveryMonitoring.MandationToSkillsTraining, false)]
        [InlineData(DeliveryMonitoring.ReleasedOnTemporaryLicence, true)]
        [InlineData(DeliveryMonitoring.SteelIndustriesRedundancyTraining, false)]
        public void IsReleasedOnTemporaryLicenceWithLearningDeliveryFAMMeetsExpectation(string candidate, bool expectation)
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
            var result = sut.IsReleasedOnTemporaryLicence(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is restart with learning delivery fam meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(DeliveryMonitoring.Types.AdvancedLearnerLoansBursaryFunding, false)]
        [InlineData(DeliveryMonitoring.Types.AdvancedLearnerLoan, false)]
        [InlineData(DeliveryMonitoring.Types.ApprenticeshipContract, false)]
        [InlineData(DeliveryMonitoring.Types.CommunityLearningProvision, false)]
        [InlineData(DeliveryMonitoring.Types.EligibilityForEnhancedApprenticeshipFunding, false)]
        [InlineData(DeliveryMonitoring.Types.FamilyEnglishMathsAndLanguage, false)]
        [InlineData(DeliveryMonitoring.Types.FullOrCoFunding, false)]
        [InlineData(DeliveryMonitoring.Types.HEMonitoring, false)]
        [InlineData(DeliveryMonitoring.Types.HouseholdSituation, false)]
        [InlineData(DeliveryMonitoring.Types.Learning, false)]
        [InlineData(DeliveryMonitoring.Types.LearningSupportFunding, false)]
        [InlineData(DeliveryMonitoring.Types.NationalSkillsAcademy, false)]
        [InlineData(DeliveryMonitoring.Types.PercentageOfOnlineDelivery, false)]
        [InlineData(DeliveryMonitoring.Types.Restart, true)]
        [InlineData(DeliveryMonitoring.Types.SourceOfFunding, false)]
        [InlineData(DeliveryMonitoring.Types.WorkProgrammeParticipation, false)]
        public void IsRestartWithLearningDeliveryFAMMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            // act
            var result = sut.IsRestart(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is steel worker redundancy training with learning delivery fam meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(DeliveryMonitoring.OLASSOffendersInCustody, false)]
        [InlineData(DeliveryMonitoring.FullyFundedLearningAim, false)]
        [InlineData(DeliveryMonitoring.CoFundedLearningAim, false)]
        [InlineData(DeliveryMonitoring.InReceiptOfLowWages, false)]
        [InlineData(DeliveryMonitoring.MandationToSkillsTraining, false)]
        [InlineData(DeliveryMonitoring.ReleasedOnTemporaryLicence, false)]
        [InlineData(DeliveryMonitoring.SteelIndustriesRedundancyTraining, true)]
        public void IsSteelWorkerRedundancyTrainingWithLearningDeliveryFAMMeetsExpectation(string candidate, bool expectation)
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
        /// Is adult funded unemployed with other state benefits meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsAdultFundedUnemployedWithOtherStateBenefitsMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearner>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(mockItem.Object))
                .Returns(expectation);

            var sut = new LearnDelFAMType_62Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsAdultFundedUnemployedWithOtherStateBenefits(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Is adult funded unemployed with benefits meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsAdultFundedUnemployedWithBenefitsMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearner>();
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(mockItem.Object))
                .Returns(expectation);

            var sut = new LearnDelFAMType_62Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsAdultFundedUnemployedWithBenefits(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Is inflexible element of training aim meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsInflexibleElementOfTrainingAimMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearner>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAim(mockItem.Object))
                .Returns(expectation);

            var sut = new LearnDelFAMType_62Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsInflexibleElementOfTrainingAim(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Is apprenticeship meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsApprenticeshipMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(expectation);

            var sut = new LearnDelFAMType_62Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsApprenticeship(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Is adult funding meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        public void IsAdultFundingMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            // act
            var result = sut.IsAdultFunding(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is viable start meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2016-08-01", false)]
        [InlineData("2017-07-31", false)]
        [InlineData("2017-08-01", true)]
        [InlineData("2017-09-14", true)]
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
        /// Is target age group meets expectation
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("1997-08-23", "2018-04-18", true)] // in age group
        [InlineData("1998-05-11", "2018-04-18", true)] // in age group
        [InlineData("1999-04-18", "2018-04-18", true)] // in age group, lower boundary
        [InlineData("1999-04-19", "2018-04-18", false)] // too young
        [InlineData("1995-04-18", "2018-04-18", true)] // in age group, upper boundary
        [InlineData("1995-04-17", "2018-04-18", false)] // too old
        public void IsTargetAgeGroupMeetsExpectation(string birthDate, string startDate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(y => y.DateOfBirthNullable)
                .Returns(DateTime.Parse(birthDate));

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            // act
            var result = sut.IsTargetAgeGroup(mockLearner.Object, mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is co funded meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(DeliveryMonitoring.OLASSOffendersInCustody, false)]
        [InlineData(DeliveryMonitoring.FullyFundedLearningAim, false)]
        [InlineData(DeliveryMonitoring.InReceiptOfLowWages, false)]
        [InlineData(DeliveryMonitoring.MandationToSkillsTraining, false)]
        [InlineData(DeliveryMonitoring.ReleasedOnTemporaryLicence, false)]
        [InlineData(DeliveryMonitoring.CoFundedLearningAim, true)]
        [InlineData(DeliveryMonitoring.SteelIndustriesRedundancyTraining, false)]
        public void IsCoFundedMeetsExpectation(string candidate, bool expectation)
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
            var result = sut.IsCoFunded(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is v2 notional level 2 meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(LARSNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.HigherLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level1, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level1_2, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level2, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level3, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level4, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level5, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level6, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level7, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level8, false)]
        [InlineData(LARSNotionalNVQLevelV2.MixedLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.NotKnown, false)]
        public void IsV2NotionalLevel2MeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILARSLearningDelivery>();
            mockItem
                .SetupGet(y => y.NotionalNVQLevelv2)
                .Returns(candidate);

            // act
            var result = sut.IsV2NotionalLevel2(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is legally entitled meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfLARSCategory.LegalEntitlementLevel2, true)]
        [InlineData(TypeOfLARSCategory.WorkPlacementSFAFunded, false)]
        [InlineData(TypeOfLARSCategory.WorkPreparationSFATraineeships, false)]
        [InlineData(23, false)]
        public void IsLegallyEntitledMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILARSLearningCategory>();
            mockItem
                .SetupGet(y => y.CategoryRef)
                .Returns(candidate);

            // act
            var result = sut.IsLegallyEntitled(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is excluded for adult funded unemployed with other state benefits
        /// </summary>
        [Fact]
        public void IsExcludedForAdultFundedUnemployedWithOtherStateBenefits()
        {
            // arrange
            var mockItem = new Mock<ILearner>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(mockItem.Object))
                .Returns(true);

            var sut = new LearnDelFAMType_62Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsExcluded(mockItem.Object);

            // assert
            Assert.True(result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Is excluded for adult funded unemployed with benefits
        /// </summary>
        [Fact]
        public void IsExcludedForAdultFundedUnemployedWithBenefits()
        {
            // arrange
            var mockItem = new Mock<ILearner>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(mockItem.Object))
                .Returns(false);
            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(mockItem.Object))
                .Returns(true);

            var sut = new LearnDelFAMType_62Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsExcluded(mockItem.Object);

            // assert
            Assert.True(result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Is excluded for inflexible element of training aim
        /// </summary>
        [Fact]
        public void IsExcludedForInflexibleElementOfTrainingAim()
        {
            // arrange
            var mockItem = new Mock<ILearner>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(mockItem.Object))
                .Returns(false);
            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(mockItem.Object))
                .Returns(false);
            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAim(mockItem.Object))
                .Returns(true);

            var sut = new LearnDelFAMType_62Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsExcluded(mockItem.Object);

            // assert
            Assert.True(result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Is excluded for apprenticeship
        /// </summary>
        [Fact]
        public void IsExcludedForApprenticeship()
        {
            // arrange
            const int progType = 23;
            var mockDel = new Mock<ILearningDelivery>();
            mockDel
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDel.Object);

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(mockItem.Object))
                .Returns(false);
            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(mockItem.Object))
                .Returns(false);
            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAim(mockItem.Object))
                .Returns(false);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(progType))
                .Returns(true);

            var sut = new LearnDelFAMType_62Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsExcluded(mockItem.Object);

            // assert
            Assert.True(result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(DeliveryMonitoring.Types.FullOrCoFunding);
            mockFAM
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns("2");

            var fams = Collection.Empty<ILearningDeliveryFAM>();
            fams.Add(mockFAM.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2017-08-01"));
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(TypeOfFunding.AdultSkills);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams.AsSafeReadOnlyList());

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.PriorAttainNullable)
                .Returns(TypeOfPriorAttainment.FullLevel2);
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.DateOfBirthNullable)
                .Returns(DateTime.Parse("1996-07-01"));
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == LearnDelFAMType_62Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == LearnDelFAMType_62Rule.MessagePropertyName),
                    Moq.It.IsAny<ILearningDelivery>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mock = new Mock<ILARSLearningDelivery>();
            mock
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level2);

            var larsDeliveries = Collection.Empty<ILARSLearningDelivery>();
            larsDeliveries.Add(mock.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveriesFor(learnAimRef))
                .Returns(larsDeliveries.AsSafeReadOnlyList());

            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_62Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            sut.ValidateDeliveries(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// the conditions here will get you to the final check which will return false for 'IsEarlyStageNVQ'
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(DeliveryMonitoring.Types.FullOrCoFunding);
            mockFAM
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns("2");

            var fams = Collection.Empty<ILearningDeliveryFAM>();
            fams.Add(mockFAM.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2017-08-01"));
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(TypeOfFunding.AdultSkills);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams.AsSafeReadOnlyList());

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.PriorAttainNullable)
                .Returns(TypeOfPriorAttainment.FullLevel2);
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.DateOfBirthNullable)
                .Returns(DateTime.Parse("1996-07-01"));
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var mockCat = new Mock<ILARSLearningCategory>();
            mockCat
                .SetupGet(x => x.CategoryRef)
                .Returns(TypeOfLARSCategory.LegalEntitlementLevel2);

            var larsCats = Collection.Empty<ILARSLearningCategory>();
            larsCats.Add(mockCat.Object);

            var mockLARSDel = new Mock<ILARSLearningDelivery>();
            mockLARSDel
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level2);
            mockLARSDel
                .SetupGet(x => x.LearningDeliveryCategories)
                .Returns(larsCats.AsSafeReadOnlyList());

            var larsDeliveries = Collection.Empty<ILARSLearningDelivery>();
            larsDeliveries.Add(mockLARSDel.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveriesFor(learnAimRef))
                .Returns(larsDeliveries.AsSafeReadOnlyList());

            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_62Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            sut.ValidateDeliveries(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnDelFAMType_62Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDD07>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            return new LearnDelFAMType_62Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);
        }
    }
}
