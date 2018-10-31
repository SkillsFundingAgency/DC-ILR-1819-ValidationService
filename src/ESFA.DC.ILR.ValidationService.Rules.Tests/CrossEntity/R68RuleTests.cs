using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R68RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new R68Rule(null));
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
            Assert.Equal("R68", result);
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
            Assert.Equal(R68Rule.Name, result);
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
        /// Has wrong code cardinality meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        /// <param name="testCodes">The test codes.</param>
        [Theory]
        [InlineData(false, 1, 2, 3, 4, 5, 6)]
        [InlineData(true, 1, 2, 3, 1, 4, 5, 6)]
        [InlineData(false, null, 1, 2, 3, 1, 4, 5, 6)]
        [InlineData(false, null, 1, 2, 3, null, 4, 5, 6)]
        [InlineData(true, 2, 2, 3, 2, 4, 5, 6)]
        [InlineData(true, 3, 2, 3, 2, 4, 3, 5, 6)]
        public void HasWrongCodeCardinalityMeetsExpectation(bool expectation, params int?[] testCodes)
        {
            // arrange
            var sut = NewRule();

            var deliveries = Collection.Empty<ILearningDelivery>();
            testCodes.ForEach(x =>
            {
                var mockDel = new Mock<ILearningDelivery>();
                mockDel
                    .SetupGet(y => y.StdCodeNullable)
                    .Returns(x);

                deliveries.Add(mockDel.Object);
            });

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.StdCodeNullable)
                .Returns(testCodes[0]);

            // act
            var result = sut.HasWrongCodeCardinality(mockItem.Object, deliveries.AsSafeReadOnlyList());

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Get candidate codes meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="testCodes">The test codes.</param>
        [Theory]
        [InlineData(0, 1, 2, 3, 4, 5, 6)]
        [InlineData(1, 1, 2, 3, 1, 4, 5, 6)]
        [InlineData(1, null, 1, 2, 3, 1, 4, 5, 6)]
        [InlineData(0, null, 1, 2, 3, null, 4, 5, 6)]
        [InlineData(1, 2, 2, 3, 2, 4, 5, 6)]
        [InlineData(2, 3, 2, 3, 2, 4, 3, 5, 6)]
        [InlineData(3, 3, 2, 3, 2, 4, 3, 5, 6, 4)]
        public void GetCandidateCodesMeetsExpectation(int expectation, params int?[] testCodes)
        {
            // arrange
            var sut = NewRule();

            var deliveries = Collection.Empty<ILearningDelivery>();
            testCodes.ForEach(x =>
            {
                var mockDel = new Mock<ILearningDelivery>();
                mockDel
                    .SetupGet(y => y.StdCodeNullable)
                    .Returns(x);

                deliveries.Add(mockDel.Object);
            });

            // act
            var result = sut.GetCandidateCodes(deliveries.AsSafeReadOnlyList());

            // assert
            Assert.Equal(expectation, result.Count());
        }

        /// <summary>
        /// Is qualifying item meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        /// <param name="testCodes">The test codes.</param>
        [Theory]
        [InlineData(null, false, 1, 2, 3, 4, 5, 6, 7)]
        [InlineData(1, true, 1, 2, 3)]
        [InlineData(7, false, 1, 2, 3, 4, 5, 6)]
        [InlineData(5, false, 1, 2, 3, 4)]
        [InlineData(5, true, 1, 2, 3, 4, 5, 6)]
        [InlineData(6, true, 1, 2, 3, 4, 5, 6, 7)]
        public void IsQualifyingItemMeetsExpectation(int? candidate, bool expectation, params int[] testCodes)
        {
            // arrange
            var sut = NewRule();

            var mockDel = new Mock<ILearningDelivery>();
            mockDel
                .SetupGet(y => y.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDel
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.ApprenticeshipStandard);
            mockDel
                .SetupGet(y => y.StdCodeNullable)
                .Returns(candidate);

            // act
            var result = sut.IsQualifyingItem(mockDel.Object, testCodes);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Get flattened records meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">The expectation.</param>
        /// <param name="testCodes">The test codes.</param>
        [Theory]
        [InlineData(1, 2, 1, 1, 2, 3)]
        [InlineData(3, 12, 1, 2, 2, 3, 4, 4, 5, 6)]
        [InlineData(5, 15, 1, 2, 3, 3, 3, 4)]
        [InlineData(2, 12, 1, 2, 2, 3, 4, 4, 5, 5, 6)]
        [InlineData(4, 16, 1, 2, 3, 3, 4, 5, 6, 6, 7)]
        public void GetFlattenedRecordsMeetsExpectation(int candidate, int expectation, params int[] testCodes)
        {
            // arrange
            var sut = NewRule();

            var collection = Collection.Empty<ILearningDelivery>();
            testCodes.ForEach(x =>
            {
                var records = Collection.Empty<IAppFinRecord>();
                for (int i = 0; i < candidate; i++)
                {
                    records.Add(new Mock<IAppFinRecord>().Object);
                }

                var mockDel = new Mock<ILearningDelivery>();
                mockDel
                    .SetupGet(y => y.AimType)
                    .Returns(TypeOfAim.ProgrammeAim);
                mockDel
                    .SetupGet(y => y.ProgTypeNullable)
                    .Returns(TypeOfLearningProgramme.ApprenticeshipStandard);
                mockDel
                    .SetupGet(y => y.StdCodeNullable)
                    .Returns(x);
                mockDel
                    .SetupGet(y => y.AppFinRecords)
                    .Returns(records.AsSafeReadOnlyList());

                collection.Add(mockDel.Object);
            });

            // act
            var codes = sut.GetCandidateCodes(collection.AsSafeReadOnlyList());
            var result = sut.GetFlattenedRecords(collection.AsSafeReadOnlyList(), codes);

            // assert
            Assert.Equal(expectation, result.Count());
        }

        [Theory]
        [InlineData(1, true, 1, 2, 3)]
        [InlineData(5, true, 1, 2, 3, 4, 5, 6)]
        [InlineData(5, true, 1, 2, 2, 3, 3, 4, 5, 6)]
        [InlineData(0, false, 1, 2, 3, 4)]
        [InlineData(0, false, 1, 1, 1, 2, 3, 4)]
        [InlineData(7, false, 1, 2, 3, 4, 5, 6)]
        [InlineData(7, false, 1, 2, 3, 3, 4, 5, 6)]
        [InlineData(4, true, 1, 2, 3, 4, 5, 6, 7)]
        [InlineData(4, true, 1, 2, 3, 4, 4, 5, 6, 7)]
        public void HasWrongRecordCardinalityMeetsExpectation(int candidate, bool expectation, params int[] testCodes)
        {
            // arrange
            var sut = NewRule();

            var records = Collection.Empty<IAppFinRecord>();
            testCodes.ForEach(code =>
            {
                var rec1 = new Mock<IAppFinRecord>();
                rec1.SetupGet(x => x.AFinCode).Returns(code);
                rec1.SetupGet(x => x.AFinDate).Returns(DateTime.Today);
                rec1.SetupGet(x => x.AFinType).Returns("TestType");

                records.Add(rec1.Object);
            });

            var rec2 = new Mock<IAppFinRecord>();
            rec2.SetupGet(x => x.AFinCode).Returns(candidate);
            rec2.SetupGet(x => x.AFinDate).Returns(DateTime.Today);
            rec2.SetupGet(x => x.AFinType).Returns("TestType");

            records.Add(rec2.Object);

            // act
            var result = sut.HasWrongRecordCardinality(rec2.Object, records.AsSafeReadOnlyList());

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="deliveryCount">The delivery count.</param>
        /// <param name="testCodes">The test codes.</param>
        [Theory]
        [InlineData(2, 1, 1, 3, 3)]
        [InlineData(3, 1, 2, 1, 4, 4, 5, 5, 6)]
        [InlineData(3, 1, 1, 1, 2, 3, 4)]
        [InlineData(5, 1, 2, 2, 3, 3, 4, 5, 6)]
        [InlineData(4, 1, 1, 2, 3, 4)]
        [InlineData(5, 1, 2, 2, 2, 3, 4, 5, 6)]
        [InlineData(3, 6, 4, 6, 3, 4, 5, 6)]
        [InlineData(2, 5, 5, 3, 4, 5, 6, 7)]
        [InlineData(3, 2, 2, 3, 4, 4, 5, 6, 7)]
        public void InvalidItemRaisesValidationMessage(int deliveryCount, params int[] testCodes)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var deliveries = Collection.Empty<ILearningDelivery>();
            for (int i = 0; i < deliveryCount; i++)
            {
                var code = testCodes.ElementAt(i);

                var records = Collection.Empty<IAppFinRecord>();
                for (int j = deliveryCount; j < testCodes.Length; j++)
                {
                    var rec2 = new Mock<IAppFinRecord>();
                    rec2.SetupGet(x => x.AFinCode).Returns(testCodes.ElementAt(j));
                    rec2.SetupGet(x => x.AFinDate).Returns(DateTime.Today);
                    rec2.SetupGet(x => x.AFinType).Returns("TestType");

                    records.Add(rec2.Object);
                }

                var mockDel = new Mock<ILearningDelivery>();
                mockDel
                    .SetupGet(y => y.AimType)
                    .Returns(TypeOfAim.ProgrammeAim);
                mockDel
                    .SetupGet(y => y.ProgTypeNullable)
                    .Returns(TypeOfLearningProgramme.ApprenticeshipStandard);
                mockDel
                    .SetupGet(y => y.StdCodeNullable)
                    .Returns(code);
                mockDel
                    .SetupGet(y => y.AppFinRecords)
                    .Returns(records.AsSafeReadOnlyList());

                deliveries.Add(mockDel.Object);
            }

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockItem
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == R68Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                null,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.ProgType),
                    TypeOfLearningProgramme.ApprenticeshipStandard))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.AFinType),
                    Moq.It.Is<string>(y => y == "TestType")))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.AFinCode),
                    Moq.It.IsAny<int>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.AFinDate),
                    Moq.It.Is<DateTime>(y => y == DateTime.Today)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new R68Rule(handler.Object);

            // act
            sut.Validate(mockItem.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="deliveryCount">The delivery count.</param>
        /// <param name="testCodes">The test codes.</param>
        [Theory]
        [InlineData(2, 1, 2, 3, 3)]
        [InlineData(3, 1, 2, 3, 4, 4, 5, 5, 6)]
        [InlineData(3, 1, 2, 3, 2, 3, 4)]
        [InlineData(5, 1, 8, 9, 10, 11, 4, 5, 6)]
        [InlineData(4, 1, 8, 2, 3, 4)]
        [InlineData(5, 1, 2, 7, 8, 9, 4, 5, 6)]
        [InlineData(3, 6, 4, 5, 3, 4, 5, 6)]
        [InlineData(2, 5, 7, 3, 4, 5, 6, 7)]
        [InlineData(3, 2, 1, 3, 4, 4, 5, 6, 7)]
        public void ValidItemDoesNotRaiseValidationMessage(int deliveryCount, params int[] testCodes)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var deliveries = Collection.Empty<ILearningDelivery>();
            for (int i = 0; i < deliveryCount; i++)
            {
                var code = testCodes.ElementAt(i);

                var records = Collection.Empty<IAppFinRecord>();
                for (int j = deliveryCount; j < testCodes.Length; j++)
                {
                    var rec2 = new Mock<IAppFinRecord>();
                    rec2.SetupGet(x => x.AFinCode).Returns(testCodes.ElementAt(j));
                    rec2.SetupGet(x => x.AFinDate).Returns(DateTime.Today);
                    rec2.SetupGet(x => x.AFinType).Returns("TestType");

                    records.Add(rec2.Object);
                }

                var mockDel = new Mock<ILearningDelivery>();
                mockDel
                    .SetupGet(y => y.AimType)
                    .Returns(TypeOfAim.ProgrammeAim);
                mockDel
                    .SetupGet(y => y.ProgTypeNullable)
                    .Returns(TypeOfLearningProgramme.ApprenticeshipStandard);
                mockDel
                    .SetupGet(y => y.StdCodeNullable)
                    .Returns(code);
                mockDel
                    .SetupGet(y => y.AppFinRecords)
                    .Returns(records.AsSafeReadOnlyList());

                deliveries.Add(mockDel.Object);
            }

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockItem
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new R68Rule(handler.Object);

            // act
            sut.Validate(mockItem.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public R68Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new R68Rule(handler.Object);
        }
    }
}
