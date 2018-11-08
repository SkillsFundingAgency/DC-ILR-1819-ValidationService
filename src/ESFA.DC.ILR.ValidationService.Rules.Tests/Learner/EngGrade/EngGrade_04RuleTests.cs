using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.EngGrade
{
    public class EngGrade_04RuleTests : AbstractRuleTests<EngGrade_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EngGrade_04");
        }

        [Fact]
        public void EngGradeConditionMet_True()
        {
            var engGrade = "A";

            NewRule().EngGradeConditionMet(engGrade).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("NONE")]
        public void EngGradeConditionMet_False(string engGrade)
        {
            NewRule().EngGradeConditionMet(engGrade).Should().BeFalse();
        }

        [Fact]
        public void LearnerFAMConditionMet_True()
        {
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(It.IsAny<IEnumerable<ILearnerFAM>>(), "ECF", new[] { 2, 3, 4 }))
                .Returns(true);

            NewRule(learnerFAMQueryServiceMock.Object).LearnerFAMConditionMet(It.IsAny<IEnumerable<ILearnerFAM>>()).Should().BeTrue();
        }

        [Fact]
        public void LearnerFamConditionMet_False()
        {
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(It.IsAny<IEnumerable<ILearnerFAM>>(), "ECF", new[] { 2, 3, 4 }))
                .Returns(false);

            NewRule(learnerFAMQueryServiceMock.Object).LearnerFAMConditionMet(It.IsAny<IEnumerable<ILearnerFAM>>()).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var engGrade = "A";

            var learnerFams = new List<TestLearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "ECF",
                    LearnFAMCode = 2
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(learnerFams, "ECF", new[] { 2, 3, 4 }))
                .Returns(true);

            NewRule(learnerFAMQueryServiceMock.Object).ConditionMet(engGrade, learnerFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseNoneGrade()
        {
            var engGrade = "NONE";

            var learnerFams = new List<TestLearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "ECF",
                    LearnFAMCode = 2
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(learnerFams, "ECF", new[] { 2, 3, 4 }))
                .Returns(true);

            NewRule(learnerFAMQueryServiceMock.Object).ConditionMet(engGrade, learnerFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseFAMType()
        {
            var engGrade = "A";

            var learnerFams = new List<TestLearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "XXX",
                    LearnFAMCode = 2
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(learnerFams, "ECF", new[] { 2, 3, 4 }))
                .Returns(false);

            NewRule(learnerFAMQueryServiceMock.Object).ConditionMet(engGrade, learnerFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseLearnerFAMNull()
        {
            var engGrade = "A";

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(null, "ECF", new[] { 2, 3, 4 }))
                .Returns(false);

            NewRule(learnerFAMQueryServiceMock.Object).ConditionMet(engGrade, null).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                EngGrade = "A",
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "ECF",
                        LearnFAMCode = 2
                    }
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(learner.LearnerFAMs, "ECF", new[] { 2, 3, 4 }))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner()
            {
                EngGrade = "NONE",
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "XXX",
                        LearnFAMCode = 2
                    }
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(learner.LearnerFAMs, "ECF", new[] { 2, 3, 4 }))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("EngGrade", "A")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("A");

            validationErrorHandlerMock.Verify();
        }

        private EngGrade_04Rule NewRule(
            ILearnerFAMQueryService learnerFamQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new EngGrade_04Rule(learnerFamQueryService, validationErrorHandler);
        }
    }
}
