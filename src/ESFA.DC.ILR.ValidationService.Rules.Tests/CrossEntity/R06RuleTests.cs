using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R06RuleTests : AbstractRuleTests<R06Rule>
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R06Rule(null));
        }

        /// <summary>
        /// Rule name 1, matches a literal.
        /// </summary>
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("R06", result);
        }

        /// <summary>
        /// Rule name 3 test, account for potential false positives.
        /// </summary>
        [Fact]
        public void RuleName3()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        /// <summary>
        /// Validate with null learner.
        /// </summary>
        [Fact]
        public void ValidateWithNullMessage()
        {
            NewRule().Validate(null);
        }

        /// <summary>
        /// Validate with null learner.
        /// </summary>
        [Fact]
        public void ValidateWithEmptyMessage()
        {
            var testMessage = new TestMessage()
            {
                Learners = new TestLearner[]
                {
                    new TestLearner()
                    {
                    },
                    new TestLearner()
                    {
                    }
                }
            };
            NewRule().Validate(testMessage);
        }

        [Fact]
        public void CheckForDuplicate_LearnRefNumber_Duplicate()
        {
            var testMessage = new TestMessage()
            {
                Learners = new TestLearner[]
                {
                    new TestLearner()
                    {
                        LearnRefNumber = "123456"
                    },
                    new TestLearner()
                    {
                        LearnRefNumber = "123456"
                    }
                }
            };

            NewRule().Validate(testMessage);
        }

        [Fact]
        public void CheckForDuplicate_LearnRefNumber_NoDuplicate()
        {
            var testMessage = new TestMessage()
            {
                Learners = new TestLearner[]
                {
                    new TestLearner()
                    {
                    },
                    new TestLearner()
                    {
                        LearnRefNumber = "123456"
                    },
                    new TestLearner()
                    {
                        LearnRefNumber = "1234567"
                    }
                }
            };

            NewRule().Validate(testMessage);
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public R06Rule NewRule()
        {
            var handler = BuildValidationErrorHandlerMockForError();
            return new R06Rule(handler.Object);
        }
    }
}
