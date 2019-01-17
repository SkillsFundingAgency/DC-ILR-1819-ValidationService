using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_43RuleTests : AbstractRuleTests<DateOfBirth_43Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_43");
        }

        [Fact]
        public void ValidatePasses_IrrelevantFundModel()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var derivedData23Mock = new Mock<IDerivedData_23Rule>();
            derivedData23Mock
                .Setup(m => m.GetLearnersAgeAtStartOfESFContract(It.IsAny<TestLearner>(), It.IsAny<List<TestLearningDelivery>>()))
                .Returns(14);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.AdultSkills
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, derivedData23Mock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_LearnerOldEnough()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var derivedData23Mock = new Mock<IDerivedData_23Rule>();
            derivedData23Mock
                .Setup(m => m.GetLearnersAgeAtStartOfESFContract(It.IsAny<TestLearner>(), It.IsAny<List<TestLearningDelivery>>()))
                .Returns(15);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.EuropeanSocialFund
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, derivedData23Mock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidateFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var derivedData23Mock = new Mock<IDerivedData_23Rule>();
            derivedData23Mock
                .Setup(m => m.GetLearnersAgeAtStartOfESFContract(It.IsAny<TestLearner>(), It.IsAny<List<TestLearningDelivery>>()))
                .Returns(14);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.EuropeanSocialFund
                    },
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.AdultSkills
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, derivedData23Mock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private DateOfBirth_43Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IDerivedData_23Rule derivedData23 = null)
        {
            return new DateOfBirth_43Rule(derivedData23, validationErrorHandler);
        }

        private void VerifyErrorHandlerMock(ValidationErrorHandlerMock errorHandlerMock, int times = 0)
        {
            errorHandlerMock.Verify(
                m => m.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()),
                Times.Exactly(times));
        }
    }
}
