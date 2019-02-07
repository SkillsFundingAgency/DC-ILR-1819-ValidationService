using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

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
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleBaseTestRule(null, provider.Object, service.Object));
        }

        /// <summary>
        /// New rule with null action provider throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullActionProviderThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleBaseTestRule(handler.Object, null, service.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleBaseTestRule(handler.Object, provider.Object, null));
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
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="funding">The funding.</param>
        /// <param name="category">The category.</param>
        /// <param name="monitor">The monitor.</param>
        /// <param name="dd07Return">if set to <c>true</c> [DD07 return].</param>
        /// <param name="dd11Return">if set to <c>true</c> [DD11 return].</param>
        /// <param name="apprReturn">The apprentice return.</param>
        /// <param name="stdAppReturn">The standard apprentice return.</param>
        /// <param name="olassReturn">The olass return.</param>
        /// <param name="componentAim">The component aim.</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLARSValidity.AdultSkills, Monitoring.Delivery.FullyFundedLearningAim, false, null, null, null, false, null)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLARSValidity.Apprenticeships, Monitoring.Delivery.FullyFundedLearningAim, true, null, true, false, null, true)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLARSValidity.Apprenticeships, Monitoring.Delivery.FullyFundedLearningAim, true, null, null, null, null, null)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLARSValidity.Unemployed, Monitoring.Delivery.FullyFundedLearningAim, false, true, null, null, null, null)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19, Monitoring.Delivery.FullyFundedLearningAim, null, null, null, null, null, null)]
        [InlineData(TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19, Monitoring.Delivery.FullyFundedLearningAim, null, null, null, null, null, null)]
        [InlineData(TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning, Monitoring.Delivery.FullyFundedLearningAim, null, null, null, null, null, null)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLARSValidity.OLASSAdult, Monitoring.Delivery.OLASSOffendersInCustody, null, null, null, null, null, null)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.AdvancedLearnerLoan, Monitoring.Delivery.FinancedByAdvancedLearnerLoans, null, null, null, null, null, null)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any, Monitoring.Delivery.FullyFundedLearningAim, null, null, null, null, null, null)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any, Monitoring.Delivery.FullyFundedLearningAim, null, null, null, null, null, null)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLARSValidity.EuropeanSocialFund, Monitoring.Delivery.FullyFundedLearningAim, null, null, null, null, null, null)]
        public void InvalidItemRaisesValidationMessage(int funding, string category, string monitor, bool? dd07Return, bool? dd11Return, bool? apprReturn, bool? stdAppReturn, bool? olassReturn, bool? componentAim)
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

            var fams = new ILearningDeliveryFAM[] { mockItem.Object };

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
                .Returns(fams);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("LearnAimRefRuleBaseTestRule", learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            // we just need to get a 'valid' category to get through the restrictions
            var mockValidity = new Mock<ILARSLearningDeliveryValidity>();
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.MinValue);
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.MaxValue);

            var larsValidities = new ILARSLearningDeliveryValidity[]
            {
                mockValidity.Object
            };

            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);

            var sut = new LearnAimRefRuleBaseTestRule(handler.Object, provider.Object, service.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
            service.VerifyAll();
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

            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);

            // we just need to get a 'valid' category to get through the restrictions
            var mockValidity = new Mock<ILARSLearningDeliveryValidity>();
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.MinValue);
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.MaxValue);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            larsValidities.Add(mockValidity.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var sut = new LearnAimRefRuleBaseTestRule(handler.Object, provider.Object, service.Object);

            // we have to ensure the secondary conditions check generate a 'pass' value
            sut.SetPassValue(true);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnAimRefRuleBaseTestRule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new LearnAimRefRuleBaseTestRule(handler.Object, provider.Object, service.Object);
        }
    }
}
