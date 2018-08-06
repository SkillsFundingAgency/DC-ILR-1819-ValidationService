using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade;
using ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.EngGrade
{
    public class EngGrade_03RuleTests
    {
        [Theory]
        [InlineData("D")]
        [InlineData("DD")]
        [InlineData("DE")]
        [InlineData("E")]
        [InlineData("EE")]
        [InlineData("EF")]
        [InlineData("F")]
        [InlineData("FF")]
        [InlineData("FG")]
        [InlineData("G")]
        [InlineData("GG")]
        [InlineData("N")]
        [InlineData("U")]
    public void ConditionMet_True(string mathGrade)
        {
            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService.Setup(x => x.HasLearnerFAMCodeForType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>(), It.IsAny<long>()))
                .Returns(false);

            var rule = new MathGrade_03Rule(null, learnerFamQueryService.Object);

            rule.ConditionMet(mathGrade, It.IsAny<IReadOnlyCollection<ILearnerFAM>>()).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("D")]
        [InlineData("DD")]
        [InlineData("DE")]
        [InlineData("E")]
        [InlineData("EE")]
        [InlineData("EF")]
        [InlineData("F")]
        [InlineData("FF")]
        [InlineData("FG")]
        [InlineData("G")]
        [InlineData("GG")]
        [InlineData("N")]
        [InlineData("U")]
        public void ConditionMet_False(string mathGrade)
        {
            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService.Setup(x => x.HasLearnerFAMCodeForType(It.IsAny<IEnumerable<ILearnerFAM>>(), "EDF", 2))
                                    .Returns(true);

            var rule = new EngGrade_03Rule(null, learnerFamQueryService.Object);
            var learnerFams = new[]
            {
                new TestLearnerFAM
                {
                    LearnFAMType = "EDF",
                    LearnFAMCodeNullable = 2
                },
                new TestLearnerFAM
                {
                    LearnFAMType = "XYZ",
                    LearnFAMCodeNullable = 2
                }
            };

            rule.ConditionMet(mathGrade, learnerFams).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner("D");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("EngGrade_03", null, null, null);

            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService.Setup(x => x.HasLearnerFAMCodeForType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>(), It.IsAny<long>()))
                .Returns(false);

            var rule = new EngGrade_03Rule(validationErrorHandlerMock.Object, learnerFamQueryService.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner("FF");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("EngGrade_03", null, null, null);

            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService.Setup(x => x.HasLearnerFAMCodeForType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>(), It.IsAny<long>()))
                .Returns(true);

            var rule = new EngGrade_03Rule(validationErrorHandlerMock.Object, learnerFamQueryService.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private static TestLearner SetupLearner(string engGrade)
        {
            var learner = new TestLearner()
            {
                EngGrade = engGrade,
                LearnerFAMs = new[]
                {
                    new TestLearnerFAM()
                }
            };
            return learner;
        }
    }
}
