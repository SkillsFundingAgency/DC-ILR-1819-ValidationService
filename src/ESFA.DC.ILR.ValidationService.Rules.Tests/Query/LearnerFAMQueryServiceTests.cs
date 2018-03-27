using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearnerFAMQueryServiceTests
    {
        [Fact]
        public void HasAnyLearnerFAMCodesForType_True()
        {
            var learnerFams = SetupLearnerFams();

            var codes = new long[] { 1, 3 };
            var queryService = new LearnerFAMQueryService();
            queryService.HasAnyLearnerFAMCodesForType(learnerFams, "FamC", codes).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearnerFAMCodesForType_NullFams()
        {
            var queryService = new LearnerFAMQueryService();
            queryService.HasAnyLearnerFAMCodesForType(null, "FamB", It.IsAny<List<long>>()).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearnerFAMCodesForType_False_CodesNull()
        {
            var learnerFams = SetupLearnerFams();
            var queryService = new LearnerFAMQueryService();

            queryService.HasAnyLearnerFAMCodesForType(learnerFams, "FamB", null).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearnerFAMCodesForType_False_Mismatch()
        {
            var learnerFams = SetupLearnerFams();

            var queryService = new LearnerFAMQueryService();
            queryService.HasAnyLearnerFAMCodesForType(learnerFams, "FamA", new long[] { 2, 3 }).Should().BeFalse();
        }

        [Fact]
        public void HasLearnerFAMCodeForType_True()
        {
            var learnerFams = SetupLearnerFams();

            var queryService = new LearnerFAMQueryService();
            queryService.HasLearnerFAMCodeForType(learnerFams, "FamB", 2).Should().BeTrue();
        }

        [Fact]
        public void HasLearnerFAMCodeForType_NullLearnerFams()
        {
            var queryService = new LearnerFAMQueryService();
            queryService.HasLearnerFAMCodeForType(null, "FamB", 2).Should().BeFalse();
        }

        [Fact]
        public void HasLearnerFAMCodeForType_FalseMismatch()
        {
            var learnerFams = SetupLearnerFams();
            var queryService = new LearnerFAMQueryService();
            queryService.HasLearnerFAMCodeForType(learnerFams, "FamA", 99999).Should().BeFalse();
        }

        [Fact]
        public void HasLearnerFAMType_True()
        {
            var learnerFams = SetupLearnerFams();
            var queryService = new LearnerFAMQueryService();
            queryService.HasLearnerFAMType(learnerFams, "FamA").Should().BeTrue();
        }

        [Fact]
        public void HasLearnerFAMType_False()
        {
            var learnerFams = SetupLearnerFams();
            var queryService = new LearnerFAMQueryService();
            queryService.HasLearnerFAMType(learnerFams, "TYPENOTFOUND").Should().BeFalse();
        }

        [Fact]
        public void HasLearnerFAMType_False_NullLearnerFams()
        {
            var queryService = new LearnerFAMQueryService();
            queryService.HasLearnerFAMType(null, It.IsAny<string>()).Should().BeFalse();
        }

        [Fact]
        public void HasLearnerFAMTypes_True()
        {
            var learnerFams = SetupLearnerFams();
            var queryService = new LearnerFAMQueryService();
            queryService.HasAnyLearnerFAMTypes(learnerFams, new List<string>() { "FamA", "TYPENOTFOUND" }).Should().BeTrue();
        }

        [Fact]
        public void HasLearnerFAMTypes_False()
        {
            var learnerFams = SetupLearnerFams();
            var queryService = new LearnerFAMQueryService();
            queryService.HasAnyLearnerFAMTypes(learnerFams, new List<string>() { "XXXX", "TYPENOTFOUND" }).Should().BeFalse();
        }

        [Fact]
        public void HasLearnerFAMTypes_Null()
        {
            var learnerFams = SetupLearnerFams();
            var queryService = new LearnerFAMQueryService();
            queryService.HasAnyLearnerFAMTypes(learnerFams, null).Should().BeFalse();
        }

        private ILearnerFAM[] SetupLearnerFams()
        {
            var learnerFams = new TestLearnerFAM[]
            {
                new TestLearnerFAM() { LearnFAMType = "FamA", LearnFAMCodeNullable = 1 },
                new TestLearnerFAM() { LearnFAMType = "FamB", LearnFAMCodeNullable = 2 },
                new TestLearnerFAM() { LearnFAMType = "FamC", LearnFAMCodeNullable = 3 },
                new TestLearnerFAM() { LearnFAMType = "FamC", LearnFAMCodeNullable = 5 },
            };

            return learnerFams;
        }
    }
}
