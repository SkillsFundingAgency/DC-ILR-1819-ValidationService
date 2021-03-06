﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
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
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_78Rule(null, service.Object, commonChecks.Object, fileData.Object, orgData.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_78Rule(handler.Object, null, commonChecks.Object, fileData.Object, orgData.Object));
        }

        /// <summary>
        /// New rule with null common checks throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonchecksThrows()
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
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_78Rule(handler.Object, service.Object, commonChecks.Object, null, orgData.Object));
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
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_78Rule(handler.Object, service.Object, commonChecks.Object, fileData.Object, null));
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
            // arrange / act
            var result = LearnAimRef_78Rule.FirstViableDate;

            // assert
            Assert.Equal(DateTime.Parse("2016-08-01"), result);
        }

        /// <summary>
        /// Last viable date meets expectation.
        /// </summary>
        [Fact]
        public void LastViableDateMeetsExpectation()
        {
            // arrange / act
            var result = LearnAimRef_78Rule.LastViableDate;

            // assert
            Assert.Equal(DateTime.Parse("2017-07-31"), result);
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
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(ukprn);

            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            orgData
                .Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, "USDC"))
                .Returns(expectation);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, commonChecks.Object, fileData.Object, orgData.Object);

            // act
            var result = sut.IsSpecialistDesignatedCollege();

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
            fileData.VerifyAll();
            orgData.VerifyAll();

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
                .Setup(x => x.GetDeliveryFor(candidate))
                .Returns((ILARSLearningDelivery)null);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, commonChecks.Object, fileData.Object, orgData.Object);

            // act
            var result = sut.HasQualifyingNotionalNVQ(mockDelivery.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
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
                .Setup(x => x.GetCategoriesFor(candidate))
                .Returns(Collection.EmptyAndReadOnly<ILARSLearningCategory>());

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, commonChecks.Object, fileData.Object, orgData.Object);

            // act
            var result = sut.HasQualifyingCategory(mockDelivery.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
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

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLars.Object);
            service
                .Setup(x => x.GetCategoriesFor(learnAimRef))
                .Returns(larsCategories.AsSafeReadOnlyList());

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.IsLearnerInCustody(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.IsSteelWorkerRedundancyTraining(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.AdultSkills))
                .Returns(true);
            commonChecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, LearnAimRef_78Rule.FirstViableDate, LearnAimRef_78Rule.LastViableDate))
                .Returns(true);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(1004);

            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            orgData
                .Setup(x => x.LegalOrgTypeMatchForUkprn(1004, "USDC"))
                .Returns(false);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, commonChecks.Object, fileData.Object, orgData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
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

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLars.Object);
            service
                .Setup(x => x.GetCategoriesFor(learnAimRef))
                .Returns(larsCategories.AsSafeReadOnlyList());

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.IsLearnerInCustody(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.IsSteelWorkerRedundancyTraining(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.AdultSkills))
                .Returns(true);
            commonChecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, LearnAimRef_78Rule.FirstViableDate, LearnAimRef_78Rule.LastViableDate))
                .Returns(true);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(1004);

            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            orgData
                .Setup(x => x.LegalOrgTypeMatchForUkprn(1004, "USDC"))
                .Returns(false);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, commonChecks.Object, fileData.Object, orgData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
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
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);

            return new LearnAimRef_78Rule(handler.Object, service.Object, commonChecks.Object, fileData.Object, orgData.Object);
        }
    }
}
