using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearnerEmploymentStatusMonitoringQueryServiceTests
    {
        [Fact]
        public void HasAnyESMTypeAndCodeForLES_True()
        {
            var learnerEmploymentStatuses = SetupLearnerEmploymentStatuses();

            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "EII", 2).Should().BeTrue();
        }

        [Fact]
        public void HasAnyESMCodesForType_False_NullLearnerEmploymentStatuses()
        {
            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(null, "EII", 2).Should().BeFalse();
        }

        [Fact]
        public void HasAnyESMTypeAndCodeForLES_False_EsmTypeNull()
        {
            var learnerEmploymentStatuses = SetupLearnerEmploymentStatuses();

            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, null, 2).Should().BeFalse();
        }

        [Fact]
        public void HasAnyESMTypeAndCodeForLES_False_EsmTypeMismatch()
        {
            var learnerEmploymentStatuses = SetupLearnerEmploymentStatuses();

            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "XX", 2).Should().BeFalse();
        }

        [Fact]
        public void HasAnyESMTypeAndCodeForLES_False_EsmCodeMismatch()
        {
            var learnerEmploymentStatuses = SetupLearnerEmploymentStatuses();

            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "SEI", 0).Should().BeFalse();
        }

        private ILearnerEmploymentStatus[] SetupLearnerEmploymentStatuses()
        {
            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring() { ESMType = "SEI", ESMCode = 1 },
                        new TestEmploymentStatusMonitoring() { ESMType = "EII", ESMCode = 2 },
                        new TestEmploymentStatusMonitoring() { ESMType = "LOU", ESMCode = 3 },
                        new TestEmploymentStatusMonitoring() { ESMType = "LOE", ESMCode = 4 },
                    }
                }
            };

            return learnerEmploymentStatuses;
        }

        private LearnerEmploymentStatusMonitoringQueryService NewService()
        {
            return new LearnerEmploymentStatusMonitoringQueryService();
        }
    }
}
