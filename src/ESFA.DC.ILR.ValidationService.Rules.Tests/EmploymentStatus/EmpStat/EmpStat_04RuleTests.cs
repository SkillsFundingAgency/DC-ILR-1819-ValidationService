using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_04RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var mockDDRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_04Rule(null, mockDDRule22.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 07 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule07Throws()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_04Rule(mockHandler.Object, null));
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
            Assert.Equal("EmpStat_04", result);
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
            Assert.Equal(EmpStat_04Rule.Name, result);
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

        [Theory]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment, false)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, false)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, false)]
        [InlineData(TypeOfEmploymentStatus.NotKnownProvided, true)]
        public void IsNotKnownOrProvidedMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(y => y.EmpStat)
                .Returns(candidate);

            // act
            var result = sut.IsNotKnownOrProvided(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasQualifyingCompletedContractWithNullDeliveriesReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearner>();

            // act
            var result = sut.HasQualifyingCompletedContract(mockItem.Object, DateTime.Today);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void HasQualifyingCompletedContractWithEmptyDeliveriesReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearningDeliveries)
                .Returns(Collection.EmptyAndReadOnly<ILearningDelivery>());

            // act
            var result = sut.HasQualifyingCompletedContract(mockItem.Object, DateTime.Today);

            // assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("2018-03-12", null, false)]
        [InlineData("2018-03-12", "2018-03-10", false)]
        [InlineData("2018-03-12", "2018-03-11", false)]
        [InlineData("2018-03-12", "2018-03-12", true)]
        [InlineData("2018-03-12", "2018-03-13", false)]
        public void HasQualifyingCompletedContractMeetsExpectation(string inDate, string outDate, bool expectation)
        {
            // arrange
            var empStatDate = DateTime.Parse(inDate);
            var learnStartDate = !string.IsNullOrWhiteSpace(outDate)
                ? DateTime.Parse(outDate)
                : (DateTime?)null;

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            mockDDRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(Moq.It.IsAny<ILearningDelivery>(), Moq.It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                .Returns(learnStartDate);

            var sut = new EmpStat_04Rule(handler.Object, mockDDRule22.Object);

            var deliveries = Collection.Empty<ILearningDelivery>();
            if (learnStartDate.HasValue)
            {
                var del = new Mock<ILearningDelivery>();
                del
                    .SetupGet(x => x.LearnStartDate)
                    .Returns(learnStartDate.Value);

                deliveries.Add(del.Object);
            }

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            // act
            var result = sut.HasQualifyingCompletedContract(mockItem.Object, empStatDate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Get latest learning start for esf contract returns expected date.
        /// </summary>
        /// <param name="outDate">The out date.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("2018-03-10")]
        [InlineData("2018-03-11")]
        [InlineData("2018-03-12")]
        [InlineData("2018-03-13")]
        public void GetLatestLearningStartForESFContractReturnsExpectedDate(string outDate)
        {
            // arrange
            var learnStartDate = !string.IsNullOrWhiteSpace(outDate)
                ? DateTime.Parse(outDate)
                : (DateTime?)null;

            var deliveries = Collection.EmptyAndReadOnly<ILearningDelivery>();
            var mockDel = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            mockDDRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(mockDel.Object, deliveries))
                .Returns(learnStartDate);

            var sut = new EmpStat_04Rule(handler.Object, mockDDRule22.Object);

            // act
            var result = sut.GetLatestLearningStartForESFContract(mockDel.Object, deliveries);

            // assert
            Assert.Equal(learnStartDate, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="outDate">The out date.</param>
        [Theory]
        [InlineData("2018-03-10")]
        [InlineData("2018-03-11")]
        [InlineData("2018-03-12")]
        [InlineData("2018-03-13")]
        public void InvalidItemRaisesValidationMessage(string outDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var learnStartDate = DateTime.Parse(outDate);

            var deliveries = Collection.Empty<ILearningDelivery>();
            var mockDel = new Mock<ILearningDelivery>();
            mockDel
                .SetupGet(x => x.LearnStartDate)
                .Returns(learnStartDate);

            deliveries.Add(mockDel.Object);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(learnStartDate);
            mockStatus
                .SetupGet(y => y.EmpStat)
                .Returns(TypeOfEmploymentStatus.NotKnownProvided);

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == EmpStat_04Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == EmpStat_04Rule.MessagePropertyName),
                    TypeOfEmploymentStatus.NotKnownProvided))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockDDRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            mockDDRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(mockDel.Object, Moq.It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                .Returns(learnStartDate);

            var sut = new EmpStat_04Rule(handler.Object, mockDDRule22.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            mockDDRule22.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="empStat">The emp stat.</param>
        [Theory]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable)]
        public void ValidItemDoesNotRaiseValidationMessage(int empStat)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.EmpStat)
                .Returns(empStat);

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            var sut = new EmpStat_04Rule(handler.Object, mockDDRule22.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            mockDDRule22.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public EmpStat_04Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            return new EmpStat_04Rule(handler.Object, mockDDRule22.Object);
        }
    }
}
