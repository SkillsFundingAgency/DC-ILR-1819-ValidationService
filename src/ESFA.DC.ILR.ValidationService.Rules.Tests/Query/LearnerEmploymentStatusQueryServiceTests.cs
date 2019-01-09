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
        public void EmpStatsForDateEmpStatApp_Null()
        {
            NewService().EmpStatsForDateEmpStatApp(null, new DateTime(2018, 8, 1)).Should().BeNull();
        }

        [Fact]
        public void EmpStatsForDateEmpStatApp_Null_DateMisMatch()
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

            NewService().EmpStatsForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate).Should().BeNullOrEmpty();
        }

        [Fact]
        public void EmpStatsForDateEmpStatApp()
        {
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            NewService().EmpStatsForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate).Should().BeEquivalentTo(new List<int> { 10 });
        }

        [Fact]
        public void EmpStatsNotExistBeforeLearnStartDate_True_Null()
        {
            NewService().EmpStatsNotExistBeforeLearnStartDate(null, new DateTime(2018, 8, 1)).Should().BeTrue();
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

            NewService().EmpStatsNotExistBeforeLearnStartDate(learnerEmploymentStatuses, learnStartDate).Should().BeTrue();
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

            NewService().EmpStatsNotExistBeforeLearnStartDate(learnerEmploymentStatuses, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void EmpStatsNotExistOnOrBeforeLearnStartDate_True_Null()
        {
            NewService().EmpStatsNotExistOnOrBeforeLearnStartDate(null, new DateTime(2018, 8, 1)).Should().BeTrue();
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

            NewService().EmpStatsNotExistOnOrBeforeLearnStartDate(learnerEmploymentStatuses, learnStartDate).Should().BeTrue();
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

            NewService().EmpStatsNotExistOnOrBeforeLearnStartDate(learnerEmploymentStatuses, learnStartDate).Should().BeFalse();
        }

        private LearnerEmploymentStatusQueryService NewService()
        {
            return new LearnerEmploymentStatusQueryService();
        }
    }
}
