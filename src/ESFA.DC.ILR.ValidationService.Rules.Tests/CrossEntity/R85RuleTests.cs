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
        public void ConditionMet_True()
        {
            var learnRefNumber = "Learner3";
            var uln = 9999999999;
            var learnerDP1 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999999 };
            var learnerDP2 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999998 };
            var learnerDP3 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 };
            var learnerDP4 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 };

            IEnumerable<ILearnerDestinationAndProgression> learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                learnerDP1,
                learnerDP2,
                learnerDP3,
                learnerDP4
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            fileDataServiceMock.Setup(dc => dc.LearnerDestinationAndProgressionsForLearnRefNumber(learnRefNumber)).Returns(learnerDP3);
            learnerDPQueryServiceMock.Setup(qs => qs.HasULNForLearnRefNumber(learnRefNumber, uln, It.IsAny<ILearnerDestinationAndProgression>())).Returns(false);

            NewRule(fileDataServiceMock.Object, learnerDPQueryServiceMock.Object).ConditionMet(learnRefNumber, uln).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learnRefNumber = "Learner1";
            var uln = 9999999999;
            var learnerDP1 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999999 };
            var learnerDP2 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999998 };
            var learnerDP3 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 };
            var learnerDP4 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 };

            IEnumerable<ILearnerDestinationAndProgression> learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                learnerDP1,
                learnerDP2,
                learnerDP3,
                learnerDP4
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            fileDataServiceMock.Setup(dc => dc.LearnerDestinationAndProgressionsForLearnRefNumber(learnRefNumber)).Returns(learnerDP1);
            learnerDPQueryServiceMock.Setup(qs => qs.HasULNForLearnRefNumber(learnRefNumber, uln, It.IsAny<ILearnerDestinationAndProgression>())).Returns(true);

            NewRule(fileDataServiceMock.Object, learnerDPQueryServiceMock.Object).ConditionMet(learnRefNumber, uln).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_MisMatch()
        {
            var learnRefNumber = "Learner1";
            var uln = 9999999999;
            var learnerDP3 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 };
            var learnerDP4 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 };

            IEnumerable<ILearnerDestinationAndProgression> learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                learnerDP3,
                learnerDP4
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            fileDataServiceMock.Setup(dc => dc.LearnerDestinationAndProgressionsForLearnRefNumber(learnRefNumber)).Returns((ILearnerDestinationAndProgression)null);
            learnerDPQueryServiceMock.Setup(qs => qs.HasULNForLearnRefNumber(learnRefNumber, uln, It.IsAny<ILearnerDestinationAndProgression>())).Returns(false);

            NewRule(fileDataServiceMock.Object, learnerDPQueryServiceMock.Object).ConditionMet(learnRefNumber, uln).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NullDP()
        {
            var learnRefNumber = "Learner1";
            var uln = 9999999999;
            TestLearnerDestinationAndProgression learnerDP = null;

            var fileDataServiceMock = new Mock<IFileDataService>();
            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            learnerDPQueryServiceMock.Setup(qs => qs.HasULNForLearnRefNumber(learnRefNumber, uln, learnerDP)).Returns(false);

            NewRule(fileDataServiceMock.Object, learnerDPQueryServiceMock.Object).ConditionMet(learnRefNumber, uln).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnRefNumber = "Learner3";
            var uln = 9999999999;
            var learnerDP1 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999999 };
            var learnerDP2 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999998 };
            var learnerDP3 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 };
            var learnerDP4 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 };

            IEnumerable<ILearnerDestinationAndProgression> learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                learnerDP1,
                learnerDP2,
                learnerDP3,
                learnerDP4
            };

            var learner = new TestLearner
            {
                LearnRefNumber = learnRefNumber,
                ULN = uln
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            fileDataServiceMock.Setup(dc => dc.LearnerDestinationAndProgressionsForLearnRefNumber(learnRefNumber)).Returns(learnerDP3);
            learnerDPQueryServiceMock.Setup(qs => qs.HasULNForLearnRefNumber(learnRefNumber, uln, It.IsAny<ILearnerDestinationAndProgression>())).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(fileDataServiceMock.Object, learnerDPQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnRefNumber = "Learner3";
            var uln = 9999999999;
            var learnerDP1 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999999 };
            var learnerDP2 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999998 };
            var learnerDP3 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999999 };
            var learnerDlearnerDP3P4 = new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 };

            IEnumerable<ILearnerDestinationAndProgression> learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                learnerDP1,
                learnerDP2,
                learnerDP3
            };

            var learner = new TestLearner
            {
                LearnRefNumber = learnRefNumber,
                ULN = uln
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            fileDataServiceMock.Setup(dc => dc.LearnerDestinationAndProgressionsForLearnRefNumber(learnRefNumber)).Returns(learnerDP3);
            learnerDPQueryServiceMock.Setup(qs => qs.HasULNForLearnRefNumber(learnRefNumber, uln, It.IsAny<ILearnerDestinationAndProgression>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(fileDataServiceMock.Object, learnerDPQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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

        private R85Rule NewRule(IFileDataService fileDataService = null, ILearnerDPQueryService learnerDPQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R85Rule(fileDataService, learnerDPQueryService, validationErrorHandler);
        }
    }
}
