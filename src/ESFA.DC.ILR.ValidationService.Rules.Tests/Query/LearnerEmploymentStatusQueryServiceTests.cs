using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearnerEmploymentStatusQueryServiceTests
    {
        [Fact]
        public void EmpStatForDateEmpStatApp_Zero()
        {
            NewService().LearnerEmploymentStatusForDate(null, new DateTime(2018, 8, 1)).Should().BeNull();
        }

        [Fact]
        public void EmpStatForDateEmpStatApp_Zero_DateTooLate()
        {
            var learnStartDate = new DateTime(2018, 7, 1);

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            NewService().LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate).Should().BeNull();
        }

        [Fact]
        public void EmpStatForDateEmpStatApp()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var matchingLearnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                matchingLearnerEmploymentStatus
            };

            NewService().LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate).Should().Be(matchingLearnerEmploymentStatus);
        }

        [Fact]
        public void EmpStatForDateEmpStatApp_MiddleOne()
        {
            var learnStartDate = new DateTime(2018, 8, 13);

            var earlyLearnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var matchingLearnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 13)
            };

            var laterLearningEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 19)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                earlyLearnerEmploymentStatus,
                matchingLearnerEmploymentStatus,
                laterLearningEmploymentStatus
            };

            NewService().LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate).Should().Be(matchingLearnerEmploymentStatus);
        }

        [Fact]
        public void EmpStatsNotExistBeforeLearnStartDate_True_Null()
        {
            NewService().EmpStatsNotExistBeforeDate(null, new DateTime(2018, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void EmpStatsNotExistBeforeLearnStartDate_True()
        {
            var learnStartDate = new DateTime(2018, 7, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            NewService().EmpStatsNotExistBeforeDate(learnerEmploymentStatuses, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void EmpStatsNotExistBeforeLearnStartDate_False()
        {
            var learnStartDate = new DateTime(2018, 9, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            NewService().EmpStatsNotExistBeforeDate(learnerEmploymentStatuses, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void EmpStatsNotExistOnOrBeforeLearnStartDate_True_Null()
        {
            NewService().EmpStatsNotExistOnOrBeforeDate(null, new DateTime(2018, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void EmpStatsNotExistOnOrBeforeLearnStartDate_True()
        {
            var learnStartDate = new DateTime(2018, 7, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            NewService().EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void EmpStatsNotExistOnOrBeforeLearnStartDate_False()
        {
            var learnStartDate = new DateTime(2018, 9, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            NewService().EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate).Should().BeFalse();
        }

        private LearnerEmploymentStatusQueryService NewService()
        {
            return new LearnerEmploymentStatusQueryService();
        }
    }
}
