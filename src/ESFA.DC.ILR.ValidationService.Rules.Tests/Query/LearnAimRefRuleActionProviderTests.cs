using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearnAimRefRuleActionProviderTests
    {
        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleActionProvider(null, service.Object, derivedData11.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleActionProvider(commonOps.Object, null, derivedData11.Object));
        }

        /// <summary>
        /// New rule with null derived data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataThrows()
        {
            // arrange
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, null));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void InReceiptOfBenefitsAtStartMeetsExpectation(bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            var employments = Collection.EmptyAndReadOnly<ILearnerEmploymentStatus>();

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            derivedData11
                .Setup(x => x.IsAdultFundedOnBenefitsAtStartOfAim(mockDelivery.Object, employments))
                .Returns(expectation);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.InReceiptOfBenefitsAtStart(mockDelivery.Object, employments);

            // assert
            commonOps.VerifyAll();
            service.VerifyAll();
            derivedData11.VerifyAll();

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
            var sut = NewService();

            var mockValidity = new Mock<ILARSLearningDeliveryValidity>();
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

            var mockValidity = new Mock<ILARSLearningDeliveryValidity>();
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            larsValidities.Add(mockValidity.Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.HasQualifyingCategory(mockDelivery.Object, desiredCategory);

            // assert
            service.VerifyAll();
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnAimRefRuleActionProvider NewService()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            return new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);
        }
    }
}
