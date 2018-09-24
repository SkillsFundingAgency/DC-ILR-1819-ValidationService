﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinType
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// </summary>
    public class AFinType_13RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new AFinType_13Rule(null));
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
            Assert.Equal("AFinType_13", result);
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
            Assert.Equal(AFinType_13Rule.Name, result);
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
            var result = sut.ConditionMet(null, null);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met with learning delivery and null financial record returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveryAndNullFinancialRecordReturnsTrue()
        {
            // arrange
            var sut = NewRule();
            var mock = new Mock<ILearningDelivery>();

            // act
            var result = sut.ConditionMet(mock.Object, null);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met with learning delivery and financial record with null date returns false.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveryAndFinancialRecordWithNullDateReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            var mockFinRec = new Mock<IAppFinRecord>();

            // act
            var result = sut.ConditionMet(mockDelivery.Object, mockFinRec.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met with learning delivery and matching financial record returns true.
        /// </summary>
        /// <param name="learnDate">The learn date.</param>
        /// <param name="finDate">The fin date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2016-04-01", "2016-04-01", true)]
        [InlineData("2016-04-01", "2016-04-02", false)]
        [InlineData("2016-04-01", "2016-03-31", false)]
        [InlineData("2016-05-01", "2016-05-01", true)]
        [InlineData("2016-04-30", "2016-05-01", false)]
        [InlineData("2016-05-02", "2016-05-01", false)]
        public void ConditionMetWithLearningDeliveryAndFinancialRecordReturnsExpectation(string learnDate, string finDate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse(learnDate));

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinDate)
                .Returns(DateTime.Parse(finDate));

            // act
            var result = sut.ConditionMet(mockDelivery.Object, mockFinRec.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Validates with null apprenctce financial records does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidateWithNullAppFinRecordsDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.ApprenticeshipsFrom1May2017);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == AFinType_13Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == AFinType_13Rule.MessagePropertyName),
                    Moq.It.Is<object>(y => y == mockDelivery.Object)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new AFinType_13Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Validate with empty apprenctce financial records does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidateWithEmptyAppFinRecordsDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.ApprenticeshipsFrom1May2017);

            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(Collection.EmptyAndReadOnly<IAppFinRecord>());

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == AFinType_13Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == AFinType_13Rule.MessagePropertyName),
                    Moq.It.Is<object>(y => y == mockDelivery.Object)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new AFinType_13Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="learnDate">The learn date.</param>
        /// <param name="finDate">The fin date.</param>
        [Theory]
        [InlineData("2016-04-01", "2016-04-02")]
        [InlineData("2016-04-01", "2016-03-31")]
        [InlineData("2016-04-30", "2016-05-01")]
        [InlineData("2016-05-02", "2016-05-01")]
        public void InvalidItemRaisesValidationMessage(string learnDate, string finDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinType)
                .Returns(ApprenticeshipFinanicalRecord.Types.TotalNegotiatedPrice);
            mockFinRec
                .SetupGet(x => x.AFinDate)
                .Returns(DateTime.Parse(finDate));

            var records = Collection.Empty<IAppFinRecord>();
            records.Add(mockFinRec.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.ApprenticeshipsFrom1May2017);
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse(learnDate));
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records.AsSafeReadOnlyList());

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == AFinType_13Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == AFinType_13Rule.MessagePropertyName),
                    Moq.It.Is<object>(y => y == mockDelivery.Object)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new AFinType_13Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="learnDate">The learn date.</param>
        /// <param name="finDate">The fin date.</param>
        [Theory]
        [InlineData("2016-04-01", "2016-04-01")]
        [InlineData("2016-05-01", "2016-05-01")]
        public void ValidItemDoesNotRaiseAValidationMessage(string learnDate, string finDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinType)
                .Returns(ApprenticeshipFinanicalRecord.Types.TotalNegotiatedPrice);
            mockFinRec
                .SetupGet(x => x.AFinDate)
                .Returns(DateTime.Parse(finDate));

            var records = Collection.Empty<IAppFinRecord>();
            records.Add(mockFinRec.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.ApprenticeshipsFrom1May2017);
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse(learnDate));
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records.AsSafeReadOnlyList());

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new AFinType_13Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public AFinType_13Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new AFinType_13Rule(mock.Object);
        }
    }
}
