using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R06RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var provider = new Mock<IFileDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R06Rule(null, provider.Object));
        }

        /// <summary>
        /// New rule with null provider throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullProviderThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R06Rule(handler.Object, null));
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
            Assert.Equal("R06", result);
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
            Assert.Equal(R06Rule.Name, result);
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
        /// Get learners verifies ok.
        /// </summary>
        /// <param name="learnRN">The learn rn.</param>
        /// <param name="candidateCount">The candidate count.</param>
        [Theory]
        [InlineData("sldfkajwefo asjf", 3)]
        [InlineData("alwerkasvf as", 2)]
        [InlineData("zxc,vmnsdlih", 5)]
        [InlineData(",samvnasorgdhkn", 1)]
        public void GetLearnersVerifiesOK(string learnRN, int candidateCount)
        {
            // arrange
            var collection = Collection.Empty<ILearner>();
            for (int i = 0; i < candidateCount; i++)
            {
                collection.Add(new Mock<ILearner>().Object);
            }

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IFileDataService>(MockBehavior.Strict);

            // we can no longer check the learn ref number gets sent into here as the mock doesn't support it
            provider
                .Setup(x => x.GetLearners(Moq.It.IsAny<Func<ILearner, bool>>()))
                .Returns(collection.AsSafeReadOnlyList());

            var sut = new R06Rule(handler.Object, provider.Object);

            // act
            var result = sut.GetLearners(learnRN);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
            Assert.Equal(candidateCount, result.Count);
        }

        /// <summary>
        /// Determines whether [has unique reference number meets expectation] [the specified learn rn].
        /// </summary>
        /// <param name="learnRN">The learn rn.</param>
        /// <param name="candidateCount">The candidate count.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("sldfkajwefo asjf", 3, false)]
        [InlineData("alwerkasvf as", 2, false)]
        [InlineData("zxc,vmnsdlih", 5, false)]
        [InlineData(",samvnasorgdhkn", 1, true)]
        public void HasUniqueReferenceNumberMeetsExpectation(string learnRN, int candidateCount, bool expectation)
        {
            // arrange
            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRN);

            var collection = Collection.Empty<ILearner>();

            for (int i = 0; i < candidateCount; i++)
            {
                collection.Add(new Mock<ILearner>().Object);
            }

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IFileDataService>(MockBehavior.Strict);

            // we can no longer check the learn ref number gets sent into here as the mock doesn't support it
            provider
                .Setup(x => x.GetLearners(Moq.It.IsAny<Func<ILearner, bool>>()))
                .Returns(collection.AsSafeReadOnlyList());

            var sut = new R06Rule(handler.Object, provider.Object);

            // act
            var result = sut.HasUniqueReferenceNumber(learner.Object);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="learnRN">The learn rn.</param>
        /// <param name="candidateCount">The candidate count.</param>
        [Theory]
        [InlineData("sldfkajwefo asjf", 3)]
        [InlineData("alwerkasvf as", 2)]
        [InlineData("zxc,vmnsdlih", 5)]
        public void InvalidItemRaisesValidationMessage(string learnRN, int candidateCount)
        {
            // arrange
            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRN);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == R06Rule.Name),
                    Moq.It.Is<string>(y => y == learnRN),
                    null,
                    null));
            var provider = new Mock<IFileDataService>(MockBehavior.Strict);

            var collection = Collection.Empty<ILearner>();
            for (int i = 0; i < candidateCount; i++)
            {
                collection.Add(new Mock<ILearner>().Object);
            }

            // we can no longer check the learn ref number gets sent into here as the mock doesn't support it
            provider
                .Setup(x => x.GetLearners(Moq.It.IsAny<Func<ILearner, bool>>()))
                .Returns(collection.AsSafeReadOnlyList());

            var sut = new R06Rule(handler.Object, provider.Object);

            // act
            sut.Validate(mockItem.Object);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="learnRN">The learn rn.</param>
        /// <param name="candidateCount">The candidate count.</param>
        [Theory]
        [InlineData(",samvnasorgdhkn", 1)]
        public void ValidItemDoesNotRaiseValidationMessage(string learnRN, int candidateCount)
        {
            // arrange
            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRN);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IFileDataService>(MockBehavior.Strict);

            var collection = Collection.Empty<ILearner>();
            for (int i = 0; i < candidateCount; i++)
            {
                collection.Add(new Mock<ILearner>().Object);
            }

            // we can no longer check the learn ref number gets sent into here as the mock doesn't support it
            provider
                .Setup(x => x.GetLearners(Moq.It.IsAny<Func<ILearner, bool>>()))
                .Returns(collection.AsSafeReadOnlyList());

            var sut = new R06Rule(handler.Object, provider.Object);

            // act
            sut.Validate(mockItem.Object);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public R06Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IFileDataService>(MockBehavior.Strict);

            return new R06Rule(handler.Object, provider.Object);
        }
    }
}
