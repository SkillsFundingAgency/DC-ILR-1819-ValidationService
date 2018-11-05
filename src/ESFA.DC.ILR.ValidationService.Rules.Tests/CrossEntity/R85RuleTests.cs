using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R85RuleTests : AbstractRuleTests<R85Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R85");
        }

        [Fact]
        public void Validate_True()
        {
            var learnerDP1 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999999 };
            var learnerDP2 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999998 };
            var learnerDP3 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 };
            var learnerDP4 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 };

            var learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                learnerDP1,
                learnerDP2,
                learnerDP3,
                learnerDP4
            };

            var message = new TestMessage()
            {
                Learners = new TestLearner[]
                {
                    new TestLearner()
                    {
                        LearnRefNumber = "Learner3",
                        ULN = 9999999999
                    }
                },
                LearnerDestinationAndProgressions = learnerDPs
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(message);
            }
        }

        [Fact]
        public void Validate_False()
        {
            var learnerDP1 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999999 };
            var learnerDP2 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999998 };
            var learnerDP3 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 };
            var learnerDP4 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 };

            var learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                learnerDP1,
                learnerDP2,
                learnerDP3,
                learnerDP4
            };

            var message = new TestMessage()
            {
                Learners = new TestLearner[] { new TestLearner { LearnRefNumber = "Learner1", ULN = 9999999999 } },
                LearnerDestinationAndProgressions = learnerDPs
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(message);
            }
        }

        [Fact]
        public void Validate_MisMatch()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "Learner1",
                ULN = 9999999999
            };
            var learnerDP3 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 };
            var learnerDP4 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 };

            var learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                learnerDP3,
                learnerDP4
            };
            var message = new TestMessage()
            {
                Learners = new TestLearner[] { learner },
                LearnerDestinationAndProgressions = learnerDPs
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(message);
            }
        }

        [Fact]
        public void Validate_NullDP()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "Learner1",
                ULN = 9999999999
            };

            var message = new TestMessage()
            {
                Learners = new TestLearner[] { learner },
                LearnerDestinationAndProgressions = null
            };
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(message);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ULN", 9999999999)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(9999999999, 999999998, "learner1");

            validationErrorHandlerMock.Verify();
        }

        private R85Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R85Rule(validationErrorHandler);
        }
    }
}
