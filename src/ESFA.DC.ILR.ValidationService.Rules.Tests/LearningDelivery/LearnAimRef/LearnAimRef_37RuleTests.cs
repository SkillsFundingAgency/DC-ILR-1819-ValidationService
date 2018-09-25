using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// </summary>
    public class LearnAimRef_37RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var mockService = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_37Rule(null, mockService.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_37Rule(mockHandler.Object, null));
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
            Assert.Equal("LearnAimRef_37", result);
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
            Assert.Equal(LearnAimRef_37Rule.Name, result);
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
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.ApprenticeshipStandard, true)]
        [InlineData(TypeOfFunding.AdultSkills, null, false)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.ApprenticeshipStandard, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.AdvancedLevelApprenticeship, false)]
        [InlineData(TypeOfFunding.CommunityLearning, TypeOfLearningProgramme.HigherApprenticeshipLevel4, false)]
        [InlineData(TypeOfFunding.CommunityLearning, null, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.ApprenticeshipStandard, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, null, false)]
        [InlineData(TypeOfFunding.Other16To19, TypeOfLearningProgramme.IntermediateLevelApprenticeship, false)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.AdvancedLevelApprenticeship, false)]
        public void IsApprenticeshipFunded(int funding, int? progType, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(progType);

            // act
            var result = sut.IsApprenticeshipFunded(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, false)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, true)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.OtherAdult, true)]
        public void IsOtherFundedMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            // act
            var result = sut.IsOtherFunded(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is viable start meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2010-08-01", false)]
        [InlineData("2011-07-31", false)]
        [InlineData("2011-08-01", true)]
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
        /// Determines whether [is current meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", true)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", true)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", true)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", false)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", false)]
        [InlineData("2013-07-16", "2015-05-09", "2013-07-15", false)]
        public void IsCurrentMeetsExpectation(string candidate, string startDate, string endDate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockValidity = new Mock<ILARSValidity>();
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(startDate));
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.Parse(endDate));

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            // act
            var result = sut.IsCurrent(mockValidity.Object, mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfLARSValidity.AdultSkills, false)]
        [InlineData(TypeOfLARSValidity.AdvancedLearnerLoan, false)]
        [InlineData(TypeOfLARSValidity.Any, true)]
        [InlineData(TypeOfLARSValidity.Apprenticeships, false)]
        [InlineData(TypeOfLARSValidity.CommunityLearning, false)]
        [InlineData(TypeOfLARSValidity.EFA16To19, false)]
        [InlineData(TypeOfLARSValidity.EFAConFundEnglish, false)]
        [InlineData(TypeOfLARSValidity.EFAConFundMaths, false)]
        [InlineData(TypeOfLARSValidity.EuropeanSocialFund, false)]
        [InlineData(TypeOfLARSValidity.OLASSAdult, false)]
        [InlineData(TypeOfLARSValidity.Unemployed, false)]
        public void HasQualifyingCategoryMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockValidity = new Mock<ILARSValidity>();
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(candidate);

            // act
            var result = sut.HasQualifyingCategory(mockValidity.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", TypeOfLARSValidity.Any, true)] // in date and correct category
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", TypeOfLARSValidity.Any, true)] // in date and correct category
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", TypeOfLARSValidity.Any, true)] // in date and correct category
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", TypeOfLARSValidity.AdvancedLearnerLoan, false)] // in date, incorrect category
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", TypeOfLARSValidity.Apprenticeships, false)] // in date, incorrect category
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", TypeOfLARSValidity.CommunityLearning, false)] // in date, incorrect category
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", TypeOfLARSValidity.Any, false)] // out of date, correct category
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", TypeOfLARSValidity.Any, false)] // out of date, correct category
        [InlineData("2013-07-16", "2015-05-09", "2013-07-15", TypeOfLARSValidity.Any, false)] // out of date, correct category
        public void HasValidLearningAimMeetsExpectation(string candidate, string startDate, string endDate, string category, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var mockValidity = new Mock<ILARSValidity>();
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(startDate));
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.Parse(endDate));

            var larsValidities = Collection.Empty<ILARSValidity>();
            larsValidities.Add(mockValidity.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var sut = new LearnAimRef_37Rule(handler.Object, service.Object);

            // act
            var result = sut.HasValidLearningAim(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
        }

        [Theory]
        [InlineData(DeliveryMonitoring.Types.AdvancedLearnerLoan, true)]
        [InlineData(DeliveryMonitoring.Types.AdvancedLearnerLoansBursaryFunding, false)]
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
        [InlineData(DeliveryMonitoring.Types.Restart, false)]
        [InlineData(DeliveryMonitoring.Types.SourceOfFunding, false)]
        [InlineData(DeliveryMonitoring.Types.WorkProgrammeParticipation, false)]
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

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLearningProgramme.HigherApprenticeshipLevel5)]
        [InlineData(TypeOfFunding.NotFundedByESFA, null)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel6)]
        [InlineData(TypeOfFunding.OtherAdult, null)]
        public void InvalidItemRaisesValidationMessage(int funding, int? progType)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2017-08-01"));
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(progType);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == LearnAimRef_37Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == LearnAimRef_37Rule.MessagePropertyName),
                    Moq.It.IsAny<ILearningDelivery>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var larsValidities = Collection.Empty<ILARSValidity>();

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var sut = new LearnAimRef_37Rule(handler.Object, service.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="funding">The funding.</param>
        /// <param name="progType">Type of the prog.</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLearningProgramme.HigherApprenticeshipLevel5)]
        [InlineData(TypeOfFunding.NotFundedByESFA, null)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel6)]
        [InlineData(TypeOfFunding.OtherAdult, null)]
        public void ValidItemDoesNotRaiseValidationMessage(int funding, int? progType)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2017-08-01"));
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(progType);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var validity = new Mock<ILARSValidity>();
            validity
                .SetupGet(x => x.ValidityCategory)
                .Returns(TypeOfLARSValidity.Any);
            validity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse("2016-04-03"));

            var larsValidities = Collection.Empty<ILARSValidity>();
            larsValidities.Add(validity.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var sut = new LearnAimRef_37Rule(handler.Object, service.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnAimRef_37Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new LearnAimRef_37Rule(handler.Object, service.Object);
        }
    }
}
