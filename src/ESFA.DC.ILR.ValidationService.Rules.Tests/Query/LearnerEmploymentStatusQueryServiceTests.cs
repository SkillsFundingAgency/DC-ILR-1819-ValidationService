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
            NewService().EmpStatForDateEmpStatApp(null, new DateTime(2018, 8, 1)).Should().Be(0);
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

            NewService().EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate).Should().Be(0);
        }

        [Fact]
        public void EmpStatForDateEmpStatApp()
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

            NewService().EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate).Should().Be(10);
        }

        [Fact]
        public void EmpStatForDateEmpStatApp_MiddleOne()
        {
            var learnStartDate = new DateTime(2018, 8, 13);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                },
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 8, 13)
                },
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 19)
                }
            };

            NewService().EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate).Should().Be(11);
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
