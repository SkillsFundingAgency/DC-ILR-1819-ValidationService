using ESFA.DC.ILR.Model.Interface;
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
    /// from version 1.1 validation spread sheet
    /// </summary>
    public class AFinType_09RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new AFinType_09Rule(null));
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
            Assert.Equal("AFinType_09", result);
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
            Assert.Equal(AFinType_09Rule.Name, result);
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
        /// Condition met with learning delivery and null prog type returns false.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveryNullProgTypeReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns((IReadOnlyCollection<IAppFinRecord>)null);

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.False(result);
            mockDelivery.VerifyAll();
        }

        /// <summary>
        /// Condition met with learning delivery and empty apprenticeship financial records returns false.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveryAndEmptyApprenticeshipFinancialRecordsReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(Collection.EmptyAndReadOnly<IAppFinRecord>());

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.False(result);
            mockDelivery.VerifyAll();
        }

        /// <summary>
        /// Condition met with learning delivery and apprenticeship financial record returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveryAndApprenticeshipFinancialRecordReturnsTrue()
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            var appFinRecords = Collection.Empty<IAppFinRecord>();

            appFinRecords.Add(new Mock<IAppFinRecord>().Object);

            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(appFinRecords.AsSafeReadOnlyList());

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.True(result);
            mockDelivery.VerifyAll();
        }

        /// <summary>
        /// Determines whether [is apprenticeship returns expectation] [for the specified type of funding].
        /// </summary>
        /// <param name="forTypeOfFunding">The type of funding.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, false)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, true)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        public void IsApprenticeshipReturnsExpectation(int forTypeOfFunding, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(forTypeOfFunding);

            // act
            var result = sut.IsApprenticeship(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
            mockDelivery.VerifyAll();
        }

        /// <summary>
        /// Determines whether [is in training returns expectation] [for the specified type of funding and the aim].
        /// </summary>
        /// <param name="forTypeOfFunding">For type of funding.</param>
        /// <param name="andProgrammeType">and the Programme Type.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.AdvancedLevelApprenticeship, false)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.ApprenticeshipStandard, false)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel4, false)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.ApprenticeshipStandard, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.HigherApprenticeshipLevel5, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.ApprenticeshipStandard, false)]
        [InlineData(TypeOfFunding.CommunityLearning, TypeOfLearningProgramme.AdvancedLevelApprenticeship, false)]
        [InlineData(TypeOfFunding.CommunityLearning, TypeOfLearningProgramme.ApprenticeshipStandard, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel6, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.ApprenticeshipStandard, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLearningProgramme.ApprenticeshipStandard, false)]
        [InlineData(TypeOfFunding.Other16To19, TypeOfLearningProgramme.Traineeship, false)]
        [InlineData(TypeOfFunding.Other16To19, TypeOfLearningProgramme.ApprenticeshipStandard, false)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.IntermediateLevelApprenticeship, false)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.ApprenticeshipStandard, true)]
        public void IsInTrainingReturnsExpectation(int forTypeOfFunding, int andProgrammeType, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(forTypeOfFunding);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(andProgrammeType);

            // act
            var result = sut.IsInTraining(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Determines whether [is in a programme returns expectation] [for the specified type of aim].
        /// </summary>
        /// <param name="forTypeOfAim">For type of aim.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfAim.AimNotPartOfAProgramme, false)]
        [InlineData(TypeOfAim.ComponentAimInAProgramme, false)]
        [InlineData(TypeOfAim.CoreAim16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfAim.ProgrammeAim, true)]
        public void IsInAProgrammeReturnsExpectation(int forTypeOfAim, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(forTypeOfAim);

            // act
            var result = sut.IsInAProgramme(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
            mockDelivery.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="forTypeOfFunding">For type of funding.</param>
        /// <param name="andProgrammeType">Type of the and programme.</param>
        [Theory]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.AdvancedLevelApprenticeship)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.HigherApprenticeshipLevel4)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.HigherApprenticeshipLevel5)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.HigherApprenticeshipLevel6)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.IntermediateLevelApprenticeship)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.Traineeship)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.ApprenticeshipStandard)]
        public void InvalidItemRaisesValidationMessage(int forTypeOfFunding, int andProgrammeType)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(forTypeOfFunding);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(andProgrammeType);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns((IReadOnlyCollection<IAppFinRecord>)null);

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
                Moq.It.Is<string>(y => y == AFinType_09Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                null,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == AFinType_09Rule.MessagePropertyName),
                    Moq.It.Is<object>(y => y == mockDelivery.Object)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new AFinType_09Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// every case (bar three) is out of scope, and therefore by definition valid
        /// </summary>
        /// <param name="forTypeOfFunding">For type of funding.</param>
        /// <param name="andProgrammeType">Type of the and programme.</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.AdvancedLevelApprenticeship)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel4)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.HigherApprenticeshipLevel5)] // in scope
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.ApprenticeshipStandard)] // in scope
        [InlineData(TypeOfFunding.CommunityLearning, TypeOfLearningProgramme.AdvancedLevelApprenticeship)]
        [InlineData(TypeOfFunding.CommunityLearning, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel6)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.Other16To19, TypeOfLearningProgramme.Traineeship)]
        [InlineData(TypeOfFunding.Other16To19, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.IntermediateLevelApprenticeship)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.ApprenticeshipStandard)] // in scope
        public void ValidItemDoesNotRaiseAValidationMessage(int forTypeOfFunding, int andProgrammeType)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var appFinRecords = Collection.Empty<IAppFinRecord>();
            appFinRecords.Add(new Mock<IAppFinRecord>().Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(forTypeOfFunding);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(andProgrammeType);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(appFinRecords.AsSafeReadOnlyList());

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

            var sut = new AFinType_09Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public AFinType_09Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new AFinType_09Rule(mock.Object);
        }
    }
}
