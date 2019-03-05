using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.TYPEYR;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.TYPEYR
{
    public class TYPEYR_01RuleTests : AbstractRuleTests<TYPEYR_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("TYPEYR_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            for (var i = 1; i <= 5; i++)
            {
                var learningDeliveryHe = new TestLearningDeliveryHE()
                {
                    TYPEYR = i
                };

                var provideLookupDetails = new Mock<IProvideLookupDetails>();
                provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.TypeYr, learningDeliveryHe.TYPEYR.ToString())).Returns(true);
                NewRule(provideLookupDetails: provideLookupDetails.Object).ConditionMet(learningDeliveryHe.TYPEYR.ToString()).Should().BeFalse();
            }
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                TYPEYR = 6
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.TypeYr, learningDeliveryHe.TYPEYR.ToString())).Returns(false);
            NewRule(provideLookupDetails: provideLookupDetails.Object).ConditionMet(learningDeliveryHe.TYPEYR.ToString()).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var typeYr = 6;
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            TYPEYR = typeYr
                        }
                    }
                }
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.TypeYr, typeYr.ToString())).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_MultipleDeliveries_Error()
        {
            var typeYr = 6;
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            TYPEYR = typeYr
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            TYPEYR = 1
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = null
                    }
                }
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.TypeYr, typeYr.ToString())).Returns(false);
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.TypeYr, "1")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var typeYr = 1;
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            TYPEYR = typeYr
                        }
                    }
                }
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.TypeYr, typeYr.ToString())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_MultipleDeliveries_NoError()
        {
            var typeYr = 1;
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            TYPEYR = typeYr
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            TYPEYR = 2
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = null
                    }
                }
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.TypeYr, typeYr.ToString())).Returns(true);
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.TypeYr, "2")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NullLearningDeliveryHE_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = null
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NullLearningDelivery_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.TYPEYR, 1)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private TYPEYR_01Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IProvideLookupDetails provideLookupDetails = null)
        {
            return new TYPEYR_01Rule(validationErrorHandler, provideLookupDetails);
        }
    }
}
