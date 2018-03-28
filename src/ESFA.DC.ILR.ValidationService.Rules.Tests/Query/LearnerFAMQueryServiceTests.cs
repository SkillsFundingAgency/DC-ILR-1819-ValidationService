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

            NewService().HasAnyLearnerFAMCodesForType(learnerFams, "FamC", new[] { 1, 3 }).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearnerFAMCodesForType_NullFams()
        {
            NewService().HasAnyLearnerFAMCodesForType(null, "FamB", It.IsAny<List<int>>()).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearnerFAMCodesForType_False_CodesNull()
        {
            var learnerFams = SetupLearnerFams();

            NewService().HasAnyLearnerFAMCodesForType(learnerFams, "FamB", null).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearnerFAMCodesForType_False_Mismatch()
        {
            var learnerFams = SetupLearnerFams();

            NewService().HasAnyLearnerFAMCodesForType(learnerFams, "FamA", new[] { 2, 3 }).Should().BeFalse();
        }

        [Fact]
        public void HasLearnerFAMCodeForType_True()
        {
            var learnerFams = SetupLearnerFams();

            NewService().HasLearnerFAMCodeForType(learnerFams, "FamB", 2).Should().BeTrue();
        }

        [Fact]
        public void HasLearnerFAMCodeForType_NullLearnerFams()
        {
            NewService().HasLearnerFAMCodeForType(null, "FamB", 2).Should().BeFalse();
        }

        [Fact]
        public void HasLearnerFAMCodeForType_FalseMismatch()
        {
            var learnerFams = SetupLearnerFams();

            NewService().HasLearnerFAMCodeForType(learnerFams, "FamA", 99999).Should().BeFalse();
        }

        [Fact]
        public void HasLearnerFAMType_True()
        {
            var learnerFams = SetupLearnerFams();

            NewService().HasLearnerFAMType(learnerFams, "FamA").Should().BeTrue();
        }

        [Fact]
        public void HasLearnerFAMType_False()
        {
            var learnerFams = SetupLearnerFams();

            NewService().HasLearnerFAMType(learnerFams, "TYPENOTFOUND").Should().BeFalse();
        }

        [Fact]
        public void HasLearnerFAMType_False_NullLearnerFams()
        {
            NewService().HasLearnerFAMType(null, It.IsAny<string>()).Should().BeFalse();
        }

        [Fact]
        public void HasLearnerFAMTypes_True()
        {
            var learnerFams = SetupLearnerFams();

            NewService().HasAnyLearnerFAMTypes(learnerFams, new List<string>() { "FamA", "TYPENOTFOUND" }).Should().BeTrue();
        }

        [Fact]
        public void HasLearnerFAMTypes_False()
        {
            var learnerFams = SetupLearnerFams();

            NewService().HasAnyLearnerFAMTypes(learnerFams, new List<string>() { "XXXX", "TYPENOTFOUND" }).Should().BeFalse();
        }

        [Fact]
        public void HasLearnerFAMTypes_Null()
        {
            var learnerFams = SetupLearnerFams();

            NewService().HasAnyLearnerFAMTypes(learnerFams, null).Should().BeFalse();
        }

        private ILearnerFAM[] SetupLearnerFams()
        {
            var learnerFams = new TestLearnerFAM[]
            {
                new TestLearnerFAM() { LearnFAMType = "FamA", LearnFAMCode = 1 },
                new TestLearnerFAM() { LearnFAMType = "FamB", LearnFAMCode = 2 },
                new TestLearnerFAM() { LearnFAMType = "FamC", LearnFAMCode = 3 },
                new TestLearnerFAM() { LearnFAMType = "FamC", LearnFAMCode = 5 },
            };

            return learnerFams;
        }

        private LearnerFAMQueryService NewService()
        {
            return new LearnerFAMQueryService();
        }
    }
}
