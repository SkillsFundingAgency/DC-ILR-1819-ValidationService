using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.DelLocPostCode
{
    public class DelLocPostCode_17RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new DelLocPostCode_17Rule(null, common.Object, fcsData.Object, postcodes.Object, ddRule22.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new DelLocPostCode_17Rule(handler.Object, null, fcsData.Object, postcodes.Object, ddRule22.Object));
        }

        /// <summary>
        /// New rule with null contract data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullContractDataThrows()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new DelLocPostCode_17Rule(handler.Object, common.Object, null, postcodes.Object, ddRule22.Object));
        }

        /// <summary>
        /// New rule with null postcode data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullPostcodeDataThrows()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, null, ddRule22.Object));
        }

        /// <summary>
        /// New rule with null derived data rule throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRuleThrows()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object, null));
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
            Assert.Equal("DelLocPostCode_17", result);
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
            Assert.Equal(DelLocPostCode_17Rule.Name, result);
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

        [Fact]
        public void GetContractCompletionDateMeetsExpectation()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(null, null))
                .Returns((DateTime?)null);

            var sut = new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object, ddRule22.Object);

            // act
            var result = sut.GetContractCompletionDate(null, null);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
            fcsData.VerifyAll();
            postcodes.VerifyAll();
            ddRule22.VerifyAll();

            Assert.Null(result);
        }

        [Fact]
        public void GetLatestStartForCompletedContractMeetsNullExpectation()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            var sut = new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object, ddRule22.Object);

            // act
            var result = sut.GetLatestStartForCompletedContract(null);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
            fcsData.VerifyAll();
            postcodes.VerifyAll();
            ddRule22.VerifyAll();

            Assert.Null(result);
        }

        [Fact]
        public void GetLatestStartForCompletedContractMeetsEmptyExpectation()
        {
            // arrange
            var deliveries = Collection.EmptyAndReadOnly<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            var sut = new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object, ddRule22.Object);

            // act
            var result = sut.GetLatestStartForCompletedContract(deliveries);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
            fcsData.VerifyAll();
            postcodes.VerifyAll();
            ddRule22.VerifyAll();

            Assert.Null(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void GetLatestStartForCompletedContractMeetsCallCountExpectation(int callCount)
        {
            // arrange
            var deliveries = Collection.Empty<ILearningDelivery>();
            for (int i = 0; i < callCount; i++)
            {
                deliveries.Add(new Mock<ILearningDelivery>().Object);
            }

            var safedeliveries = deliveries.AsSafeReadOnlyList();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(Moq.It.IsAny<ILearningDelivery>(), safedeliveries))
                .Returns((DateTime?)null);

            var sut = new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object, ddRule22.Object);

            // act
            var result = sut.GetLatestStartForCompletedContract(safedeliveries);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
            fcsData.VerifyAll();
            postcodes.VerifyAll();
            ddRule22.VerifyAll();

            ddRule22
                .Verify(x => x.GetLatestLearningStartForESFContract(Moq.It.IsAny<ILearningDelivery>(), safedeliveries), Times.Exactly(callCount));

            Assert.Null(result);
        }

        [Theory]
        [InlineData("TestThingy1")]
        [InlineData("TestThingy2")]
        [InlineData("TestThingy3")]
        [InlineData("TestThingy4")]
        public void GetLocalAuthority(string candidate)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleLocalAuthorityFor(candidate))
                .Returns(new Mock<IEsfEligibilityRuleLocalAuthority>().Object);

            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            var sut = new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object, ddRule22.Object);

            // act
            var result = sut.GetLocalAuthority(delivery.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
            fcsData.VerifyAll();
            postcodes.VerifyAll();
            ddRule22.VerifyAll();

            Assert.IsAssignableFrom<IEsfEligibilityRuleLocalAuthority>(result);
        }

        [Theory]
        [InlineData("TestThingy1")]
        [InlineData("TestThingy2")]
        [InlineData("TestThingy3")]
        [InlineData("TestThingy4")]
        public void GetONSPostcodeMeetsExpectation(string candidate)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.DelLocPostCode)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            postcodes
                .Setup(x => x.GetONSPostcode(candidate))
                .Returns(new Mock<IONSPostcode>().Object);

            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            var sut = new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object, ddRule22.Object);

            // act
            var result = sut.GetONSPostcode(delivery.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
            fcsData.VerifyAll();
            postcodes.VerifyAll();
            ddRule22.VerifyAll();

            Assert.IsAssignableFrom<IONSPostcode>(result);
        }

        [Fact]
        public void HasQualifyingLocalAuthorityMeetsNullPostcodeExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasQualifyingLocalAuthority(null, new Mock<IEsfEligibilityRuleLocalAuthority>().Object);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void HasQualifyingLocalAuthorityMeetsNullEligibilityExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasQualifyingLocalAuthority(new Mock<IONSPostcode>().Object, null);

            // assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("ESF0002", "tt_9972", false)]
        [InlineData("tt_9972", "ESF0002", false)]
        [InlineData("TT_9972", "tt_9972", true)]
        [InlineData("tt_9972", "TT_9972", true)]
        [InlineData("tt_9972", "tt_9972", true)]
        [InlineData("TT_9973", "tt_9972", false)]
        [InlineData("tt_9972", "TT_9973", false)]
        [InlineData("tt_9973", "tt_9972", false)]
        [InlineData("tt_9972", "tt_9973", false)]
        public void HasQualifyingLocalAuthorityMeetsExpectation(string elAuthority, string pcAuthority, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var postcode = new Mock<IONSPostcode>();
            postcode
                .SetupGet(x => x.LocalAuthority)
                .Returns(pcAuthority);
            var authority = new Mock<IEsfEligibilityRuleLocalAuthority>();
            authority
                .SetupGet(x => x.Code)
                .Returns(elAuthority);

            // act
            var result = sut.HasQualifyingLocalAuthority(postcode.Object, authority.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2016-02-28", "2016-03-01", "2016-03-10", false)]
        [InlineData("2016-02-28", "2016-03-01", null, false)]
        [InlineData("2016-02-28", "2016-02-28", "2016-03-01", true)]
        [InlineData("2016-02-28", "2016-02-27", "2016-03-01", true)]
        [InlineData("2016-02-28", "2016-02-28", null, true)]
        [InlineData("2016-02-28", "2016-02-27", null, true)]
        public void IsQualifyingAuthorityMeetsExpectation(string startDate, string from, string to, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            var toDate = string.IsNullOrWhiteSpace(to)
                ? (DateTime?)null
                : DateTime.Parse(to);

            var postcode = new Mock<IONSPostcode>();
            postcode
                .SetupGet(x => x.EffectiveFrom)
                .Returns(DateTime.Parse(from));
            postcode
                .SetupGet(x => x.EffectiveTo)
                .Returns(toDate);

            // act
            var result = sut.InQualifyingPeriod(mockDelivery.Object, postcode.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        [Theory]
        [InlineData("2016-02-28", "2016-03-01", "2016-03-10")]
        [InlineData("2016-02-28", "2016-03-01", null)]
        public void InvalidItemRaisesValidationMessage(string startDate, string from, string to)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string localAuthority = "LA0001";
            const string delLocPC = "testPostcode";
            const string conRefNum = "tt_1234";
            const string learnAimRef = "shonkyRefCode";

            var learnStart = DateTime.Parse(startDate);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.EuropeanSocialFund);
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(conRefNum);
            mockDelivery
                .SetupGet(x => x.DelLocPostCode)
                .Returns(delLocPC);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStart);

            var toDate = string.IsNullOrWhiteSpace(to)
                ? (DateTime?)null
                : DateTime.Parse(to);

            var postcode = new Mock<IONSPostcode>();
            postcode
                .SetupGet(x => x.LocalAuthority)
                .Returns(localAuthority);
            postcode
                .SetupGet(x => x.EffectiveFrom)
                .Returns(DateTime.Parse(from));
            postcode
                .SetupGet(x => x.EffectiveTo)
                .Returns(toDate);

            var authority = new Mock<IEsfEligibilityRuleLocalAuthority>();
            authority
                .SetupGet(x => x.Code)
                .Returns(localAuthority);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            var safedeliveries = deliveries.AsSafeReadOnlyList();

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(safedeliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(DelLocPostCode_17Rule.Name, learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", TypeOfFunding.EuropeanSocialFund))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("DelLocPostCode", delLocPC))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ConRefNumber", conRefNum))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DelLocPostCode_17Rule.FirstViableDate, null))
                .Returns(true);
            common
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.EuropeanSocialFund))
                .Returns(true);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleLocalAuthorityFor(conRefNum))
                .Returns(authority.Object);

            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            postcodes
                .Setup(x => x.GetONSPostcode(delLocPC))
                .Returns(postcode.Object);

            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(mockDelivery.Object, safedeliveries))
                .Returns(learnStart);

            var sut = new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object, ddRule22.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
            fcsData.VerifyAll();
            postcodes.VerifyAll();
            ddRule22.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        [Theory]
        [InlineData("2016-02-28", "2016-02-28", "2016-03-01")]
        [InlineData("2016-02-28", "2016-02-27", "2016-03-01")]
        [InlineData("2016-02-28", "2016-02-28", null)]
        [InlineData("2016-02-28", "2016-02-27", null)]
        public void ValidItemDoesNotRaiseAValidationMessage(string startDate, string from, string to)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string localAuthority = "LA0001";
            const string delLocPC = "testPostcode";
            const string conRefNum = "tt_1234";
            const string learnAimRef = "shonkyRefCode";

            var learnStart = DateTime.Parse(startDate);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.EuropeanSocialFund);
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(conRefNum);
            mockDelivery
                .SetupGet(x => x.DelLocPostCode)
                .Returns(delLocPC);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStart);

            var toDate = string.IsNullOrWhiteSpace(to)
                ? (DateTime?)null
                : DateTime.Parse(to);

            var postcode = new Mock<IONSPostcode>();
            postcode
                .SetupGet(x => x.LocalAuthority)
                .Returns(localAuthority);
            postcode
                .SetupGet(x => x.EffectiveFrom)
                .Returns(DateTime.Parse(from));
            postcode
                .SetupGet(x => x.EffectiveTo)
                .Returns(toDate);

            var authority = new Mock<IEsfEligibilityRuleLocalAuthority>();
            authority
                .SetupGet(x => x.Code)
                .Returns(localAuthority);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            var safedeliveries = deliveries.AsSafeReadOnlyList();

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(safedeliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DelLocPostCode_17Rule.FirstViableDate, null))
                .Returns(true);
            common
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.EuropeanSocialFund))
                .Returns(true);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleLocalAuthorityFor(conRefNum))
                .Returns(authority.Object);

            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            postcodes
                .Setup(x => x.GetONSPostcode(delLocPC))
                .Returns(postcode.Object);

            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(mockDelivery.Object, safedeliveries))
                .Returns(learnStart);

            var sut = new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object, ddRule22.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
            fcsData.VerifyAll();
            postcodes.VerifyAll();
            ddRule22.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public DelLocPostCode_17Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            return new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object, ddRule22.Object);
        }
    }
}
