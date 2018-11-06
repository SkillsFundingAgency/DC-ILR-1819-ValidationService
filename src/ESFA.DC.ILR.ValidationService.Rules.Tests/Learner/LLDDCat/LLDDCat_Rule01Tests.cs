using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.LLDDCat.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDCat;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LLDDCat
{
    public class LLDDCat_Rule01Tests : AbstractRuleTests<LLDDCat_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LLDDCat_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var llddCat = 1;

            var llddCatDataServiceMock = new Mock<ILLDDCatDataService>();

            llddCatDataServiceMock.Setup(ds => ds.Exists(llddCat)).Returns(false);

            NewRule(llddCatDataServiceMock.Object).ConditionMet(llddCat).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var llddCat = 1;

            var llddCatDataServiceMock = new Mock<ILLDDCatDataService>();

            llddCatDataServiceMock.Setup(ds => ds.Exists(llddCat)).Returns(true);

            NewRule(llddCatDataServiceMock.Object).ConditionMet(llddCat).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var llddCat = 1;

            var learner = new TestLearner()
            {
                LLDDAndHealthProblems = new List<TestLLDDAndHealthProblem>
                {
                    new TestLLDDAndHealthProblem
                    {
                        PrimaryLLDDNullable = 1,
                        LLDDCat = llddCat
                    }
                }
            };

            var llddCatDataServiceMock = new Mock<ILLDDCatDataService>();

            llddCatDataServiceMock.Setup(ds => ds.Exists(llddCat)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(llddCatDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var llddCat = 1;

            var learner = new TestLearner()
            {
                LLDDAndHealthProblems = new List<TestLLDDAndHealthProblem>
                {
                    new TestLLDDAndHealthProblem
                    {
                        PrimaryLLDDNullable = 1,
                        LLDDCat = llddCat
                    }
                }
            };

            var llddCatDataServiceMock = new Mock<ILLDDCatDataService>();

            llddCatDataServiceMock.Setup(ds => ds.Exists(llddCat)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(llddCatDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LLDDCat", 1)).Verifiable();

            NewRule(null, validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private LLDDCat_01Rule NewRule(ILLDDCatDataService llddCatDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new LLDDCat_01Rule(llddCatDataService, validationErrorHandler);
        }
    }
}