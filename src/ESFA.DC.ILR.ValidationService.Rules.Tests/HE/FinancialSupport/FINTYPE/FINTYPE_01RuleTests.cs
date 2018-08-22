using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.FinancialSupport.FINTYPE;
using ESFA.DC.ILR.ValidationService.Rules.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.FinancialSupport.FINTYPE
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// </summary>
    public class FINTYPE_01RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new FINTYPE_01Rule(null));
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
            Assert.Equal("FINTYPE_01", result);
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
            Assert.Equal(FINTYPE_01Rule.Name, result);
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
        /// Condition met with NULL finacial support returns true.
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
        /// Condition met with no finacial support returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithEmptyFinancialSupportReturnsTrue()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(Collection.EmptyAndReadOnly<ILearnerHEFinancialSupport>());

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met with valid financial support combinations returns true.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        [Theory]
        [InlineData(TypeOfFinancialSupport.Cash, TypeOfFinancialSupport.NearCash)]
        [InlineData(TypeOfFinancialSupport.Cash, TypeOfFinancialSupport.Other)]
        [InlineData(TypeOfFinancialSupport.Cash, TypeOfFinancialSupport.AccommodationDiscount)]
        [InlineData(TypeOfFinancialSupport.AccommodationDiscount, TypeOfFinancialSupport.NearCash)]
        [InlineData(TypeOfFinancialSupport.AccommodationDiscount, TypeOfFinancialSupport.Other)]
        [InlineData(TypeOfFinancialSupport.NearCash, TypeOfFinancialSupport.Other)]
        [InlineData(TypeOfFinancialSupport.Cash, TypeOfFinancialSupport.AccommodationDiscount, TypeOfFinancialSupport.NearCash, TypeOfFinancialSupport.Other)]
        public void ConditionMetWithValidFinancialSupportCombinationsReturnsTrue(params int[] candidates)
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(GetFinancialSupport(candidates));

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met with invalid financial support combinations returns false.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        [Theory]
        [InlineData(TypeOfFinancialSupport.Cash, 51)]
        [InlineData(16, TypeOfFinancialSupport.Other)]
        [InlineData(TypeOfFinancialSupport.AccommodationDiscount, 9)]
        [InlineData(0, TypeOfFinancialSupport.AccommodationDiscount, TypeOfFinancialSupport.NearCash)]
        [InlineData(TypeOfFinancialSupport.NearCash, 12, TypeOfFinancialSupport.AccommodationDiscount, TypeOfFinancialSupport.Other)]
        public void ConditionMetWithInvalidFinancialSupportCombinationsReturnsFalse(params int[] candidates)
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(GetFinancialSupport(candidates));

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        [Theory]
        [InlineData(TypeOfFinancialSupport.Cash, TypeOfFinancialSupport.NearCash)]
        [InlineData(TypeOfFinancialSupport.Cash, TypeOfFinancialSupport.Other)]
        [InlineData(TypeOfFinancialSupport.Cash, TypeOfFinancialSupport.AccommodationDiscount)]
        [InlineData(TypeOfFinancialSupport.AccommodationDiscount, TypeOfFinancialSupport.NearCash)]
        [InlineData(TypeOfFinancialSupport.AccommodationDiscount, TypeOfFinancialSupport.Other)]
        [InlineData(TypeOfFinancialSupport.NearCash, TypeOfFinancialSupport.Other)]
        [InlineData(TypeOfFinancialSupport.Cash, TypeOfFinancialSupport.AccommodationDiscount, TypeOfFinancialSupport.NearCash, TypeOfFinancialSupport.Other)]
        public void ValidItemDoesNotRaiseAValidationMessage(params int[] candidates)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE.SetupGet(x => x.LearnerHEFinancialSupports).Returns(GetFinancialSupport(candidates));
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            var mockDeliveryHE = new Mock<ILearningDeliveryHE>();

            mockDelivery.SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockDeliveryHE.Object);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries.AsSafeReadOnlyList());

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new FINTYPE_01Rule(mockHandler.Object);

            // act
            sut.Validate(mock.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        [Theory]
        [InlineData(TypeOfFinancialSupport.Cash, 51)]
        [InlineData(16, TypeOfFinancialSupport.Other)]
        [InlineData(TypeOfFinancialSupport.AccommodationDiscount, 9)]
        [InlineData(0, TypeOfFinancialSupport.AccommodationDiscount, TypeOfFinancialSupport.NearCash)]
        [InlineData(TypeOfFinancialSupport.NearCash, 12, TypeOfFinancialSupport.AccommodationDiscount, TypeOfFinancialSupport.Other)]
        public void InvalidItemRaisesValidationMessage(params int[] candidates)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE.SetupGet(x => x.LearnerHEFinancialSupports).Returns(GetFinancialSupport(candidates));
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = Collection.Empty<ILearningDelivery>();
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries.AsSafeReadOnlyList());

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == FINTYPE_01Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                null,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == FINTYPE_01Rule.MessagePropertyName),
                    Moq.It.IsAny<IReadOnlyCollection<ILearnerHEFinancialSupport>>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new FINTYPE_01Rule(mockHandler.Object);

            // act
            sut.Validate(mock.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Gets the financial support.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <returns>a collection of mocks built from the candidate "Fin Types"</returns>
        public IReadOnlyCollection<ILearnerHEFinancialSupport> GetFinancialSupport(int[] candidates)
        {
            var collection = Collection.Empty<ILearnerHEFinancialSupport>();

            candidates.ForEach(x =>
            {
                var mock = new Mock<ILearnerHEFinancialSupport>();
                mock.SetupGet(y => y.FINTYPE).Returns(x);
                collection.Add(mock.Object);
            });

            return collection.AsSafeReadOnlyList();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public FINTYPE_01Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new FINTYPE_01Rule(mock.Object);
        }
    }
}
