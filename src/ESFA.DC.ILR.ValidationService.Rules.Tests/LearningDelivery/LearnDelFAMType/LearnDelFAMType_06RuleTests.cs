using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_06RuleTests : AbstractRuleTests<LearnDelFAMType_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_06");
        }

        [Fact]
        public void ValidationPasses()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var lookupProviderMock = new Mock<IProvideLookupDetails>();
            lookupProviderMock.Setup(m =>
                    m.IsCurrent(LookupComplexKey.LearnDelFAMType, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(true);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "034"
                            }
                        },
                        LearnStartDate = new DateTime(2018, 09, 01)
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, lookupProviderMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();
            var lookupProviderMock = new Mock<IProvideLookupDetails>();
            lookupProviderMock.Setup(m =>
                    m.IsCurrent(LookupComplexKey.LearnDelFAMType, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(false);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "034"
                            }
                        },
                        LearnStartDate = new DateTime(2018, 09, 01)
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, lookupProviderMock.Object).Validate(testLearner);
            validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<string>()));
        }

        private LearnDelFAMType_06Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IProvideLookupDetails lookupProvider = null)
        {
            return new LearnDelFAMType_06Rule(validationErrorHandler, lookupProvider);
        }
    }
}
