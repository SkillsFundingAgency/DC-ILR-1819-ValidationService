using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using Moq;
using System;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_28RuleTests
    {
        /// <summary>
        /// Determines whether [is adult funded unemployed with benefits with null learner throws].
        /// </summary>
        [Fact]
        public void IsAdultFundedUnemployedWithBenefitsWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.IsAdultFundedUnemployedWithBenefits(null));
        }

        /// <summary>
        /// Determines whether [is adult skills meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        public void IsAdultSkillsMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            // act
            var result = sut.IsAdultSkills(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// In receipt of employment support meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(EmploymentStatusMonitoring.EmployedForLessThan16HoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor16HoursOrMorePW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor16To19HoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor20HoursOrMorePW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor0To10HourPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor11To20HoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor21To30HoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor31PlusHoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedForUpTo3M, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor4To6M, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor7To12M, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedForMoreThan12M, false)]
        [InlineData(EmploymentStatusMonitoring.InFulltimeEducationOrTrainingPriorToEnrolment, false)]
        [InlineData(EmploymentStatusMonitoring.InReceiptOfAnotherStateBenefit, false)]
        [InlineData(EmploymentStatusMonitoring.InReceiptOfEmploymentAndSupportAllowance, true)]
        [InlineData(EmploymentStatusMonitoring.InReceiptOfJobSeekersAllowance, true)]
        [InlineData(EmploymentStatusMonitoring.InReceiptOfUniversalCredit, false)]
        [InlineData(EmploymentStatusMonitoring.SelfEmployed, false)]
        [InlineData(EmploymentStatusMonitoring.SmallEmployer, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedForLessThan6M, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedFor6To11M, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedFor12To23M, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedFor24To35M, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedFor36MPlus, false)]
        public void InReceiptOfEmploymentSupportMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            // act
            var result = sut.InReceiptOfEmploymentSupport(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        /// <summary>
        /// In receipt of credits meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(EmploymentStatusMonitoring.EmployedForLessThan16HoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor16HoursOrMorePW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor16To19HoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor20HoursOrMorePW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor0To10HourPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor11To20HoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor21To30HoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor31PlusHoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedForUpTo3M, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor4To6M, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor7To12M, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedForMoreThan12M, false)]
        [InlineData(EmploymentStatusMonitoring.InFulltimeEducationOrTrainingPriorToEnrolment, false)]
        [InlineData(EmploymentStatusMonitoring.InReceiptOfAnotherStateBenefit, true)]
        [InlineData(EmploymentStatusMonitoring.InReceiptOfEmploymentAndSupportAllowance, false)]
        [InlineData(EmploymentStatusMonitoring.InReceiptOfJobSeekersAllowance, false)]
        [InlineData(EmploymentStatusMonitoring.InReceiptOfUniversalCredit, true)]
        [InlineData(EmploymentStatusMonitoring.SelfEmployed, false)]
        [InlineData(EmploymentStatusMonitoring.SmallEmployer, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedForLessThan6M, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedFor6To11M, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedFor12To23M, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedFor24To35M, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedFor36MPlus, false)]
        public void InReceiptOfCreditsMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            // act
            var result = sut.InReceiptOfCredits(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        /// <summary>
        /// Determines whether [has valid employment status meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, true)]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment, true)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, true)]
        [InlineData(TypeOfEmploymentStatus.NotKnownProvided, true)]
        public void HasValidEmploymentStatusMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearnerEmploymentStatus>();
            mockDelivery
                .SetupGet(y => y.EmpStat)
                .Returns(candidate);

            // act
            var result = sut.HasValidEmploymentStatus(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Determines whether [is working short hours meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(EmploymentStatusMonitoring.EmployedForLessThan16HoursPW, true)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor16HoursOrMorePW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor16To19HoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor20HoursOrMorePW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor0To10HourPW, true)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor11To20HoursPW, true)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor21To30HoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor31PlusHoursPW, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedForUpTo3M, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor4To6M, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedFor7To12M, false)]
        [InlineData(EmploymentStatusMonitoring.EmployedForMoreThan12M, false)]
        [InlineData(EmploymentStatusMonitoring.InFulltimeEducationOrTrainingPriorToEnrolment, false)]
        [InlineData(EmploymentStatusMonitoring.InReceiptOfAnotherStateBenefit, false)]
        [InlineData(EmploymentStatusMonitoring.InReceiptOfEmploymentAndSupportAllowance, false)]
        [InlineData(EmploymentStatusMonitoring.InReceiptOfJobSeekersAllowance, false)]
        [InlineData(EmploymentStatusMonitoring.InReceiptOfUniversalCredit, false)]
        [InlineData(EmploymentStatusMonitoring.SelfEmployed, false)]
        [InlineData(EmploymentStatusMonitoring.SmallEmployer, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedForLessThan6M, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedFor6To11M, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedFor12To23M, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedFor24To35M, false)]
        [InlineData(EmploymentStatusMonitoring.UnemployedFor36MPlus, false)]
        public void IsWorkingShortHoursMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            // act
            var result = sut.IsWorkingShortHours(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        /// <summary>
        /// Determines whether [is employed meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, false)]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment, true)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, false)]
        [InlineData(TypeOfEmploymentStatus.NotKnownProvided, false)]
        public void IsEmployedMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearnerEmploymentStatus>();
            mockDelivery
                .SetupGet(y => y.EmpStat)
                .Returns(candidate);

            // act
            var result = sut.IsEmployed(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up derived data rule</returns>
        public DerivedData_28Rule NewRule()
        {
            return new DerivedData_28Rule();
        }
    }
}
