using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LearnFAMType
{
    public class LearnFAMType_10RuleTests : AbstractRuleTests<LearnFAMType_10Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnFAMType_10");
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(4, false)]
        [InlineData(5, true)]
        public void ConditionMetMeetsExpectation(int count, bool expectation)
        {
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMQueryServiceMock.Setup(s => s.GetLearnerFAMsCountByFAMType(It.IsAny<List<ILearnerFAM>>(), LearnerFAMTypeConstants.LSR)).Returns(count);

            NewRule(learnerFAMQueryServiceMock.Object).ConditionMet(It.IsAny<List<ILearnerFAM>>()).Should().Be(expectation);
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.LSR
                    }
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(x => x.GetLearnerFAMsCountByFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), LearnerFAMTypeConstants.LSR))
                .Returns(5);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner
            {
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.LSR
                    }
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(x => x.GetLearnerFAMsCountByFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), LearnerFAMTypeConstants.LSR))
                .Returns(4);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_WithNoLearnerFAMS_Returns_NoError()
        {
            var learner = new TestLearner();

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(x => x.GetLearnerFAMsCountByFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(1);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuilderErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.LSR)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearnerFAMTypeConstants.LSR);

            validationErrorHandlerMock.Verify();
        }

        private LearnFAMType_10Rule NewRule(
            ILearnerFAMQueryService learnerFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnFAMType_10Rule(learnerFAMQueryService, validationErrorHandler);
        }
    }
}
