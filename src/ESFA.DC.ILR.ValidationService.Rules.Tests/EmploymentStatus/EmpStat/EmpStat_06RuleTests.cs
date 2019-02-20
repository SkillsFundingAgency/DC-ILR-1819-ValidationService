using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpId
{
    public class EmpStat_06RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_06Rule(null, commonOps.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_06Rule(handler.Object, null));
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
            Assert.Equal("EmpStat_06", result);
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
            Assert.Equal(RuleNameConstants.EmpStat_06, result);
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

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// First viable date meets expectation.
        /// </summary>
        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(DateTime.Parse("2013-08-01"), EmpStat_06Rule.FirstViableDate);
        }

        /// <summary>
        /// Last viable date meets expectation.
        /// </summary>
        [Fact]
        public void LastViableDateMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(DateTime.Parse("2014-07-31"), EmpStat_06Rule.LastViableDate);
        }

        /// <summary>
        /// Planned total qualifying hours meets expectation.
        /// </summary>
        [Fact]
        public void PlannedTotalQualifyingHoursMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(540, EmpStat_07Rule.PlannedTotalQualifyingHours);
        }

        /// <summary>
        /// Get learning hours total meets expectation.
        /// </summary>
        /// <param name="planned">The planned.</param>
        /// <param name="eep">The eep.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData(null, null, 0)]
        [InlineData(1, null, 1)]
        [InlineData(null, 1, 1)]
        [InlineData(1, 1, 2)]
        public void GetLearningHoursTotalMeetsExpectation(int? planned, int? eep, int expectation)
        {
            // arrange
            var mockItem = new Mock<ILearner>();
            mockItem.SetupGet(x => x.PlanLearnHoursNullable).Returns(planned);
            mockItem.SetupGet(x => x.PlanEEPHoursNullable).Returns(eep);

            var sut = NewRule();

            // act
            var result = sut.GetLearningHoursTotal(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is excluded meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsExcludedMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsTraineeship(mockItem.Object))
                .Returns(expectation);

            var sut = new EmpStat_06Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.IsExcluded(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has qualifying funding meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingFundingMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockItem.Object, 25, 82))
                .Returns(expectation);

            var sut = new EmpStat_06Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingFunding(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has qualifying start meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingStartMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingStart(mockItem.Object, DateTime.Parse("2013-08-01"), DateTime.Parse("2014-07-31")))
                .Returns(expectation);

            var sut = new EmpStat_06Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingStart(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has qualifying employment status returns true
        /// </summary>
        [Fact]
        public void HasQualifyingEmploymentStatusReturnsTrue()
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();

            // act
            var result = sut.HasQualifyingEmployment(mockItem.Object);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Has qualifying employment status with null returns false
        /// </summary>
        [Fact]
        public void HasQualifyingEmploymentStatusWithNullReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasQualifyingEmployment(null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse("2013-08-01");

            var status = new Mock<ILearnerEmploymentStatus>();

            var statii = new ILearnerEmploymentStatus[] { status.Object };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("EmpStat_06", LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("DateEmpStatApp", "(missing)"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("PlanLearnHours", null))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("PlanEEPHours", null))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.GetEmploymentStatusOn(testDate, statii))
                .Returns((ILearnerEmploymentStatus)null);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 25, 82))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.Parse("2013-08-01"), DateTime.Parse("2014-07-31")))
                .Returns(true);
            commonOps
                .Setup(x => x.IsTraineeship(mockDelivery.Object))
                .Returns(false);

            var sut = new EmpStat_06Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse("2013-08-01");

            var status = new Mock<ILearnerEmploymentStatus>();

            var statii = new ILearnerEmploymentStatus[] { status.Object };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.GetEmploymentStatusOn(testDate, statii))
                .Returns(status.Object);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 25, 82))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.Parse("2013-08-01"), DateTime.Parse("2014-07-31")))
                .Returns(true);
            commonOps
                .Setup(x => x.IsTraineeship(mockDelivery.Object))
                .Returns(false);

            var sut = new EmpStat_06Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public EmpStat_06Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new EmpStat_06Rule(handler.Object, commonOps.Object);
        }
    }
}