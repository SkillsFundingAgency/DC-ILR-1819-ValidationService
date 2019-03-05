using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Apprenticeship minimum start meets expectation.
        /// </summary>
        [Fact]
        public void ApprenticeshipMinimumStartMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(DateTime.Parse("2011-08-01"), LearnAimRefRuleActionProvider.ApprenticeshipMinimumStart);
        }

        /// <summary>
        /// Unemployed maximum start meets expectation.
        /// </summary>
        [Fact]
        public void UnemployedMaximumStartMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(DateTime.Parse("2016-08-01"), LearnAimRefRuleActionProvider.UnemployedMaximumStart);
        }

        /// <summary>
        /// In receipt of benefits at start meets expectation.
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void InReceiptOfBenefitsAtStartMeetsExpectation(bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            var employments = new ILearnerEmploymentStatus[] { };

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
        /// Has qualifying category (1) meets expectation
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
        public void HasQualifyingCategory1MeetsExpectation(string category, string candidate, bool expectation)
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
        /// Has qualifying category (2) meets expectation
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
        public void HasQualifyingCategory2MeetsExpectation(string category, string desiredCategory, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

            var validity = new Mock<ILARSLearningDeliveryValidity>();
            validity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);

            var larsValidities = new ILARSLearningDeliveryValidity[]
            {
                validity.Object
            };

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);

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
        /// Is qualifying category adult skills returns correct category
        /// </summary>
        [Fact]
        public void IsQualifyingCategoryAdultSkillsReturnsCorrectCategory()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 35))
                .Returns(false);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryAdultSkills(mockDelivery.Object, null);

            // assert
            Assert.Equal("ADULT_SKILLS", result.Category);
        }

        /// <summary>
        /// Is qualifying category adult skills meets expectation
        /// </summary>
        /// <param name="hasFunding">if set to <c>true</c> [has funding].</param>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="inApprenticeship">if set to <c>true</c> [in apprenticeship].</param>
        /// <param name="isInCustody">if set to <c>true</c> [is in custody].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true, false, false, false, true)]
        public void IsQualifyingCategoryAdultSkillsMeetsExpectation(bool hasFunding, bool isRestart, bool inApprenticeship, bool isInCustody, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 35))
                .Returns(hasFunding);

            var validity = new Mock<ILARSLearningDeliveryValidity>();
            validity
                .SetupGet(x => x.ValidityCategory)
                .Returns("ADULT_SKILLS");

            var larsValidities = new ILARSLearningDeliveryValidity[]
            {
                validity.Object
            };

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);

            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);
            commonOps
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(inApprenticeship);
            commonOps
                .Setup(x => x.IsLearnerInCustody(mockDelivery.Object))
                .Returns(isInCustody);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryAdultSkills(mockDelivery.Object, null);

            // assert
            service.VerifyAll();
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category apprenticeship returns correct category
        /// </summary>
        [Fact]
        public void IsQualifyingCategoryApprenticeshipReturnsCorrectCategory()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 35, 36))
                .Returns(false);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryApprenticeship(mockDelivery.Object, null);

            // assert
            Assert.Equal("APPRENTICESHIPS", result.Category);
        }

        /// <summary>
        /// Is qualifying category apprenticeship meets expectation
        /// </summary>
        /// <param name="hasFunding">if set to <c>true</c> [has funding].</param>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="inStdApp">if set to <c>true</c> [in standard application].</param>
        /// <param name="inApprenticeship">if set to <c>true</c> [in apprenticeship].</param>
        /// <param name="isComponent">if set to <c>true</c> [is component].</param>
        /// <param name="qualifyingStart">if set to <c>true</c> [qualifying start].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true, false, false, true, true, true, true)]
        public void IsQualifyingCategoryApprenticeshipMeetsExpectation(bool hasFunding, bool isRestart, bool inStdApp, bool inApprenticeship, bool isComponent, bool qualifyingStart, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 35, 36))
                .Returns(hasFunding);

            var validity = new Mock<ILARSLearningDeliveryValidity>();
            validity
                .SetupGet(x => x.ValidityCategory)
                .Returns("APPRENTICESHIPS");

            var larsValidities = new ILARSLearningDeliveryValidity[]
            {
                validity.Object
            };

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);

            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);
            commonOps
                .Setup(x => x.IsStandardApprencticeship(mockDelivery.Object))
                .Returns(inStdApp);
            commonOps
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(inApprenticeship);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(mockDelivery.Object))
                .Returns(isComponent);
            commonOps
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.Parse("2011-08-01"), DateTime.Today))
                .Returns(qualifyingStart);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryApprenticeship(mockDelivery.Object, null);

            // assert
            service.VerifyAll();
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category unemployed returns correct category
        /// </summary>
        [Fact]
        public void IsQualifyingCategoryUnemployedReturnsCorrectCategory()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 35))
                .Returns(false);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryUnemployed(mockDelivery.Object, null);

            // assert
            Assert.Equal("UNEMPLOYED", result.Category);
        }

        /// <summary>
        /// Is qualifying category unemployed meets expectation
        /// </summary>
        /// <param name="hasFunding">if set to <c>true</c> [has funding].</param>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="inApprenticeship">if set to <c>true</c> [in apprenticeship].</param>
        /// <param name="isInCustody">if set to <c>true</c> [is in custody].</param>
        /// <param name="qualifyingStart">if set to <c>true</c> [qualifying start].</param>
        /// <param name="isFundedOnBenefits">if set to <c>true</c> [is funded on benefits].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true, false, false, false, true, true, true)]
        public void IsQualifyingCategoryUnemployedMeetsExpectation(bool hasFunding, bool isRestart, bool inApprenticeship, bool isInCustody, bool qualifyingStart, bool isFundedOnBenefits, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 35))
                .Returns(hasFunding);

            var validity = new Mock<ILARSLearningDeliveryValidity>();
            validity
                .SetupGet(x => x.ValidityCategory)
                .Returns("UNEMPLOYED");

            var larsValidities = new ILARSLearningDeliveryValidity[]
            {
                validity.Object
            };

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);

            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);
            commonOps
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(inApprenticeship);
            commonOps
                .Setup(x => x.IsLearnerInCustody(mockDelivery.Object))
                .Returns(isInCustody);
            commonOps
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.MinValue, DateTime.Parse("2016-08-01")))
                .Returns(qualifyingStart);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            derivedData11
                .Setup(x => x.IsAdultFundedOnBenefitsAtStartOfAim(mockDelivery.Object, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()))
                .Returns(isFundedOnBenefits);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryUnemployed(mockDelivery.Object, null);

            // assert
            service.VerifyAll();
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category 16 to 19 efa returns correct category
        /// </summary>
        [Fact]
        public void IsQualifyingCategory16To19EFAReturnsCorrectCategory()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 25, 82))
                .Returns(false);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategory16To19EFA(mockDelivery.Object, null);

            // assert
            Assert.Equal("1619_EFA", result.Category);
        }

        /// <summary>
        /// Is qualifying category 16 to 19 efa meets expectation
        /// </summary>
        /// <param name="hasFunding">if set to <c>true</c> [has funding].</param>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true, false, true)]
        public void IsQualifyingCategory16To19EFAMeetsExpectation(bool hasFunding, bool isRestart, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 25, 82))
                .Returns(hasFunding);

            var validity = new Mock<ILARSLearningDeliveryValidity>();
            validity
                .SetupGet(x => x.ValidityCategory)
                .Returns("1619_EFA");

            var larsValidities = new ILARSLearningDeliveryValidity[]
            {
                validity.Object
            };

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);

            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategory16To19EFA(mockDelivery.Object, null);

            // assert
            service.VerifyAll();
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category community learning returns correct category
        /// </summary>
        [Fact]
        public void IsQualifyingCategoryCommunityLearningReturnsCorrectCategory()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 10))
                .Returns(false);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryCommunityLearning(mockDelivery.Object, null);

            // assert
            Assert.Equal("COMM_LEARN", result.Category);
        }

        /// <summary>
        /// Is qualifying category community learning meets expectation
        /// </summary>
        /// <param name="hasFunding">if set to <c>true</c> [has funding].</param>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true, false, true)]
        public void IsQualifyingCategoryCommunityLearningMeetsExpectation(bool hasFunding, bool isRestart, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 10))
                .Returns(hasFunding);

            var validity = new Mock<ILARSLearningDeliveryValidity>();
            validity
                .SetupGet(x => x.ValidityCategory)
                .Returns("COMM_LEARN");

            var larsValidities = new ILARSLearningDeliveryValidity[]
            {
                validity.Object
            };

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);

            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryCommunityLearning(mockDelivery.Object, null);

            // assert
            service.VerifyAll();
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category olass returns correct category
        /// </summary>
        [Fact]
        public void IsQualifyingCategoryOLASSReturnsCorrectCategory()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 35))
                .Returns(false);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryOLASS(mockDelivery.Object, null);

            // assert
            Assert.Equal("OLASS_ADULT", result.Category);
        }

        /// <summary>
        /// Is qualifying category olass meets expectation
        /// </summary>
        /// <param name="hasFunding">if set to <c>true</c> [has funding].</param>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="isInCustody">if set to <c>true</c> [is in custody].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true, false, true, true)]
        public void IsQualifyingCategoryOLASSMeetsExpectation(bool hasFunding, bool isRestart, bool isInCustody, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 35))
                .Returns(hasFunding);

            var validity = new Mock<ILARSLearningDeliveryValidity>();
            validity
                .SetupGet(x => x.ValidityCategory)
                .Returns("OLASS_ADULT");

            var larsValidities = new ILARSLearningDeliveryValidity[]
            {
                validity.Object
            };

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);

            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);
            commonOps
                .Setup(x => x.IsLearnerInCustody(mockDelivery.Object))
                .Returns(isInCustody);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryOLASS(mockDelivery.Object, null);

            // assert
            service.VerifyAll();
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category advanced learner loan returns correct category
        /// </summary>
        [Fact]
        public void IsQualifyingCategoryAdvancedLearnerLoanReturnsCorrectCategory()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 99))
                .Returns(false);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryAdvancedLearnerLoan(mockDelivery.Object, null);

            // assert
            Assert.Equal("ADV_LEARN_LOAN", result.Category);
        }

        /// <summary>
        /// Is qualifying category advanced learner loan meets expectation
        /// </summary>
        /// <param name="hasFunding">if set to <c>true</c> [has funding].</param>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="isAdvLoan">if set to <c>true</c> [is adv loan].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true, false, true, true)]
        public void IsQualifyingCategoryAdvancedLearnerLoanMeetsExpectation(bool hasFunding, bool isRestart, bool isAdvLoan, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 99))
                .Returns(hasFunding);

            var validity = new Mock<ILARSLearningDeliveryValidity>();
            validity
                .SetupGet(x => x.ValidityCategory)
                .Returns("ADV_LEARN_LOAN");

            var larsValidities = new ILARSLearningDeliveryValidity[]
            {
                validity.Object
            };

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);

            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);
            commonOps
                .Setup(x => x.IsAdvancedLearnerLoan(mockDelivery.Object))
                .Returns(isAdvLoan);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryAdvancedLearnerLoan(mockDelivery.Object, null);

            // assert
            service.VerifyAll();
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category any returns correct category
        /// </summary>
        [Fact]
        public void IsQualifyingCategoryAnyReturnsCorrectCategory()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // tries this
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 99, 81))
                .Returns(false);

            // then this...
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 36))
                .Returns(false);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryAny(mockDelivery.Object, null);

            // assert
            Assert.Equal("ANY", result.Category);
        }

        /// <summary>
        /// Is qualifying category any meets expectation
        /// </summary>
        /// <param name="hasFunding1">if set to <c>true</c> [has funding1].</param>
        /// <param name="hasFunding2">if set to <c>true</c> [has funding2].</param>
        /// <param name="isStdApp">if set to <c>true</c> [is standard application].</param>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="isAdvLoan">if set to <c>true</c> [is adv loan].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true, false, false, false, false, true)]
        [InlineData(false, true, true, false, false, true)]
        public void IsQualifyingCategoryAnyMeetsExpectation(bool hasFunding1, bool hasFunding2, bool isStdApp, bool isRestart, bool isAdvLoan, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 99, 81))
                .Returns(hasFunding1);

            if (!hasFunding1)
            {
                commonOps
                    .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 36))
                    .Returns(hasFunding2);
                commonOps
                    .Setup(x => x.IsStandardApprencticeship(mockDelivery.Object))
                    .Returns(isStdApp);
            }

            var validity = new Mock<ILARSLearningDeliveryValidity>();
            validity
                .SetupGet(x => x.ValidityCategory)
                .Returns("ANY");

            var larsValidities = new ILARSLearningDeliveryValidity[]
            {
                validity.Object
            };

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);

            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);
            commonOps
                .Setup(x => x.IsAdvancedLearnerLoan(mockDelivery.Object))
                .Returns(isAdvLoan);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryAny(mockDelivery.Object, null);

            // assert
            service.VerifyAll();
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category esf returns correct category
        /// </summary>
        [Fact]
        public void IsQualifyingCategoryESFReturnsCorrectCategory()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 70))
                .Returns(false);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryESF(mockDelivery.Object, null);

            // assert
            Assert.Equal("ESF", result.Category);
        }

        /// <summary>
        /// Is qualifying category esf meets expectation
        /// </summary>
        /// <param name="hasFunding">if set to <c>true</c> [has funding].</param>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true, false, true)]
        public void IsQualifyingCategoryESFMeetsExpectation(bool hasFunding, bool isRestart, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 70))
                .Returns(hasFunding);

            var validity = new Mock<ILARSLearningDeliveryValidity>();
            validity
                .SetupGet(x => x.ValidityCategory)
                .Returns("ESF");

            var larsValidities = new ILARSLearningDeliveryValidity[]
            {
                validity.Object
            };

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);

            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, service.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryESF(mockDelivery.Object, null);

            // assert
            service.VerifyAll();
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// New service.
        /// </summary>
        /// <returns>a constructed and mocked up service</returns>
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
