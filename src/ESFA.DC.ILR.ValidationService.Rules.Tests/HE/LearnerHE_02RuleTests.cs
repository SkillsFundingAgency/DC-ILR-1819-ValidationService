using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE;
using ESFA.DC.ILR.ValidationService.Rules.Utility;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE
{
    public class LearnerHE_02RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new LearnerHE_02Rule(null));
        }

        /// <summary>
        /// Rule name 1, matches a literal.
        /// </summary>
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act/assert
            sut.RuleName.Should().Be("LearnerHE_02");
        }

        /// <summary>
        /// Rule name 2, matches the constant.
        /// </summary>
        [Fact]
        public void RuleName2()
        {
            // arrange
            var sut = NewRule();

            // act/assert
            sut.RuleName.Should().Be(RuleNameConstants.LearnerHE_02);
        }

        /// <summary>
        /// Rule name 3 test, account for potential false positives.
        /// </summary>
        [Fact]
        public void RuleName3()
        {
            // arrange
            var sut = NewRule();

            // act/assert
            sut.RuleName.Should().NotBe(RuleNameConstants.LearnFAMType_16);
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
        /// Condition met with null learning HE returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithNullLearningHEReturnsTrue()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(null, null);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met with null learning deliveries returns false.
        /// </summary>
        [Fact]
        public void ConditionMetWithNullLearningDeliveriesReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mock = new Mock<ILearnerHE>();

            // act
            var result = sut.ConditionMet(mock.Object, null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met with no learning deliveries returns false.
        /// </summary>
        [Fact]
        public void ConditionMetWithNoLearningDeliveriesReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mock = new Mock<ILearnerHE>();
            var learningDeliveries = Collection.EmptyAndReadOnly<ILearningDelivery>();

            // act
            var result = sut.ConditionMet(mock.Object, learningDeliveries);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met with learning deliveries and no HE match returns false.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveriesAndNoHEMatchReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mock = new Mock<ILearnerHE>();

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            // act
            var result = sut.ConditionMet(mock.Object, deliveries.AsSafeReadOnlyList());

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met with learning deliveries and HE match returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveriesAndHEMatchReturnsTrue()
        {
            // arrange
            var sut = NewRule();
            var mock = new Mock<ILearnerHE>();

            var mockDelivery = new Mock<ILearningDelivery>();
            var mockDeliveryHE = new Mock<ILearningDeliveryHE>();

            mockDelivery.SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockDeliveryHE.Object);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            // act
            var result = sut.ConditionMet(mock.Object, deliveries.AsSafeReadOnlyList());

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met with null HE and learning deliveries returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithNullHEAndLearningDeliveriesReturnsTrue()
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            var mockDeliveryHE = new Mock<ILearningDeliveryHE>();

            mockDelivery.SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockDeliveryHE.Object);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            // act
            var result = sut.ConditionMet(null, deliveries.AsSafeReadOnlyList());

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = Collection.Empty<ILearningDelivery>();
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries.AsSafeReadOnlyList());

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == RuleNameConstants.LearnerHE_02),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                null,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));

            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "learnerHE"),
                    Moq.It.Is<object>(y => y == mockHE.Object)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new LearnerHE_02Rule(mockHandler.Object);

            // act
            sut.Validate(mock.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseAValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            var mockDeliveryHE = new Mock<ILearningDeliveryHE>();

            mockDelivery.SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockDeliveryHE.Object);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries.AsSafeReadOnlyList());

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new LearnerHE_02Rule(mockHandler.Object);

            // act
            sut.Validate(mock.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnerHE_02Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new LearnerHE_02Rule(mock.Object);
        }
    }
}
