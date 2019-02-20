﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.UKPRN
{
    public class UKPRN_08RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_08Rule(null, fileData.Object, academicData.Object, commonOps.Object, fcsData.Object));
        }

        /// <summary>
        /// New rule with null file data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullFileDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_08Rule(handler.Object, null, academicData.Object, commonOps.Object, fcsData.Object));
        }

        /// <summary>
        /// New rule with null academic data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullAcademicDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_08Rule(handler.Object, fileData.Object, null, commonOps.Object, fcsData.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_08Rule(handler.Object, fileData.Object, academicData.Object, null, fcsData.Object));
        }

        /// <summary>
        /// New rule with null FCS data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullFCSDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_08Rule(handler.Object, fileData.Object, academicData.Object, commonOps.Object, null));
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
            Assert.Equal("UKPRN_08", result);
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
            Assert.Equal(RuleNameConstants.UKPRN_08, result);
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
        /// Is not part of this year meet expectdation
        /// </summary>
        /// <param name="endDate">The end date.</param>
        /// <param name="commencementDate">The commencement date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2016-02-01", "2016-04-04", true)]
        [InlineData("2016-04-03", "2016-04-04", true)]
        [InlineData("2016-04-04", "2016-04-04", false)]
        [InlineData("2016-04-05", "2016-04-04", false)]
        public void IsNotPartOfThisYearMeetExpectdation(string endDate, string commencementDate, bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.LearnActEndDateNullable)
                .Returns(DateTime.Parse(endDate));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicData
                .SetupGet(x => x.Today)
                .Returns(DateTime.Today);
            academicData
                .Setup(x => x.GetAcademicYearOfLearningDate(DateTime.Today, AcademicYearDates.Commencement))
                .Returns(DateTime.Parse(commencementDate));

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new UKPRN_08Rule(handler.Object, fileData.Object, academicData.Object, commonOps.Object, fcsData.Object);

            // act
            var result = sut.IsNotPartOfThisYear(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            fileData.VerifyAll();
            academicData.VerifyAll();
            commonOps.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Has qualifying provider identifier meets expectation
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="deliveryID">The delivery identifier.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(1, 2, false)]
        public void HasQualifyingProviderIDMeetsExpectation(int provider, int deliveryID, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IFcsContractAllocation>();
            mockItem
                .SetupGet(y => y.DeliveryUKPRN)
                .Returns(deliveryID);

            // act
            var result = sut.HasQualifyingProviderID(mockItem.Object, provider);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying funding stream meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("AEBC1819", false)] // FundingStreamPeriodCodeConstants.AEBC1819
        [InlineData("AEBTO-TOL1819", false)] // FundingStreamPeriodCodeConstants.AEBTO_TOL1819
        [InlineData("AEB-LS1819", false)] // FundingStreamPeriodCodeConstants.AEB_LS1819
        [InlineData("AEB-TOL1819", false)] // FundingStreamPeriodCodeConstants.AEB_TOL1819
        [InlineData("ALLB1819", true)] // FundingStreamPeriodCodeConstants.ALLB1819
        [InlineData("ALLBC1819", true)] // FundingStreamPeriodCodeConstants.ALLBC1819
        [InlineData("ANLAP2018", false)] // FundingStreamPeriodCodeConstants.ANLAP2018
        [InlineData("APPS1819", false)] // FundingStreamPeriodCodeConstants.APPS1819
        [InlineData("16-18NLAP2018", false)] // FundingStreamPeriodCodeConstants.C1618_NLAP2018
        [InlineData("ESF1420", false)] // FundingStreamPeriodCodeConstants.ESF1420
        [InlineData("LEVY1799", false)] // FundingStreamPeriodCodeConstants.LEVY1799
        public void HasQualifyingFundingStreamMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IFcsContractAllocation>();
            mockItem
                .SetupGet(y => y.FundingStreamPeriodCode)
                .Returns(candidate);

            // act
            var result = sut.HasQualifyingFundingStream(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has funding relationship with null delivery returns false
        /// </summary>
        [Fact]
        public void HasFundingRelationshipWithNullDeliveryReturnsFalse()
        {
            // arrange
            const int testUKPRN = 123;

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(testUKPRN);

            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(testUKPRN))
                .Returns((IReadOnlyCollection<IFcsContractAllocation>)null);

            var sut = new UKPRN_08Rule(handler.Object, fileData.Object, academicData.Object, commonOps.Object, fcsData.Object);

            // act
            var result = sut.HasFundingRelationship(null);

            // assert
            Assert.False(result);

            handler.VerifyAll();
            fileData.VerifyAll();
            academicData.VerifyAll();
            commonOps.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("AEBC1819")] // FundingStreamPeriodCodeConstants.AEBC1819
        [InlineData("AEBTO-TOL1819")] // FundingStreamPeriodCodeConstants.AEBTO_TOL1819
        [InlineData("AEB-LS1819")] // FundingStreamPeriodCodeConstants.AEB_LS1819
        [InlineData("AEB-TOL1819")] // FundingStreamPeriodCodeConstants.AEB_TOL1819
        [InlineData("ANLAP2018")] // FundingStreamPeriodCodeConstants.ANLAP2018
        [InlineData("APPS1819")] // FundingStreamPeriodCodeConstants.APPS1819
        [InlineData("16-18NLAP2018")] // FundingStreamPeriodCodeConstants.C1618_NLAP2018
        [InlineData("ESF1420")] // FundingStreamPeriodCodeConstants.ESF1420
        [InlineData("LEVY1799")] // FundingStreamPeriodCodeConstants.LEVY1799
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string contractRef = "shonkyRef12";
            const int providerID = 123;

            var testDate = DateTime.Today;

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(contractRef);
            delivery
                .SetupGet(y => y.LearnActEndDateNullable)
                .Returns(testDate);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.UKPRN_08, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("UKPRN", providerID))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", "ALB"))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(providerID);

            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicData
                .SetupGet(x => x.Today)
                .Returns(DateTime.Today);
            academicData
                .Setup(x => x.GetAcademicYearOfLearningDate(DateTime.Today, AcademicYearDates.Commencement))
                .Returns(testDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsLoansBursary(delivery.Object))
                .Returns(true);

            var allocation = new Mock<IFcsContractAllocation>();
            allocation
                .SetupGet(x => x.FundingStreamPeriodCode)
                .Returns(candidate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(providerID))
                .Returns(new[] { allocation.Object });

            var sut = new UKPRN_08Rule(handler.Object, fileData.Object, academicData.Object, commonOps.Object, fcsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fileData.VerifyAll();
            academicData.VerifyAll();
            commonOps.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("ALLB1819")] // FundingStreamPeriodCodeConstants.ALLB1819
        [InlineData("ALLBC1819")] // FundingStreamPeriodCodeConstants.ALLBC1819
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string contractRef = "shonkyRef12";
            const int providerID = 123;

            var testDate = DateTime.Today;

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(contractRef);
            delivery
                .SetupGet(y => y.LearnActEndDateNullable)
                .Returns(testDate);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(providerID);

            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicData
                .SetupGet(x => x.Today)
                .Returns(DateTime.Today);
            academicData
                .Setup(x => x.GetAcademicYearOfLearningDate(DateTime.Today, AcademicYearDates.Commencement))
                .Returns(testDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsLoansBursary(delivery.Object))
                .Returns(true);

            var allocation = new Mock<IFcsContractAllocation>();
            allocation
                .SetupGet(x => x.FundingStreamPeriodCode)
                .Returns(candidate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(providerID))
                .Returns(new[] { allocation.Object });

            var sut = new UKPRN_08Rule(handler.Object, fileData.Object, academicData.Object, commonOps.Object, fcsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fileData.VerifyAll();
            academicData.VerifyAll();
            commonOps.VerifyAll();
            fcsData.VerifyAll();
        }

        private UKPRN_08Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            return new UKPRN_08Rule(handler.Object, fileData.Object, academicData.Object, commonOps.Object, fcsData.Object);
        }
    }
}
