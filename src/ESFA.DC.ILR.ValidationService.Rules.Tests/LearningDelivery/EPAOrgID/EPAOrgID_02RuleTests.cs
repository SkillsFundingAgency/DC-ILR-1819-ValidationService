using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EPAOrgID;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.EPAOrgID
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// </summary>
    public class EPAOrgID_02RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var mockService = new Mock<IProvideEPAOrganisationDetails>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EPAOrgID_02Rule(null, mockService.Object));
        }

        /// <summary>
        /// New rule with null EPA details provider throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullEPADetailsProviderThrows()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EPAOrgID_02Rule(mockHandler.Object, null));
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
            Assert.Equal("EPAOrgID_02", result);
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
            Assert.Equal(EPAOrgID_02Rule.Name, result);
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
        /// Is assessment price meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(ApprenticeshipFinanicalRecord.AssessmentPayment, false)]
        [InlineData(ApprenticeshipFinanicalRecord.EmployerPaymentReimbursedByProvider, false)]
        [InlineData(ApprenticeshipFinanicalRecord.ResidualAssessmentPrice, true)]
        [InlineData(ApprenticeshipFinanicalRecord.ResidualTrainingPrice, false)]
        [InlineData(ApprenticeshipFinanicalRecord.TotalAssessmentPrice, true)]
        [InlineData(ApprenticeshipFinanicalRecord.TotalTrainingPrice, false)]
        [InlineData(ApprenticeshipFinanicalRecord.TrainingPayment, false)]
        [InlineData("NOT1", false)]
        public void IsAssessmentPriceMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IAppFinRecord>();
            mockItem
                .SetupGet(y => y.AFinType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.AFinCode)
                .Returns(int.Parse(candidate.Substring(3)));

            // act
            var result = sut.IsAssessmentPrice(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has assessment price with null application fin records returns false
        /// </summary>
        [Fact]
        public void HasAssessmentPriceWithNullAppFinRecordsReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();

            // act
            var result = sut.HasAssessmentPrice(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has assessment price with empty application fin records returns false
        /// </summary>
        [Fact]
        public void HasAssessmentPriceWithEmptyAppFinRecordsReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.AppFinRecords)
                .Returns(Collection.EmptyAndReadOnly<IAppFinRecord>());

            // act
            var result = sut.HasAssessmentPrice(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="financeType">Type of the finance.</param>
        /// <param name="epaOrgID">The epa org identifier.</param>
        [Theory]
        [InlineData(ApprenticeshipFinanicalRecord.ResidualAssessmentPrice, null)]
        [InlineData(ApprenticeshipFinanicalRecord.TotalAssessmentPrice, null)]
        [InlineData(ApprenticeshipFinanicalRecord.ResidualAssessmentPrice, "EPA0003")]
        [InlineData(ApprenticeshipFinanicalRecord.TotalAssessmentPrice, "EPA0004")]
        public void InvalidItemRaisesValidationMessage(string financeType, string epaOrgID)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockItem = new Mock<IAppFinRecord>();
            mockItem
                .SetupGet(y => y.AFinType)
                .Returns(financeType.Substring(0, 3));
            mockItem
                .SetupGet(y => y.AFinCode)
                .Returns(int.Parse(financeType.Substring(3)));

            var records = Collection.Empty<IAppFinRecord>();
            records.Add(mockItem.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.EPAOrgID)
                .Returns(epaOrgID);
            mockDelivery
                .SetupGet(y => y.AppFinRecords)
                .Returns(records.AsSafeReadOnlyList());

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
                    Moq.It.Is<string>(y => y == EPAOrgID_02Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == EPAOrgID_02Rule.MessagePropertyName),
                    Moq.It.IsAny<ILearningDelivery>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var service = new Mock<IProvideEPAOrganisationDetails>(MockBehavior.Strict);
            service
                .Setup(x => x.IsKnown(epaOrgID))
                .Returns(false);

            var sut = new EPAOrgID_02Rule(handler.Object, service.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="financeType">Type of the finance.</param>
        /// <param name="epaOrgID">The epa org identifier.</param>
        /// <param name="isKnownRequired">if set to <c>true</c> [is known required].</param>
        [Theory]
        [InlineData(ApprenticeshipFinanicalRecord.AssessmentPayment, "EPA0001", false)]
        [InlineData(ApprenticeshipFinanicalRecord.EmployerPaymentReimbursedByProvider, "EPA0002", false)]
        [InlineData(ApprenticeshipFinanicalRecord.ResidualAssessmentPrice, "EPA0003", true)]
        [InlineData(ApprenticeshipFinanicalRecord.ResidualTrainingPrice, "EPA0004", false)]
        [InlineData(ApprenticeshipFinanicalRecord.TotalAssessmentPrice, "EPA0005", true)]
        [InlineData(ApprenticeshipFinanicalRecord.TotalTrainingPrice, "EPA0006", false)]
        [InlineData(ApprenticeshipFinanicalRecord.TrainingPayment, "EPA0007", false)]
        [InlineData("NOT1", "EPA0008", false)]
        public void ValidItemDoesNotRaiseAValidationMessage(string financeType, string epaOrgID, bool isKnownRequired)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockItem = new Mock<IAppFinRecord>();
            mockItem
                .SetupGet(y => y.AFinType)
                .Returns(financeType.Substring(0, 3));
            mockItem
                .SetupGet(y => y.AFinCode)
                .Returns(int.Parse(financeType.Substring(3)));

            var records = Collection.Empty<IAppFinRecord>();
            records.Add(mockItem.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.EPAOrgID)
                .Returns(epaOrgID);
            mockDelivery
                .SetupGet(y => y.AppFinRecords)
                .Returns(records.AsSafeReadOnlyList());

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
            var service = new Mock<IProvideEPAOrganisationDetails>(MockBehavior.Strict);
            if (isKnownRequired)
            {
                service
                    .Setup(x => x.IsKnown(epaOrgID))
                    .Returns(true);
            }

            var sut = new EPAOrgID_02Rule(handler.Object, service.Object);

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
        public EPAOrgID_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IProvideEPAOrganisationDetails>(MockBehavior.Strict);

            return new EPAOrgID_02Rule(handler.Object, service.Object);
        }
    }
}
