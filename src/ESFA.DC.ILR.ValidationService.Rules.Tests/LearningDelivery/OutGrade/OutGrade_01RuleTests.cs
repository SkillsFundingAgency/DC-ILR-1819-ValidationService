using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OutGrade
{
    public class OutGrade_01RuleTests : AbstractRuleTests<OutGrade_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutGrade_01");
        }

        [Theory]
        [InlineData("9999")]
        [InlineData("***A")]
        [InlineData("ZZZ")]
        [InlineData("P1")]
        [InlineData("1")]
        [InlineData("L2")]
        [InlineData("D**")]
        [InlineData("***D")]
        public void ConditionMet_True(string outGrade)
        {
            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.OutGrade, outGrade)).Returns(false);
            NewRule(provideLookupDetails: provideLookupDetails.Object).ConditionMet(outGrade).Should().BeTrue();
        }

        [Theory]
        [InlineData("**")]
        [InlineData("A*A")]
        [InlineData("16")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ConditionMet_False(string outGrade)
        {
            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.OutGrade, outGrade)).Returns(true);
            NewRule(provideLookupDetails: provideLookupDetails.Object).ConditionMet(outGrade).Should().BeFalse();
        }

        [Theory]
        [InlineData("9999")]
        [InlineData("***A")]
        [InlineData("ZZZ")]
        public void ValidateError(string outGrade)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutGrade = outGrade
                    }
                }
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.OutGrade, outGrade)).Returns(false);
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData("**")]
        [InlineData("A*A")]
        [InlineData("16")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateNoError(string outGrade)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutGrade = outGrade
                    }
                }
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.OutGrade, outGrade)).Returns(true);
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullLearningDeliveries()
        {
            var learner = new TestLearner
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
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutGrade", "A")).Verifiable();
            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("A");
            validationErrorHandlerMock.Verify();
        }

        private OutGrade_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IProvideLookupDetails provideLookupDetails = null)
        {
            return new OutGrade_01Rule(validationErrorHandler, provideLookupDetails);
        }
    }
}
