using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearnerDPQueryServiceTests
    {
        [Fact]
        public void HasAnyLearnerFAMCodesForType_True()
        {
            var learnerDPs = SetupLearnerDPs();

            NewService().HasULNForLearnRefNumber("Learner1", 9999999999, learnerDPs).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearnerFAMCodesForType_False()
        {
            var learnerDPs = SetupLearnerDPs();

            NewService().HasULNForLearnRefNumber("Learner1", 1000000000, learnerDPs).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearnerFAMCodesForType_False_Null()
        {
            NewService().HasULNForLearnRefNumber("Learner1", 9999999998, null).Should().BeFalse();
        }

        private ILearnerDestinationAndProgression[] SetupLearnerDPs()
        {
            var learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999999 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999998 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 },
            };

            return learnerDPs;
        }

        private LearnerDPQueryService NewService()
        {
            return new LearnerDPQueryService();
        }
    }
}
