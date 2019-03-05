using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_14RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var ddRule18 = new Mock<IDerivedData_18Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_14Rule(null, larsData.Object, ddRule18.Object, commonOps.Object));
        }

        /// <summary>
        /// New rule with null lars data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var ddRule18 = new Mock<IDerivedData_18Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_14Rule(handler.Object, null, ddRule18.Object, commonOps.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 18 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule18Throws()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var ddRule18 = new Mock<IDerivedData_18Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_14Rule(handler.Object, larsData.Object, null, commonOps.Object));
        }

        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var ddRule18 = new Mock<IDerivedData_18Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_14Rule(handler.Object, larsData.Object, ddRule18.Object, null));
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
            Assert.Equal("LearnStartDate_14", result);
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
            Assert.Equal(LearnStartDate_14Rule.Name, result);
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
        /// Gets start for meets expection.
        /// </summary>
        [Fact]
        public void GetStartForMeetsExpection()
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            var deliveries = Collection.EmptyAndReadOnly<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var ddRule18 = new Mock<IDerivedData_18Rule>(MockBehavior.Strict);
            ddRule18
                .Setup(x => x.GetApprenticeshipStandardProgrammeStartDateFor(delivery.Object, deliveries))
                .Returns(DateTime.Today);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnStartDate_14Rule(handler.Object, larsData.Object, ddRule18.Object, commonOps.Object);

            // act
            var result = sut.GetStartFor(delivery.Object, deliveries);

            // assert
            Assert.Equal(DateTime.Today, result);

            handler.VerifyAll();
            larsData.VerifyAll();
            ddRule18.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Get periods of validity for meets expection.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(1)]
        [InlineData(123)]
        [InlineData(123456)]
        public void GetPeriodsOfValidityForMeetsExpection(int candidate)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetStandardValiditiesFor(candidate))
                .Returns(Collection.EmptyAndReadOnly<ILARSStandardValidity>());
            var ddRule18 = new Mock<IDerivedData_18Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnStartDate_14Rule(handler.Object, larsData.Object, ddRule18.Object, commonOps.Object);

            // act
            var result = sut.GetPeriodsOfValidityFor(delivery.Object);

            // assert
            Assert.Empty(result);

            handler.VerifyAll();
            larsData.VerifyAll();
            ddRule18.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has standard code meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, false)]
        [InlineData(1, true)]
        [InlineData(123, true)]
        [InlineData(123456, true)]
        public void HasStandardCodeMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.StdCodeNullable)
                .Returns(candidate);

            // act
            var result = sut.HasStandardCode(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying period of validity meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start date.</param>
        /// <param name="end">The end date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, "2018-01-01", null, true)]
        [InlineData("2017-12-31", "2018-01-01", null, false)]
        [InlineData("2018-01-01", "2018-01-01", null, true)]
        [InlineData("2018-12-31", "2018-01-01", null, true)]
        [InlineData("2018-12-31", "2018-01-01", "2018-12-31", true)]
        [InlineData("2018-12-31", "2018-01-01", "2018-12-30", false)]
        public void HasQualifyingPeriodOfValidityMeetsExpectation(string candidate, string start, string end, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var testDate = string.IsNullOrWhiteSpace(candidate)
                ? (DateTime?)null
                : DateTime.Parse(candidate);
            var startDate = DateTime.Parse(start);
            var endDate = string.IsNullOrWhiteSpace(end)
                ? (DateTime?)null
                : DateTime.Parse(end);

            var validities = Collection.Empty<ILARSStandardValidity>();
            var validity = new Mock<ILARSStandardValidity>();
            validity.SetupGet(x => x.StartDate).Returns(startDate);
            validity.SetupGet(x => x.EndDate).Returns(endDate);
            validities.Add(validity.Object);

            // act
            var result = sut.HasQualifyingPeriodOfValidity(testDate, validities.AsSafeReadOnlyList());

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        [Theory]
        [InlineData("2017-12-31", "2018-01-01", null)]
        [InlineData("2018-12-31", "2018-01-01", "2018-12-30")]
        public void InvalidItemRaisesValidationMessage(string candidate, string start, string end)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const int stdCodeForTest = 14; // <= any old code for the purpose of the test...

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.StdCodeNullable)
                .Returns(stdCodeForTest);
            delivery.SetupGet(y => y.AimType).Returns(3);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(delivery.Object);
            var safeDeliveries = deliveries.AsSafeReadOnlyList();

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(safeDeliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(LearnStartDate_14Rule.Name, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("StdCode", stdCodeForTest))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var startDate = DateTime.Parse(start);
            var endDate = string.IsNullOrWhiteSpace(end)
                ? (DateTime?)null
                : DateTime.Parse(end);

            var validities = Collection.Empty<ILARSStandardValidity>();
            var validity = new Mock<ILARSStandardValidity>();
            validity.SetupGet(x => x.StartDate).Returns(startDate);
            validity.SetupGet(x => x.EndDate).Returns(endDate);
            validities.Add(validity.Object);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetStandardValiditiesFor(stdCodeForTest))
                .Returns(validities.AsSafeReadOnlyList());

            var testDate = string.IsNullOrWhiteSpace(candidate)
                ? (DateTime?)null
                : DateTime.Parse(candidate);

            var ddRule18 = new Mock<IDerivedData_18Rule>(MockBehavior.Strict);
            ddRule18
                .Setup(x => x.GetApprenticeshipStandardProgrammeStartDateFor(delivery.Object, safeDeliveries))
                .Returns(testDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps.Setup(x => x.IsRestart(delivery.Object)).Returns(false);
            commonOps.Setup(x => x.IsStandardApprencticeship(delivery.Object)).Returns(true);

            var sut = new LearnStartDate_14Rule(handler.Object, larsData.Object, ddRule18.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            larsData.VerifyAll();
            ddRule18.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        [Theory]
        [InlineData("2018-01-01", "2018-01-01", null)]
        [InlineData("2018-12-31", "2018-01-01", null)]
        [InlineData("2018-12-31", "2018-01-01", "2018-12-31")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, string start, string end)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const int stdCodeForTest = 14; // <= any old code for the purpose of the test...

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.StdCodeNullable)
                .Returns(stdCodeForTest);
            delivery.SetupGet(y => y.AimType).Returns(3);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(delivery.Object);
            var safeDeliveries = deliveries.AsSafeReadOnlyList();

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(safeDeliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var startDate = DateTime.Parse(start);
            var endDate = string.IsNullOrWhiteSpace(end)
                ? (DateTime?)null
                : DateTime.Parse(end);

            var validities = Collection.Empty<ILARSStandardValidity>();
            var validity = new Mock<ILARSStandardValidity>();
            validity.SetupGet(x => x.StartDate).Returns(startDate);
            validity.SetupGet(x => x.EndDate).Returns(endDate);
            validities.Add(validity.Object);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetStandardValiditiesFor(stdCodeForTest))
                .Returns(validities.AsSafeReadOnlyList());

            var testDate = string.IsNullOrWhiteSpace(candidate)
                ? (DateTime?)null
                : DateTime.Parse(candidate);

            var ddRule18 = new Mock<IDerivedData_18Rule>(MockBehavior.Strict);
            ddRule18
                .Setup(x => x.GetApprenticeshipStandardProgrammeStartDateFor(delivery.Object, safeDeliveries))
                .Returns(testDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps.Setup(x => x.IsRestart(delivery.Object)).Returns(false);
            commonOps.Setup(x => x.IsStandardApprencticeship(delivery.Object)).Returns(true);

            var sut = new LearnStartDate_14Rule(handler.Object, larsData.Object, ddRule18.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            larsData.VerifyAll();
            ddRule18.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnStartDate_14Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var ddRule18 = new Mock<IDerivedData_18Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnStartDate_14Rule(handler.Object, larsData.Object, ddRule18.Object, commonOps.Object);
        }
    }
}
