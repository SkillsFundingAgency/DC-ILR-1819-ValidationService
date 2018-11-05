using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OutGrade
{
    public class OutGrade_03RuleTests : AbstractRuleTests<OutGrade_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutGrade_03");
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var outGrade = "ABC";

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutGrade", "ABC")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(outGrade);

            validationErrorHandlerMock.Verify();
        }

        [Fact]
        public void OutcomeCondtionMet_True()
        {
            NewRule().OutcomeCondtionMet(1).Should().BeTrue();
        }

        [Fact]
        public void OutcomeCondtionMet_False()
        {
            NewRule().OutcomeCondtionMet(2).Should().BeFalse();
        }

        [Fact]
        public void OutcomeCondtionMet_False_Null()
        {
            NewRule().OutcomeCondtionMet(null).Should().BeFalse();
        }

        [Fact]
        public void OutGradeCondtionMet_True()
        {
            NewRule().OutGradeCondtionMet("FM1").Should().BeTrue();
        }

        [Fact]
        public void OutGradeCondtionMet_False()
        {
            NewRule().OutGradeCondtionMet("EL1").Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelMatchForLearnAimRef(learnAimRef, "E")).Returns(true);
            larsDataServiceMock.Setup(ds => ds.BasicSkillsMatchForLearnAimRef(learnAimRef, 1)).Returns(true);

            NewRule(larsDataServiceMock.Object).LARSConditionMet(learnAimRef).Should().BeTrue();
        }

        [Theory]
        [InlineData("LearnAimRef", "E", 2, true, false)]
        [InlineData("LearnAimRef", "F", 2, false, false)]
        [InlineData("LearnAimRef", "F", 1, false, true)]
        [InlineData("NotLearnAimRef", "E", 1, false, false)]
        public void LARSConditionMet_False(string learnAimRef, string level, int basicSkills, bool mock1, bool mock2)
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelMatchForLearnAimRef(learnAimRef, level)).Returns(mock1);
            larsDataServiceMock.Setup(ds => ds.BasicSkillsMatchForLearnAimRef(learnAimRef, basicSkills)).Returns(mock2);

            NewRule(larsDataServiceMock.Object).LARSConditionMet("LearnAimRef").Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var outcome = 1;
            var outGrade = "FM1";
            var learnAimRef = "LearnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelMatchForLearnAimRef(learnAimRef, "E")).Returns(true);
            larsDataServiceMock.Setup(ds => ds.BasicSkillsMatchForLearnAimRef(learnAimRef, 1)).Returns(true);

            NewRule(larsDataServiceMock.Object).ConditionMet(outcome, outGrade, learnAimRef).Should().BeTrue();
        }

        [Theory]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(false, true, true)]
        [InlineData(false, false, true)]
        [InlineData(false, false, false)]
        public void ConditionMet_False(bool mock1, bool mock2, bool mock3)
        {
            var outcome = 1;
            var outGrade = "FM1";
            var learnAimRef = "LearnAimRef";

            var rule = NewRuleMock();

            rule.Setup(r => r.OutcomeCondtionMet(outcome)).Returns(mock1);
            rule.Setup(r => r.OutGradeCondtionMet(outGrade)).Returns(mock2);
            rule.Setup(r => r.LARSConditionMet(learnAimRef)).Returns(mock3);

            rule.Object.ConditionMet(outcome, outGrade, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var outcome = 1;
            var outGrade = "FM1";
            var learnAimRef = "LearnAimRef";

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        OutcomeNullable = outcome,
                        OutGrade = outGrade
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelMatchForLearnAimRef("LearnAimRef", "E")).Returns(true);
            larsDataServiceMock.Setup(ds => ds.BasicSkillsMatchForLearnAimRef("LearnAimRef", 1)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var outcome = 1;
            var outGrade = "FM1";
            var learnAimRef = "LearnAimRef";

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        OutcomeNullable = outcome,
                        OutGrade = outGrade
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelMatchForLearnAimRef("LearnAimRef", "E")).Returns(true);
            larsDataServiceMock.Setup(ds => ds.BasicSkillsMatchForLearnAimRef("LearnAimRef", 1)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private OutGrade_03Rule NewRule(ILARSDataService larsDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutGrade_03Rule(larsDataService, validationErrorHandler);
        }
    }
}
