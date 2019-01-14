using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_89RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_89Rule(null, service.Object, derivedData07.Object, derivedData11.Object, yearData.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_89Rule(handler.Object, null, derivedData07.Object, derivedData11.Object, yearData.Object));
        }

        /// <summary>
        /// New rule with null derived data 07 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedData07Throws()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_89Rule(handler.Object, service.Object, null, derivedData11.Object, yearData.Object));
        }

        /// <summary>
        /// New rule with null derived data 11 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedData11Throws()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_89Rule(handler.Object, service.Object, derivedData07.Object, null, yearData.Object));
        }

        /// <summary>
        /// New rule with null year data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullYearDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_89Rule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object, null));
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
            Assert.Equal("LearnAimRef_89", result);
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
            Assert.Equal(sut.GetName(), result);
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
        /// Gets the closing date of last academic year meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData("2017-08-26", "2017-03-06")]
        [InlineData("2017-09-15", "2016-08-12")]
        [InlineData("2018-09-09", "2017-06-18")]
        [InlineData("2015-04-21", "2017-01-07")]
        [InlineData("2015-09-23", "2016-02-28")]
        public void GetClosingDateOfLastAcademicYearMeetsExpectation(string candidate, string expectation)
        {
            // arrange
            var learnStart = DateTime.Parse(candidate);
            var testDate = DateTime.Parse(expectation);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(learnStart, AcademicYearDates.PreviousYearEnd))
                .Returns(testDate);

            var sut = new LearnAimRef_89Rule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object, yearData.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStart);

            // act, not interested in the result just that we hit a strict signature
            var result = sut.GetClosingDateOfLastAcademicYear(mockDelivery.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();
            yearData.VerifyAll();

            Assert.Equal(testDate, result);
        }

        /// <summary>
        /// Has valid learning aim meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="previousYearEnd">The previous year end.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        /// <param name="startDates">The start dates.</param>
        [Theory]
        [InlineData("2018-04-01", "2017-07-31", true, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", true, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        [InlineData("2018-04-01", "2017-07-31", false, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", false, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        public void HasValidLearningAimMeetsExpectation(string candidate, string previousYearEnd, bool expectation, params string[] startDates)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse(candidate);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            startDates.ForEach(sd =>
            {
                var mockValidity = new Mock<ILARSLearningDeliveryValidity>();
                mockValidity
                    .SetupGet(x => x.StartDate)
                    .Returns(DateTime.Parse(sd));
                mockValidity
                    .SetupGet(x => x.EndDate)
                    .Returns(DateTime.Parse(sd));

                larsValidities.Add(mockValidity.Object);
            });

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(DateTime.Parse(previousYearEnd));

            var sut = new LearnAimRef_89Rule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object, yearData.Object);

            // act
            var result = sut.HasValidLearningAim(mockDelivery.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();
            yearData.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2018-04-01", "2017-07-31", true, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", true, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        [InlineData("2018-04-01", "2017-07-31", true, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", true, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        public void HasValidLearningAimWithNulEndDatesMeetsExpectation(string candidate, string previousYearEnd, bool expectation, params string[] startDates)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse(candidate);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            startDates.ForEach(sd =>
            {
                var mockValidity = new Mock<ILARSLearningDeliveryValidity>();
                mockValidity
                    .SetupGet(x => x.StartDate)
                    .Returns(DateTime.Parse(sd));

                larsValidities.Add(mockValidity.Object);
            });

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(DateTime.Parse(previousYearEnd));

            var sut = new LearnAimRef_89Rule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object, yearData.Object);

            // act
            var result = sut.HasValidLearningAim(mockDelivery.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();
            yearData.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="previousYearEnd">The previous year end.</param>
        /// <param name="funding">The funding.</param>
        /// <param name="category">The category.</param>
        /// <param name="startDates">The start dates.</param>
        [Theory]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        public void InvalidItemRaisesValidationMessage(string candidate, string previousYearEnd, int funding, string category, params string[] startDates)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse(candidate);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(TypeOfAim.ComponentAimInAProgramme);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == LearnAimRef_89Rule.Name),
                    Moq.It.Is<string>(y => y == learnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == LearnAimRefRuleBase.MessagePropertyName),
                    learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            startDates.ForEach(sd =>
            {
                var mockValidity = new Mock<ILARSLearningDeliveryValidity>();
                mockValidity
                    .SetupGet(x => x.ValidityCategory)
                    .Returns(category);
                mockValidity
                    .SetupGet(x => x.StartDate)
                    .Returns(DateTime.Parse(sd));
                mockValidity
                    .SetupGet(x => x.EndDate)
                    .Returns(DateTime.Parse(sd));

                larsValidities.Add(mockValidity.Object);
            });

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(DateTime.Parse(previousYearEnd));

            var sut = new LearnAimRef_89Rule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object, yearData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();
            yearData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="previousYearEnd">The previous year end.</param>
        /// <param name="funding">The funding.</param>
        /// <param name="category">The category.</param>
        /// <param name="startDates">The start dates.</param>
        [Theory]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, string previousYearEnd, int funding, string category, params string[] startDates)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse(candidate);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(TypeOfAim.ComponentAimInAProgramme);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            startDates.ForEach(sd =>
            {
                var mockValidity = new Mock<ILARSLearningDeliveryValidity>();
                mockValidity
                    .SetupGet(x => x.ValidityCategory)
                    .Returns(category);
                mockValidity
                    .SetupGet(x => x.StartDate)
                    .Returns(DateTime.Parse(sd));
                mockValidity
                    .SetupGet(x => x.EndDate)
                    .Returns(DateTime.Parse(sd));

                larsValidities.Add(mockValidity.Object);
            });

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(DateTime.Parse(previousYearEnd));

            var sut = new LearnAimRef_89Rule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object, yearData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();
            yearData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnAimRef_89Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDD07>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            return new LearnAimRef_89Rule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object, yearData.Object);
        }
    }
}
