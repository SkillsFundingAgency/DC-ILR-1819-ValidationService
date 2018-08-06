using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.MathGrade
{
    public class MathGrade_04RuleTests
    {
        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ConditionMet_True(long famCode)
        {
            var learnerFamQueryService = new LearnerFAMQueryService();
            var learnerFams = new[]
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "MCF",
                    LearnFAMCodeNullable = famCode
                }
            };

            var rule = new MathGrade_04Rule(null, learnerFamQueryService);

            rule.ConditionMet("XYZ", learnerFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NoneGrade()
        {
            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService.Setup(x => x.HasAnyLearnerFAMCodesForType(It.IsAny<IEnumerable<ILearnerFAM>>(), "MCF", new[] { (long)2 }))
                                    .Returns(true);

            var rule = new MathGrade_03Rule(null, learnerFamQueryService.Object);

            rule.ConditionMet("NONE", It.IsAny<IReadOnlyCollection<ILearnerFAM>>()).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FamTypeNotMatching()
        {
            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService.Setup(x => x.HasAnyLearnerFAMCodesForType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>(), It.IsAny<IEnumerable<long>>()))
                .Returns(false);

            var rule = new MathGrade_03Rule(null, learnerFamQueryService.Object);

            rule.ConditionMet("ABCC", It.IsAny<IReadOnlyCollection<ILearnerFAM>>()).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner("XYZ");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("MathGrade_04", null, null, null);

            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService.Setup(x => x.HasAnyLearnerFAMCodesForType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>(), It.IsAny<IEnumerable<long>>()))
                .Returns(true);

            var rule = new MathGrade_04Rule(validationErrorHandlerMock.Object, learnerFamQueryService.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner("NONE");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("MathGrade_03", null, null, null);

            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService.Setup(x => x.HasAnyLearnerFAMCodesForType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>(), It.IsAny<IEnumerable<long>>()))
                .Returns(true);

            var rule = new MathGrade_04Rule(validationErrorHandlerMock.Object, learnerFamQueryService.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private static TestLearner SetupLearner(string mathGrade)
        {
            var learner = new TestLearner()
            {
                MathGrade = mathGrade,
                LearnerFAMs = new[]
                {
                    new TestLearnerFAM()
                }
            };
            return learner;
        }
    }
}
