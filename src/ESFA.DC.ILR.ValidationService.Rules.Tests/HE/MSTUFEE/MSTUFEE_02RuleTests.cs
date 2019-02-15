using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.MSTUFEE;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.MSTUFEE
{
    public class MSTUFEE_02RuleTests : AbstractRuleTests<MSTUFEE_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("MSTUFEE_02");
        }

        [Theory]
        [InlineData(1, "2019/01/01", true, false)]
        [InlineData(2, "2018/07/01", true, false)]
        [InlineData(4, "2018/12/01", false, true)]
        [InlineData(5, "2018/09/11", false, true)]
        public void ConditionMetMeetsExpectation(int mstuFee, string learnStartDateString, bool lookUpValid, bool expectation)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);

            var providerLookupDetailsMock = new Mock<IProvideLookupDetails>();

            providerLookupDetailsMock.Setup(q => q.IsCurrent(TypeOfLimitedLifeLookup.MSTuFee, mstuFee, learnStartDate)).Returns(lookUpValid);

            NewRule(provideLookupDetails: providerLookupDetailsMock.Object).ConditionMet(learnStartDate, mstuFee).Should().Be(expectation);
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2013, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                           MSTUFEE = 2
                        }
                    }
                }
            };

            var providerLookupDetailsMock = new Mock<IProvideLookupDetails>();

            providerLookupDetailsMock.Setup(q => q.IsCurrent(TypeOfLimitedLifeLookup.MSTuFee, 2, new DateTime(2013, 01, 01))).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandlerMock.Object,
                    providerLookupDetailsMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_No_Error()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2013, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            MSTUFEE = 2
                        }
                    }
                }
            };

            var providerLookupDetailsMock = new Mock<IProvideLookupDetails>();

            providerLookupDetailsMock.Setup(q => q.IsCurrent(TypeOfLimitedLifeLookup.MSTuFee, 2, new DateTime(2013, 01, 01))).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandlerMock.Object,
                    providerLookupDetailsMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NullLearningDeliveryHEEntity_NoError()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2013, 01, 01)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoLearningDeliveries_NoError()
        {
            var testLearner = new TestLearner();

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "31/07/2013")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.MSTUFEE, 2)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2013, 07, 31), 2);

            validationErrorHandlerMock.Verify();
        }

        public MSTUFEE_02Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IProvideLookupDetails provideLookupDetails = null)
        {
            return new MSTUFEE_02Rule(
                validationErrorHandler,
                provideLookupDetails);
        }
    }
}
