using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.EngGrade
{
    public class EngGrade_04RuleTests
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
                    LearnFAMType = "ECF",
                    LearnFAMCodeNullable = famCode
                }
            };

            var rule = new EngGrade_04Rule(null, learnerFamQueryService);

            rule.ConditionMet("XYZ", learnerFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NoneGrade()
        {
            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService.Setup(x => x.HasAnyLearnerFAMCodesForType(It.IsAny<IEnumerable<ILearnerFAM>>(), "ECF", new[] { (long)2 }))
                                    .Returns(true);

            var rule = new EngGrade_04Rule(null, learnerFamQueryService.Object);

            rule.ConditionMet("NONE", It.IsAny<IReadOnlyCollection<ILearnerFAM>>()).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FamTypeNotMatching()
        {
            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService.Setup(x => x.HasAnyLearnerFAMCodesForType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>(), It.IsAny<IEnumerable<long>>()))
                .Returns(false);

            var rule = new EngGrade_04Rule(null, learnerFamQueryService.Object);

            rule.ConditionMet("ABCC", It.IsAny<IReadOnlyCollection<ILearnerFAM>>()).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner("XYZ");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("EngGrade_04", null, null, null);

            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService.Setup(x => x.HasAnyLearnerFAMCodesForType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>(), It.IsAny<IEnumerable<long>>()))
                .Returns(true);

            var rule = new EngGrade_04Rule(validationErrorHandlerMock.Object, learnerFamQueryService.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner("NONE");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("EngGrade_04", null, null, null);

            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService.Setup(x => x.HasAnyLearnerFAMCodesForType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>(), It.IsAny<IEnumerable<long>>()))
                .Returns(true);

            var rule = new EngGrade_04Rule(validationErrorHandlerMock.Object, learnerFamQueryService.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private static TestLearner SetupLearner(string engGrade)
        {
            var learner = new TestLearner()
            {
                EngGrade = engGrade,
                LearnerFAMs = new TestLearnerFAM[]
                {
                    new TestLearnerFAM()
                }
            };
            return learner;
        }
    }
}
