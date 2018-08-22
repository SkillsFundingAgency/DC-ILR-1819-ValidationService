using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM;
using ESFA.DC.ILR.ValidationService.Rules.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.TTACCOM
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// </summary>
    public class TTACCOM_01RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new TTACCOM_01Rule(null));
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
            Assert.Equal("TTACCOM_01", result);
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
            Assert.Equal(TTACCOM_01Rule.Name, result);
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
        /// Condition met with null TTAccom returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithNullTTAccomReturnsTrue()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(null);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met with out of range TTAccom returns false.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(TermTimeAccommodation.InstitutionMaintainedProperty - 1)] // 0
        [InlineData(TermTimeAccommodation.PrivateSectorHalls + 1)] // 10
        public void ConditionMetWithOutOfRangeTTAccomReturnsFalse(int candidate)
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(candidate);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met with valid TTAccom code returns true.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(TermTimeAccommodation.InstitutionMaintainedProperty)]
        [InlineData(TermTimeAccommodation.NotInAttendanceAtTheInstitution)]
        [InlineData(TermTimeAccommodation.NotKnown)]
        [InlineData(TermTimeAccommodation.Other)]
        [InlineData(TermTimeAccommodation.OtherRentedAccommodation)]
        [InlineData(TermTimeAccommodation.OwnResidence)]
        [InlineData(TermTimeAccommodation.ParentaOrGuardianHome)]
        [InlineData(TermTimeAccommodation.PrivateSectorHalls)]
        public void ConditionMetWithLearningDeliveriesAndHEMatchReturnsTrue(int candidate)
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(candidate);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(TermTimeAccommodation.InstitutionMaintainedProperty - 1)] // 0
        [InlineData(TermTimeAccommodation.PrivateSectorHalls + 1)] // 10
        [InlineData(3)] // TermTimeAccommodation.OwnHome
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE.SetupGet(x => x.TTACCOMNullable).Returns(candidate);
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = Collection.Empty<ILearningDelivery>();
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries.AsSafeReadOnlyList());

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == TTACCOM_01Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                null,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == TTACCOM_01Rule.MessagePropertyName),
                    Moq.It.Is<int>(y => y == candidate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new TTACCOM_01Rule(mockHandler.Object);

            // act
            sut.Validate(mock.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(null)]
        [InlineData(TermTimeAccommodation.InstitutionMaintainedProperty)]
        [InlineData(TermTimeAccommodation.NotInAttendanceAtTheInstitution)]
        [InlineData(TermTimeAccommodation.NotKnown)]
        [InlineData(TermTimeAccommodation.Other)]
        [InlineData(TermTimeAccommodation.OtherRentedAccommodation)]
        [InlineData(TermTimeAccommodation.OwnResidence)]
        [InlineData(TermTimeAccommodation.ParentaOrGuardianHome)]
        [InlineData(TermTimeAccommodation.PrivateSectorHalls)]
        public void ValidItemDoesNotRaiseAValidationMessage(int? candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE.SetupGet(x => x.TTACCOMNullable).Returns(candidate);
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            var mockDeliveryHE = new Mock<ILearningDeliveryHE>();

            mockDelivery.SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockDeliveryHE.Object);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries.AsSafeReadOnlyList());

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new TTACCOM_01Rule(mockHandler.Object);

            // act
            sut.Validate(mock.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public TTACCOM_01Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new TTACCOM_01Rule(mock.Object);
        }
    }
}
