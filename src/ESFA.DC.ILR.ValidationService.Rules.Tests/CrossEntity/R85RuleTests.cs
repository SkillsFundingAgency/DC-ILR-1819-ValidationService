using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
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
        public void ConditionMet_True()
        {
            var learnRefNumber = "Learner3";
            var uln = 9999999999;
            IEnumerable<ILearnerDestinationAndProgression> learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999999 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999998 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 },
            };

            var fileDataCacheMock = new Mock<IFileDataCache>();
            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            fileDataCacheMock.Setup(dc => dc.LearnerDestinationAndProgressions).Returns(learnerDPs);
            learnerDPQueryServiceMock.Setup(qs => qs.HasULNForLearnRefNumber(learnRefNumber, uln, It.IsAny<ILearnerDestinationAndProgression>())).Returns(false);

            NewRule(fileDataCacheMock.Object, learnerDPQueryServiceMock.Object).ConditionMet(learnRefNumber, uln).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learnRefNumber = "Learner1";
            var uln = 9999999999;
            IEnumerable<ILearnerDestinationAndProgression> learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999999 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner2", ULN = 9999999998 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 },
            };

            var fileDataCacheMock = new Mock<IFileDataCache>();
            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            fileDataCacheMock.Setup(dc => dc.LearnerDestinationAndProgressions).Returns(learnerDPs);
            learnerDPQueryServiceMock.Setup(qs => qs.HasULNForLearnRefNumber(learnRefNumber, uln, It.IsAny<ILearnerDestinationAndProgression>())).Returns(true);

            NewRule(fileDataCacheMock.Object, learnerDPQueryServiceMock.Object).ConditionMet(learnRefNumber, uln).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_MisMatch()
        {
            var learnRefNumber = "Learner1";
            var uln = 9999999999;
            IEnumerable<ILearnerDestinationAndProgression> learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner2", ULN = 9999999998 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 },
            };

            var fileDataCacheMock = new Mock<IFileDataCache>();
            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            fileDataCacheMock.Setup(dc => dc.LearnerDestinationAndProgressions).Returns(learnerDPs);
            learnerDPQueryServiceMock.Setup(qs => qs.HasULNForLearnRefNumber(learnRefNumber, uln, It.IsAny<ILearnerDestinationAndProgression>())).Returns(false);

            NewRule(fileDataCacheMock.Object, learnerDPQueryServiceMock.Object).ConditionMet(learnRefNumber, uln).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NullDP()
        {
            var learnRefNumber = "Learner1";
            var uln = 9999999999;
            TestLearnerDestinationAndProgression learnerDPs = null;

            var fileDataCacheMock = new Mock<IFileDataCache>();
            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            learnerDPQueryServiceMock.Setup(qs => qs.HasULNForLearnRefNumber(learnRefNumber, uln, learnerDPs)).Returns(false);

            NewRule(fileDataCacheMock.Object, learnerDPQueryServiceMock.Object).ConditionMet(learnRefNumber, uln).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnRefNumber = "Learner3";
            var uln = 9999999999;
            IEnumerable<ILearnerDestinationAndProgression> learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999999 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999998 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 },
            };

            var learner = new TestLearner
            {
                LearnRefNumber = learnRefNumber,
                ULN = uln
            };

            var fileDataCacheMock = new Mock<IFileDataCache>();
            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            fileDataCacheMock.Setup(dc => dc.LearnerDestinationAndProgressions).Returns(learnerDPs);
            learnerDPQueryServiceMock.Setup(qs => qs.HasULNForLearnRefNumber(learnRefNumber, uln, It.IsAny<ILearnerDestinationAndProgression>())).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(fileDataCacheMock.Object, learnerDPQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnRefNumber = "Learner3";
            var uln = 9999999999;
            IEnumerable<ILearnerDestinationAndProgression> learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999999 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999998 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 },
            };

            var learner = new TestLearner
            {
                LearnRefNumber = learnRefNumber,
                ULN = uln
            };

            var fileDataCacheMock = new Mock<IFileDataCache>();
            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            fileDataCacheMock.Setup(dc => dc.LearnerDestinationAndProgressions).Returns(learnerDPs);
            learnerDPQueryServiceMock.Setup(qs => qs.HasULNForLearnRefNumber(learnRefNumber, uln, It.IsAny<ILearnerDestinationAndProgression>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(fileDataCacheMock.Object, learnerDPQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ULN", 9999999999)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(9999999999);

            validationErrorHandlerMock.Verify();
        }

        private R85Rule NewRule(IFileDataCache fileDataCache = null, ILearnerDPQueryService learnerDPQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R85Rule(fileDataCache, learnerDPQueryService, validationErrorHandler);
        }
    }
}
