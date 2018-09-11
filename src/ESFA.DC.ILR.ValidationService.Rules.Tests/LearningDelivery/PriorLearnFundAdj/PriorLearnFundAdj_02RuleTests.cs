using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PriorLearnFundAdj;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.PriorLearnFundAdj
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// </summary>
    public class PriorLearnFundAdj_02RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new PriorLearnFundAdj_02Rule(null));
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
            Assert.Equal("PriorLearnFundAdj_02", result);
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
            Assert.Equal(PriorLearnFundAdj_02Rule.Name, result);
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

        [Theory]
        [InlineData(TypeOfAim.ComponentAimInAProgramme, true)]
        [InlineData(TypeOfAim.AimNotPartOfAProgramme, true)]
        [InlineData(TypeOfAim.CoreAim16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfAim.ProgrammeAim, false)]
        [InlineData(2, false)]
        public void IsComponentAimMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(candidate);

            // act
            var result = sut.IsComponentAim(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Determines whether [is right fund model (and) meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        public void IsRightFundModelMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            // act
            var result = sut.IsRightFundModel(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.ADL, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.ALB, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.ASL, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.HHS, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.LSF, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.RES, true)]
        [InlineData(LearningDeliveryFAMTypeConstants.SOF, false)]
        public void IsRestartMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockFam = new Mock<ILearningDeliveryFAM>();
            mockFam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            var fams = Collection.Empty<ILearningDeliveryFAM>();
            fams.Add(mockFam.Object);

            // act
            var result = sut.IsRestart(fams.AsSafeReadOnlyList());

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Determines whether [is restart with null fams returns false].
        /// </summary>
        [Fact]
        public void IsRestartWithNullFAMsReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.IsRestart(null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met for prior learn fund adj meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, true)]
        [InlineData(26, false)]
        public void ConditionMetForPriorLearnFundAdjMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.PriorLearnFundAdjNullable)
                .Returns(candidate);

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="typeofFunding">The typeof funding.</param>
        /// <param name="typeOfAim">The type of aim.</param>
        /// <param name="famType">Type of the fam.</param>
        /// <param name="priorAdjustment">The prior adjustment.</param>
        /// <param name="aimSeqNumber">The aim seq number.</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfAim.ComponentAimInAProgramme, LearningDeliveryFAMTypeConstants.RES, 26, 1)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfAim.AimNotPartOfAProgramme, LearningDeliveryFAMTypeConstants.RES, 43, 2)]
        public void InvalidItemRaisesValidationMessage(int typeofFunding, int typeOfAim, string famType, int? priorAdjustment, int aimSeqNumber)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockFam = new Mock<ILearningDeliveryFAM>(MockBehavior.Strict);
            mockFam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);

            var fams = Collection.Empty<ILearningDeliveryFAM>();
            fams.Add(mockFam.Object);

            var mockDelivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            mockDelivery
                .SetupGet(y => y.AimSeqNumber)
                .Returns(aimSeqNumber);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(typeofFunding);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(typeofFunding);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(typeOfAim);
            mockDelivery
                .SetupGet(y => y.PriorLearnFundAdjNullable)
                .Returns(priorAdjustment);
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(fams.AsSafeReadOnlyList());

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == PriorLearnFundAdj_02Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                aimSeqNumber,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PriorLearnFundAdj_02Rule.MessagePropertyName),
                    Moq.It.IsAny<ILearningDelivery>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new PriorLearnFundAdj_02Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            mockFam.VerifyAll();
            mockDelivery.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// the definitive list of test cases is far greater than this; but it is indicative of good behaviour
        /// </summary>
        /// <param name="typeofFunding">The typeof funding.</param>
        /// <param name="typeOfAim">The type of aim.</param>
        /// <param name="famType">Type of the fam.</param>
        /// <param name="priorAdjustment">The prior adjustment.</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfAim.ComponentAimInAProgramme, LearningDeliveryFAMTypeConstants.RES, null)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfAim.AimNotPartOfAProgramme, LearningDeliveryFAMTypeConstants.RES, null)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfAim.AimNotPartOfAProgramme, LearningDeliveryFAMTypeConstants.ACT, 23)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfAim.AimNotPartOfAProgramme, LearningDeliveryFAMTypeConstants.ASL, 23)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfAim.CoreAim16To19ExcludingApprenticeships, LearningDeliveryFAMTypeConstants.RES, 23)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfAim.ProgrammeAim, LearningDeliveryFAMTypeConstants.RES, 23)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfAim.ComponentAimInAProgramme, LearningDeliveryFAMTypeConstants.RES, 23)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfAim.AimNotPartOfAProgramme, LearningDeliveryFAMTypeConstants.RES, 23)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.ComponentAimInAProgramme, LearningDeliveryFAMTypeConstants.RES, 23)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.AimNotPartOfAProgramme, LearningDeliveryFAMTypeConstants.RES, 23)]
        public void ValidItemDoesNotRaiseAValidationMessage(int typeofFunding, int typeOfAim, string famType, int? priorAdjustment)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockFam = new Mock<ILearningDeliveryFAM>();
            mockFam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);

            var fams = Collection.Empty<ILearningDeliveryFAM>();
            fams.Add(mockFam.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(typeofFunding);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(typeOfAim);
            mockDelivery
                .SetupGet(y => y.PriorLearnFundAdjNullable)
                .Returns(priorAdjustment);
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(fams.AsSafeReadOnlyList());

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new PriorLearnFundAdj_02Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public PriorLearnFundAdj_02Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new PriorLearnFundAdj_02Rule(mock.Object);
        }
    }
}
