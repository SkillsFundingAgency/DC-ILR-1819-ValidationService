using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R31RuleTests : AbstractRuleTests<R30Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R31");
        }

        [Fact]
        public void ValidationPasses()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<ILearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimType = 1,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1,
                        LearnActEndDateNullable = null
                    },
                    new TestLearningDelivery
                    {
                        AimType = 3,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();
            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<ILearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimType = 1,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1,
                        LearnActEndDateNullable = null
                    },
                    new TestLearningDelivery
                    {
                        AimType = 2,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1
                    },
                    new TestLearningDelivery
                    {
                        AimType = 3,
                        ProgTypeNullable = 2,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()));
        }

        [Fact]
        public void LearnActEndDateIsIgnored()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<ILearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimType = 1,
                        ProgTypeNullable = 25,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1,
                        LearnActEndDateNullable = DateTime.Now
                    },
                    new TestLearningDelivery
                    {
                        AimType = 2,
                        ProgTypeNullable = 25,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        private R31Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R31Rule(validationErrorHandler);
        }
    }
}
