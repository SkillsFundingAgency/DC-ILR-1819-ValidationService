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
    public class R30RuleTests : AbstractRuleTests<R30Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R30");
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
                        AimType = 3,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1
                    },
                    new TestLearningDelivery
                    {
                        AimType = 1,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationSingleDeliveryFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();
            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<ILearningDelivery>
                {
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
            validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()));
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
                        AimType = 3,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1
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
                        AimType = 1,
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
        public void ProgType25IsIgnored()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<ILearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimType = 3,
                        ProgTypeNullable = 25,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1
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

        private R30Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R30Rule(validationErrorHandler);
        }
    }
}
