using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using ESFA.DC.ILR.ValidationService.Rules.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// </summary>
    public class ProgType_03RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new ProgType_03Rule(null));
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
            Assert.Equal("ProgType_03", result);
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
            Assert.Equal(ProgType_03Rule.Name, result);
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
        /// Condition met with null learning delivery returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(null);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met with learning delivery containing null prog type returns false.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveryContainingNullProgTypeReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met with learning deliveries containing invalid prog types returns false.
        /// </summary>
        /// <param name="progType">The programme type.</param>
        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(19)]
        [InlineData(26)]
        public void ConditionMetWithLearningDeliveriesContainingInvalidProgTypesReturnsFalse(int progType)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(progType);

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met with learning deliveries containing null prog types returns true.
        /// </summary>
        /// <param name="progType">The programme type.</param>
        [Theory]
        [InlineData(TypeOfProgramme.AdvancedLevelApprenticeship)]
        [InlineData(TypeOfProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfProgramme.HigherApprenticeshipLevel4)]
        [InlineData(TypeOfProgramme.HigherApprenticeshipLevel5)]
        [InlineData(TypeOfProgramme.HigherApprenticeshipLevel6)]
        [InlineData(TypeOfProgramme.HigherApprenticeshipLevel7Plus)]
        [InlineData(TypeOfProgramme.IntermediateLevelApprenticeship)]
        [InlineData(TypeOfProgramme.Traineeship)]
        public void ConditionMetWithLearningDeliveriesContainingValidProgTypesReturnsTrue(int progType)
        {
            // arrange
            var sut = NewRule();
            var deliveries = Collection.Empty<ILearningDelivery>();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(progType);

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="progTypes">The programme types.</param>
        [Theory]
        [InlineData(1, 4, 19, 26)]
        public void InvalidItemRaisesValidationMessage(params int[] progTypes)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var deliveries = Collection.Empty<ILearningDelivery>();
            progTypes.ForEach(x =>
            {
                var mockDelivery = new Mock<ILearningDelivery>();
                mockDelivery
                    .SetupGet(y => y.ProgTypeNullable)
                    .Returns(x);

                deliveries.Add(mockDelivery.Object);
            });

            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == ProgType_03Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));

            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == ProgType_03Rule.MessagePropertyName),
                    Moq.It.IsAny<ILearningDelivery>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new ProgType_03Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="progTypes">The programme types.</param>
        [Theory]
        [InlineData(TypeOfProgramme.AdvancedLevelApprenticeship, TypeOfProgramme.HigherApprenticeshipLevel4, TypeOfProgramme.HigherApprenticeshipLevel6)]
        [InlineData(TypeOfProgramme.HigherApprenticeshipLevel7Plus, TypeOfProgramme.HigherApprenticeshipLevel4)]
        [InlineData(TypeOfProgramme.IntermediateLevelApprenticeship, TypeOfProgramme.AdvancedLevelApprenticeship, TypeOfProgramme.HigherApprenticeshipLevel6, TypeOfProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfProgramme.HigherApprenticeshipLevel4, TypeOfProgramme.HigherApprenticeshipLevel6)]
        [InlineData(TypeOfProgramme.AdvancedLevelApprenticeship, TypeOfProgramme.IntermediateLevelApprenticeship)]
        public void ValidItemDoesNotRaiseAValidationMessage(params int[] progTypes)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var deliveries = Collection.Empty<ILearningDelivery>();
            progTypes.ForEach(x =>
            {
                var mockDelivery = new Mock<ILearningDelivery>();
                mockDelivery
                    .SetupGet(y => y.ProgTypeNullable)
                    .Returns(x);

                deliveries.Add(mockDelivery.Object);
            });

            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new ProgType_03Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public ProgType_03Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new ProgType_03Rule(mock.Object);
        }
    }
}
