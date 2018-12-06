using System.Linq;
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

        [Theory]
        [InlineData("SEI")]
        [InlineData("EII")]
        [InlineData("LOU")]
        [InlineData("LOE")]
        [InlineData("BSI")]
        [InlineData("PEI")]
        [InlineData("SEM")]
        public void HasAnyEmploymentStatusMonitoringTypeMoreThanOnce_True(string duplicateType)
        {
            var esmTypes = new[] { "SEI", "EII", "LOU", "LOE", "BSI", "PEI", "SEM" };

            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
            {
                new TestEmploymentStatusMonitoring() { ESMType = "SEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "EII" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOU" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOE" },
                new TestEmploymentStatusMonitoring() { ESMType = "BSI" },
                new TestEmploymentStatusMonitoring() { ESMType = "PEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "SEM" },
                new TestEmploymentStatusMonitoring() { ESMType = duplicateType }
            };

            NewService().HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(employmentStatusMonitorings, esmTypes).Should().BeTrue();
        }

        [Fact]
        public void HasAnyEmploymentStatusMonitoringTypeMoreThanOnce_False()
        {
            var esmTypes = new[] { "SEI", "EII", "LOU", "LOE", "BSI", "PEI", "SEM" };

            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
            {
                new TestEmploymentStatusMonitoring() { ESMType = "SEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "EII" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOU" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOE" },
                new TestEmploymentStatusMonitoring() { ESMType = "BSI" },
                new TestEmploymentStatusMonitoring() { ESMType = "PEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "SEM" }
            };

            NewService().HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(employmentStatusMonitorings, esmTypes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyEmploymentStatusMonitoringTypeMoreThanOnce_FalseNoTypeMatch()
        {
            var esmTypes = new[] { "SEI", "EII", "LOU", "LOE", "BSI", "PEI", "SEM" };

            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
            {
                new TestEmploymentStatusMonitoring() { ESMType = "XXX" },
                new TestEmploymentStatusMonitoring() { ESMType = "XXX" }
            };

            NewService().HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(employmentStatusMonitorings, esmTypes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyEmploymentStatusMonitoringTypeMoreThanOnce_FalseNull()
        {
            var esmTypes = new[] { "SEI", "EII", "LOU", "LOE", "BSI", "PEI", "SEM" };

            NewService().HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(null, esmTypes).Should().BeFalse();
        }

        [Fact]
        public void GetDuplicatedEmploymentStatusMonitoringTypesForTypes_OneMatch()
        {
            var esmTypes = new[] { "SEI", "EII", "LOU", "LOE", "BSI", "PEI", "SEM" };

            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
            {
                new TestEmploymentStatusMonitoring() { ESMType = "SEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "EII" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOU" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOE" },
                new TestEmploymentStatusMonitoring() { ESMType = "BSI" },
                new TestEmploymentStatusMonitoring() { ESMType = "PEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "SEM" },
                new TestEmploymentStatusMonitoring() { ESMType = "SEM" },
            };

            NewService().GetDuplicatedEmploymentStatusMonitoringTypesForTypes(employmentStatusMonitorings, esmTypes).Count().Should().Be(1);
        }

        [Fact]
        public void GetDuplicatedEmploymentStatusMonitoringTypesForTypes_MultipleMatches()
        {
            var esmTypes = new[] { "SEI", "EII", "LOU", "LOE", "BSI", "PEI", "SEM" };

            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
            {
                new TestEmploymentStatusMonitoring() { ESMType = "SEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "EII" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOU" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOE" },
                new TestEmploymentStatusMonitoring() { ESMType = "BSI" },
                new TestEmploymentStatusMonitoring() { ESMType = "BSI" },
                new TestEmploymentStatusMonitoring() { ESMType = "PEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "PEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "SEM" },
                new TestEmploymentStatusMonitoring() { ESMType = "SEM" },
            };

            NewService().GetDuplicatedEmploymentStatusMonitoringTypesForTypes(employmentStatusMonitorings, esmTypes).Count().Should().Be(3);
        }

        [Fact]
        public void GetDuplicatedEmploymentStatusMonitoringTypesForTypes_NoMatch()
        {
            var esmTypes = new[] { "SEI", "EII", "LOU", "LOE", "BSI", "PEI", "SEM" };

            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
            {
                new TestEmploymentStatusMonitoring() { ESMType = "SEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "EII" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOU" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOE" },
                new TestEmploymentStatusMonitoring() { ESMType = "BSI" },
                new TestEmploymentStatusMonitoring() { ESMType = "PEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "SEM" },
            };

            NewService().GetDuplicatedEmploymentStatusMonitoringTypesForTypes(employmentStatusMonitorings, esmTypes).Count().Should().Be(0);
        }

        [Fact]
        public void GetDuplicatedEmploymentStatusMonitoringTypesForTypes_Null()
        {
            var esmTypes = new[] { "SEI", "EII", "LOU", "LOE", "BSI", "PEI", "SEM" };

            NewService().GetDuplicatedEmploymentStatusMonitoringTypesForTypes(null, esmTypes).Should().BeNull();
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
