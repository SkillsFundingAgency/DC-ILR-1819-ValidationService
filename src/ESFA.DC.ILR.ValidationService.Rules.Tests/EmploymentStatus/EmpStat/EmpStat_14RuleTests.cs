using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_14RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_14Rule(null, ddRule22.Object, fcsData.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 22 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule22Throws()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_14Rule(handler.Object, null, fcsData.Object));
        }

        [Fact]
        public void NewRuleWithNullFCSDataServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_14Rule(handler.Object, ddRule22.Object, null));
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
            Assert.Equal("EmpStat_14", result);
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
            Assert.Equal(EmpStat_14Rule.Name, result);
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
        /// Get latest contract completion date meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="d22Dates">The D22 dates.</param>
        [Theory]
        [InlineData("2018-09-11", "2014-08-01", "2018-09-11", null, "2016-02-11", null, "2017-06-09")]
        [InlineData("2017-12-31", null, "2015-12-31", "2017-12-31", "2014-12-31", null, "2017-10-16", null)]
        [InlineData("2018-07-01", "2018-07-01", "2014-05-11", "2014-07-12")]
        [InlineData("2016-11-17", "2016-11-17", null)]
        public void GetLatestContractCompletionDateMeetsExpectation(string candidate, params string[] d22Dates)
        {
            // arrange
            var testDate = DateTime.Parse(candidate);

            var latestContractCandidates = Collection.Empty<DateTime?>();
            d22Dates.ForEach(x => latestContractCandidates.Add(GetNullableDate(x)));
            var expectedContractDate = latestContractCandidates.Max();

            var deliveries = Collection.Empty<ILearningDelivery>();
            for (int i = 0; i < latestContractCandidates.Count; i++)
            {
                var mockDelivery = new Mock<ILearningDelivery>();
                deliveries.Add(mockDelivery.Object);
            }

            var safeDeliveries = deliveries.AsSafeReadOnlyList();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(Moq.It.IsAny<ILearningDelivery>(), safeDeliveries))
                .ReturnsInOrder(latestContractCandidates);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new EmpStat_14Rule(handler.Object, ddRule22.Object, fcsData.Object);

            // act
            var result = sut.GetLatestContractCompletionDate(safeDeliveries);

            // assert
            handler.VerifyAll();
            ddRule22.VerifyAll();
            fcsData.VerifyAll();

            Assert.Equal(testDate, result);
        }

        /// <summary>
        /// Get qualifying aim meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="d22Dates">The D22 dates.</param>
        [Theory]
        [InlineData("2018-09-11", "2014-08-01", "2018-09-11", null, "2016-02-11", null, "2017-06-09")]
        [InlineData("2017-12-31", null, "2015-12-31", "2017-12-30", "2014-12-31", null, "2017-10-16", null)]
        [InlineData("2018-07-01", "2018-06-30", "2014-05-11", "2014-07-12")]
        [InlineData("2016-11-17", "2016-11-17", null)]
        public void GetQualifyingAimMeetsExpectation(string candidate, params string[] d22Dates)
        {
            // arrange
            var testDate = DateTime.Parse(candidate);

            var latestContractCandidates = Collection.Empty<DateTime?>();
            d22Dates.ForEach(x => latestContractCandidates.Add(GetNullableDate(x)));
            var expectedContractDate = latestContractCandidates.Max();

            var deliveries = Collection.Empty<ILearningDelivery>();
            for (int i = 0; i < latestContractCandidates.Count; i++)
            {
                var mockDelivery = new Mock<ILearningDelivery>();
                deliveries.Add(mockDelivery.Object);
            }

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.LearnAimRef)
                .Returns(TypeOfAim.References.ESFLearnerStartandAssessment);
            mockItem
                .SetupGet(x => x.CompStatus)
                .Returns(CompletionState.HasCompleted);
            mockItem
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.EuropeanSocialFund);
            mockItem
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            deliveries.Add(mockItem.Object);

            var safeDeliveries = deliveries.AsSafeReadOnlyList();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new EmpStat_14Rule(handler.Object, ddRule22.Object, fcsData.Object);

            // act
            var result = sut.GetQualifyingdAim(safeDeliveries, testDate);

            // assert
            handler.VerifyAll();
            ddRule22.VerifyAll();
            fcsData.VerifyAll();

            Assert.Equal(mockItem.Object, result);
        }

        /// <summary>
        /// Get eligible employment status with null delivery meets expectation.
        /// </summary>
        [Fact]
        public void GetEligibleEmploymentStatusWithNullDeliveryMeetsExpectation()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleEmploymentStatusFor(null))
                .Returns((IEsfEligibilityRuleEmploymentStatus)null);

            var sut = new EmpStat_14Rule(handler.Object, ddRule22.Object, fcsData.Object);

            // act
            var result = sut.GetEligibleEmploymentStatus(null);

            // assert
            handler.VerifyAll();
            ddRule22.VerifyAll();
            fcsData.VerifyAll();

            Assert.Null(result);
        }

        [Theory]
        [InlineData("2018-09-11", "2014-08-01", "2018-09-11", "2016-02-11", "2017-06-09")]
        [InlineData("2017-12-31", "2015-12-31", "2017-12-30", "2014-12-31", "2017-10-16")]
        [InlineData("2018-07-01", "2018-06-30", "2014-05-11", "2014-07-12")]
        [InlineData("2016-11-17", "2016-11-17")]
        public void GetClosestEmploymentMeetsExpectation(string candidate, params string[] d22Dates)
        {
            // arrange
            var testDate = DateTime.Parse(candidate);

            var statusCandidates = Collection.Empty<DateTime>();
            d22Dates.ForEach(x => statusCandidates.Add(DateTime.Parse(x)));
            var expectedDate = statusCandidates.Max();

            var employmentCandidates = Collection.Empty<ILearnerEmploymentStatus>();
            statusCandidates
                .ForEach(x =>
                {
                    var mockItem = new Mock<ILearnerEmploymentStatus>();
                    mockItem.SetupGet(y => y.DateEmpStatApp).Returns(x);
                    employmentCandidates.Add(mockItem.Object);
                });

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new EmpStat_14Rule(handler.Object, ddRule22.Object, fcsData.Object);

            // act
            var result = sut.GetClosestEmployment(employmentCandidates.AsSafeReadOnlyList(), testDate);

            // assert
            handler.VerifyAll();
            ddRule22.VerifyAll();
            fcsData.VerifyAll();

            Assert.Equal(expectedDate, result.DateEmpStatApp);
        }

        /// <summary>
        /// Has a qualifying employment status meets expectation
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="eligibility">The eligibility.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment, TypeOfEmploymentStatus.InPaidEmployment, true)]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment, TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, false)]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment, TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, false)]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment, TypeOfEmploymentStatus.NotKnownProvided, false)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, false)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, TypeOfEmploymentStatus.NotKnownProvided, false)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, TypeOfEmploymentStatus.NotKnownProvided, false)]
        public void HasAQualifyingEmploymentStatusMeetsExpectation(int status, int eligibility, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(x => x.EmpStat)
                .Returns(status);

            var mockEligibility = new Mock<IEsfEligibilityRuleEmploymentStatus>();
            mockEligibility
                .SetupGet(x => x.Code)
                .Returns(eligibility);

            // act
            var result = sut.HasAQualifyingEmploymentStatus(mockStatus.Object, mockEligibility.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="d22Dates">The D22 dates.</param>
        [Theory]
        [InlineData("2018-09-11", "2014-08-01", "2018-09-11", null, "2016-02-11", null, "2017-06-09")]
        [InlineData("2017-12-31", null, "2015-12-31", "2017-12-31", "2017-12-30", "2014-12-31", null, "2017-10-16", null)]
        [InlineData("2018-07-01", "2018-06-30", "2018-07-01", "2014-05-11", "2014-07-12")]
        [InlineData("2016-11-17", "2016-11-17", null)]
        public void InvalidItemRaisesValidationMessage(string candidate, params string[] d22Dates)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string conRefNumber = "test-Con-Ref";

            var testDate = DateTime.Parse(candidate);

            var contractCandidates = Collection.Empty<DateTime?>();
            d22Dates.ForEach(x => contractCandidates.Add(GetNullableDate(x)));

            var deliveries = Collection.Empty<ILearningDelivery>();
            for (int i = 0; i < contractCandidates.Count - 1; i++)
            {
                var mockDelivery = new Mock<ILearningDelivery>();
                deliveries.Add(mockDelivery.Object);
            }

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.LearnAimRef)
                .Returns(TypeOfAim.References.ESFLearnerStartandAssessment);
            mockItem
                .SetupGet(x => x.CompStatus)
                .Returns(CompletionState.HasCompleted);
            mockItem
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.EuropeanSocialFund);
            mockItem
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            mockItem
                .SetupGet(x => x.ConRefNumber)
                .Returns(conRefNumber);

            deliveries.Add(mockItem.Object);

            var safeDeliveries = deliveries.AsSafeReadOnlyList();

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(testDate);
            mockStatus
                .SetupGet(y => y.EmpStat)
                .Returns(TypeOfEmploymentStatus.InPaidEmployment);

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(safeDeliveries);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == EmpStat_14Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == EmpStat_14Rule.MessagePropertyName),
                    TypeOfEmploymentStatus.InPaidEmployment))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.ConRefNumber),
                    conRefNumber))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.LearnStartDate),
                    testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(Moq.It.IsAny<ILearningDelivery>(), safeDeliveries))
                .ReturnsInOrder(contractCandidates);

            var eligibubble = new Mock<IEsfEligibilityRuleEmploymentStatus>();
            eligibubble
                .SetupGet(x => x.Code)
                .Returns(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleEmploymentStatusFor(conRefNumber))
                .Returns(eligibubble.Object);

            var sut = new EmpStat_14Rule(handler.Object, ddRule22.Object, fcsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule22.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="d22Dates">The D22 dates.</param>
        [Theory]
        [InlineData("2018-09-11", "2014-08-01", "2018-09-11", null, "2016-02-11", null, "2017-06-09")]
        [InlineData("2017-12-31", null, "2015-12-31", "2017-12-31", "2017-12-30", "2014-12-31", null, "2017-10-16", null)]
        [InlineData("2018-07-01", "2018-06-30", "2018-07-01", "2014-05-11", "2014-07-12")]
        [InlineData("2016-11-17", "2016-11-17", null)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, params string[] d22Dates)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string conRefNumber = "test-Con-Ref";

            var testDate = DateTime.Parse(candidate);

            var contractCandidates = Collection.Empty<DateTime?>();
            d22Dates.ForEach(x => contractCandidates.Add(GetNullableDate(x)));

            var deliveries = Collection.Empty<ILearningDelivery>();
            for (int i = 0; i < contractCandidates.Count - 1; i++)
            {
                var mockDelivery = new Mock<ILearningDelivery>();
                deliveries.Add(mockDelivery.Object);
            }

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.LearnAimRef)
                .Returns(TypeOfAim.References.ESFLearnerStartandAssessment);
            mockItem
                .SetupGet(x => x.CompStatus)
                .Returns(CompletionState.HasCompleted);
            mockItem
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.EuropeanSocialFund);
            mockItem
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            mockItem
                .SetupGet(x => x.ConRefNumber)
                .Returns(conRefNumber);

            deliveries.Add(mockItem.Object);

            var safeDeliveries = deliveries.AsSafeReadOnlyList();

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(testDate);
            mockStatus
                .SetupGet(y => y.EmpStat)
                .Returns(TypeOfEmploymentStatus.InPaidEmployment);

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(safeDeliveries);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(Moq.It.IsAny<ILearningDelivery>(), safeDeliveries))
                .ReturnsInOrder(contractCandidates);

            var eligibubble = new Mock<IEsfEligibilityRuleEmploymentStatus>();
            eligibubble
                .SetupGet(x => x.Code)
                .Returns(TypeOfEmploymentStatus.InPaidEmployment);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleEmploymentStatusFor(conRefNumber))
                .Returns(eligibubble.Object);

            var sut = new EmpStat_14Rule(handler.Object, ddRule22.Object, fcsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule22.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Gets a nullable date.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>a nullable date time</returns>
        public DateTime? GetNullableDate(string candidate) =>
            Utility.It.Has(candidate) ? DateTime.Parse(candidate) : (DateTime?)null;

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public EmpStat_14Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            return new EmpStat_14Rule(handler.Object, ddRule22.Object, fcsData.Object);
        }
    }
}
