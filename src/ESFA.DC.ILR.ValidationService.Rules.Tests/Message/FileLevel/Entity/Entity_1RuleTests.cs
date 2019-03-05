using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Entity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Message.FileLevel.Entity
{
    public class Entity_1RuleTests : AbstractRuleTests<Entity_1Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Entity_1");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var testLearners = new TestLearner[]
            {
            };

            var testLearnerDestinationAndProgressions = new TestLearnerDestinationAndProgression[]
            {
            };

            NewRule().ConditionMet(learners: testLearners, learnerDestinationAndProgressions: testLearnerDestinationAndProgressions).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_Null()
        {
            TestLearner[] testLearners = null;

            TestLearnerDestinationAndProgression[] testLearnerDestinationAndProgressions = null;

            NewRule().ConditionMet(learners: testLearners, learnerDestinationAndProgressions: testLearnerDestinationAndProgressions).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var testLearners = new TestLearner[]
                {
                    new TestLearner()
                    {
                        Postcode = "ABC DEF"
                    }
                };

            var testLearnerDestinationAndProgressions = new TestLearnerDestinationAndProgression[]
            {
                new TestLearnerDestinationAndProgression()
                {
                    LearnRefNumber = "1234"
                }
            };

            NewRule().ConditionMet(learners: testLearners, learnerDestinationAndProgressions: testLearnerDestinationAndProgressions).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            TestLearner[] testLearners = null;

            var testLearnerDestinationAndProgressions = new TestLearnerDestinationAndProgression[]
            {
                new TestLearnerDestinationAndProgression()
                {
                    LearnRefNumber = "1234"
                }
            };

            NewRule().ConditionMet(learners: testLearners, learnerDestinationAndProgressions: testLearnerDestinationAndProgressions).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var testMessage = new TestMessage()
            {
                Learners = new TestLearner[]
                {
                },
                LearnerDestinationAndProgressions = new TestLearnerDestinationAndProgression[]
                {
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testMessage);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testMessage = new TestMessage()
            {
                Learners = new TestLearner[]
                {
                    new TestLearner()
                    {
                        LearnRefNumber = "123456"
                    }
                },
                LearnerDestinationAndProgressions = new TestLearnerDestinationAndProgression[]
                {
                    new TestLearnerDestinationAndProgression()
                    {
                        LearnRefNumber = "123456"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testMessage);
            }
        }

        public Entity_1Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new Entity_1Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
