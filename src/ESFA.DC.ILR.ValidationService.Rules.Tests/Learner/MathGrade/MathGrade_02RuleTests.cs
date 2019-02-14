using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.MathGrade
{
    public class MathGrade_02RuleTests : AbstractRuleTests<MathGrade_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("MathGrade_02");
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var mathGrade = Grades.AstarA;
            var validaionErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validaionErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.MathGrade, mathGrade)).Verifiable();
            NewRule(validationErrorHandler: validaionErrorHandlerMock.Object).BuildErrorMessageParameters(mathGrade);
            validaionErrorHandlerMock.Verify();
        }

        [Fact]
        public void MathGradeConditionMet_False()
        {
            string mathGrade = "A";
            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.GCSEGrade, mathGrade)).Returns(true);
            NewRule(provideLookupDetails: provideLookupDetails.Object).MathGradeConditionMet(mathGrade).Should().BeFalse();
        }

        [Theory]
        [InlineData(Grades.AstarA)]
        [InlineData("a*a*")]
        public void EngGradeConditionMet_True(string mathGrade)
        {
            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.GCSEGrade, mathGrade)).Returns(false);
            NewRule(provideLookupDetails: provideLookupDetails.Object).MathGradeConditionMet(mathGrade).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(Grades.NONE)]
        [InlineData("none")]
        public void MathGradeSuppliedAndNotNone_False(string mathGrade)
        {
            NewRule().MathGradeSuppliedAndNotNone(mathGrade).Should().BeFalse();
        }

        [Theory]
        [InlineData("a*a")]
        [InlineData(Grades.AstarAstar)]
        public void EngGradeSuppliedAndNotNone_True(string mathGrade)
        {
            NewRule().MathGradeSuppliedAndNotNone(mathGrade).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            string mathGrade = "QQ";
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "AB12345",
                MathGrade = mathGrade,
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.GCSEGrade, mathGrade)).Returns(false);
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(Grades.NONE)]
        [InlineData("")]
        [InlineData(Grades.AB)]
        [InlineData("ab")]
        public void Validate_NoError(string mathGrade)
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "AB12345",
                MathGrade = mathGrade
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.GCSEGrade, mathGrade)).Returns(true);
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoGradeSupplied_NoError()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "AB12345",
                LearningDeliveries = new[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "1A234"
                    }
                }
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.GCSEGrade, Grades.AstarA)).Returns(true);
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_NullCheck()
        {
            TestLearner testLearner = null;
            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.GCSEGrade, Grades.AstarA)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(testLearner);
            }
        }

        public MathGrade_02Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IProvideLookupDetails provideLookupDetails = null)
        {
            return new MathGrade_02Rule(validationErrorHandler, provideLookupDetails);
        }
    }
}
