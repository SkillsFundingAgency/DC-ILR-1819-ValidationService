using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearnerDPQueryServiceTests
    {
        [Fact]
        public void HasAnyLearnerFAMCodesForType_True()
        {
            var learnerDPs = SetupLearnerDPs();

            NewService().HasULNForLearnRefNumber("Learner1", 9999999999, learnerDPs[0]).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearnerFAMCodesForType_False()
        {
            var learnerDPs = SetupLearnerDPs();

            NewService().HasULNForLearnRefNumber("Learner1", 1000000000, learnerDPs[0]).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearnerFAMCodesForType_False_MisMatch()
        {
            var learnerDPs = SetupLearnerDPs();

            NewService().HasULNForLearnRefNumber("Learner3", 1000000000, learnerDPs[0]).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearnerFAMCodesForType_False_Null()
        {
            NewService().HasULNForLearnRefNumber("Learner1", 9999999998, null).Should().BeFalse();
        }

        [Fact]
        public void OutTypesForStartDate()
        {
            var outTypes = new List<string>
            {
                OutTypeConstants.PaidEmployment,
                OutTypeConstants.GapYear,
                OutTypeConstants.NotInPaidEmployment,
                OutTypeConstants.Other,
                OutTypeConstants.SocialDestination,
                OutTypeConstants.VoluntaryWork
            };

        var dpOutcomes = new List<TestDPOutcome>
            {
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 8, 1),
                    OutType = "GAP"
                },
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 8, 1),
                    OutType = "GAP"
                },
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 9, 1),
                    OutType = "EMP"
                }
            };

            var dictionary = new Dictionary<DateTime, List<string>>
            {
                { new DateTime(2018, 8, 1), new List<string> { "GAP", "GAP" } },
                { new DateTime(2018, 9, 1), new List<string> { "EMP" } }
            };

            NewService().OutTypesForStartDateAndTypes(dpOutcomes, outTypes).Should().BeEquivalentTo(dictionary);
        }

        [Fact]
        public void OutTypesForStartDate_NullDPOutcomes()
        {
            var outTypes = new List<string>
            {
                OutTypeConstants.PaidEmployment,
                OutTypeConstants.GapYear,
                OutTypeConstants.NotInPaidEmployment,
                OutTypeConstants.Other,
                OutTypeConstants.SocialDestination,
                OutTypeConstants.VoluntaryWork
            };

            var dpOutcomes = new List<TestDPOutcome>();

            NewService().OutTypesForStartDateAndTypes(dpOutcomes, outTypes).Should().BeNull();
        }

        [Fact]
        public void OutTypesForStartDate_NullOutTypes()
        {
            var dpOutcomes = new List<TestDPOutcome>
            {
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 8, 1),
                    OutType = "GAP"
                },
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 8, 1),
                    OutType = "GAP"
                },
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 9, 1),
                    OutType = "EMP"
                }
            };

            NewService().OutTypesForStartDateAndTypes(dpOutcomes, null).Should().BeNull();
        }

        [Fact]
        public void OutTypesForStartDate_MisMatch()
        {
            var outTypes = new List<string>
            {
                OutTypeConstants.PaidEmployment,
                OutTypeConstants.GapYear,
                OutTypeConstants.NotInPaidEmployment,
                OutTypeConstants.Other,
                OutTypeConstants.SocialDestination,
                OutTypeConstants.VoluntaryWork
            };

            var dpOutcomes = new List<TestDPOutcome>
            {
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 8, 1),
                    OutType = "EDU"
                }
            };

            NewService().OutTypesForStartDateAndTypes(dpOutcomes, outTypes).Should().BeNull();
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
