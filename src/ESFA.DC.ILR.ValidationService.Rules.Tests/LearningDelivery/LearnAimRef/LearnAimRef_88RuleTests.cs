using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
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
    public class LearnAimRef_88RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_88Rule(null, service.Object, derivedData07.Object, derivedData11.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_88Rule(handler.Object, null, derivedData07.Object, derivedData11.Object));
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

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_88Rule(handler.Object, service.Object, null, derivedData11.Object));
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
            var derivedData07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_88Rule(handler.Object, service.Object, derivedData07.Object, null));
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
            Assert.Equal("LearnAimRef_88", result);
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
        /// Has valid start range with validity last new start date meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", true)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", true)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", true)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", false)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", false)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", false)]
        public void HasValidStartRangeWithValidityLastNewStartDateMeetsExpectation(string candidate, string startDate, string endDate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockValidity = new Mock<ILARSLearningDeliveryValidity>(MockBehavior.Strict);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(startDate));
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns((DateTime?)null);
            mockValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns(DateTime.Parse(endDate));

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            // act
            var result = sut.HasValidStartRange(mockValidity.Object, mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has valid start range with validity end date meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", true)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", true)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", true)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", false)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", false)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", false)]

        // the next 4 test cases account for the 'custom and practice' of
        // withdrawing funding for a learning aim by shifting the end date
        // of the validty to one day before the start date
        [InlineData("2013-07-17", "2013-07-16", "2013-07-15", false)]
        [InlineData("2013-07-16", "2013-07-16", "2013-07-15", false)]
        [InlineData("2013-07-15", "2013-07-16", "2013-07-15", false)]
        [InlineData("2013-07-14", "2013-07-16", "2013-07-15", false)]
        public void HasValidStartRangeWithValidityEndDateMeetsExpectation(string candidate, string startDate, string endDate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            // for testing, effective from and effect to,
            // this establishes whether the item is current or not
            var mockValidity = new Mock<ILARSLearningDeliveryValidity>(MockBehavior.Strict);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(startDate));
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.Parse(endDate));
            mockValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns((DateTime?)null);

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            // act
            var result = sut.HasValidStartRange(mockValidity.Object, mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has valid learning aim meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", true)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", true)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", true)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", false)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", false)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", false)]
        public void HasValidLearningAimMeetsExpectation(string candidate, string startDate, string endDate, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var category = "larsCat";
            var mockValidity = new Mock<ILARSLearningDeliveryValidity>(MockBehavior.Strict);
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(startDate));
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.Parse(endDate));
            mockValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns((DateTime?)null);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            larsValidities.Add(mockValidity.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var derivedData07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRef_88Rule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object);

            // act
            var result = sut.HasValidLearningAim(mockDelivery.Object, category);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="funding">The funding.</param>
        /// <param name="category">The category.</param>
        [Theory]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any)]
        public void InvalidItemRaisesValidationMessage(string candidate, string startDate, string endDate, int funding, string category)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(TypeOfAim.ComponentAimInAProgramme);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));
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
                    Moq.It.Is<string>(y => y == LearnAimRef_88Rule.Name),
                    Moq.It.Is<string>(y => y == learnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == LearnAimRefRuleBase.MessagePropertyName),
                    learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockValidity = new Mock<ILARSLearningDeliveryValidity>(MockBehavior.Strict);
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(startDate));
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.Parse(endDate));
            mockValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns((DateTime?)null);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            larsValidities.Add(mockValidity.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var derivedData07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRef_88Rule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="funding">The funding.</param>
        /// <param name="category">The category.</param>
        [Theory]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLARSValidity.EFA16To19)]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", TypeOfFunding.Other16To19, TypeOfLARSValidity.EFA16To19)]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", TypeOfFunding.CommunityLearning, TypeOfLARSValidity.CommunityLearning)]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", TypeOfFunding.NotFundedByESFA, TypeOfLARSValidity.Any)]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", TypeOfFunding.OtherAdult, TypeOfLARSValidity.Any)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, string startDate, string endDate, int funding, string category)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(TypeOfAim.ComponentAimInAProgramme);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));
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

            var mockValidity = new Mock<ILARSLearningDeliveryValidity>(MockBehavior.Strict);
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(startDate));
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.Parse(endDate));
            mockValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns((DateTime?)null);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            larsValidities.Add(mockValidity.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var derivedData07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRef_88Rule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            derivedData07.VerifyAll();
            derivedData11.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnAimRef_88Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var derivedData07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            return new LearnAimRef_88Rule(handler.Object, service.Object, derivedData07.Object, derivedData11.Object);
        }
    }
}
