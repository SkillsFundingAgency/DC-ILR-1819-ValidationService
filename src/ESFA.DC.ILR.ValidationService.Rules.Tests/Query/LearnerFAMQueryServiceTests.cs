using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
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

            NewService().HasLearnerFAMCodeForType(learnerFams, "FAmB", 2).Should().BeTrue();
        }

        [Fact]
        public void HasLearnerFAMCodeForType_NullLearnerFams()
        {
            NewService().HasLearnerFAMCodeForType(null, "FaMB", 2).Should().BeFalse();
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

            NewService().HasLearnerFAMType(learnerFams, "FAmA").Should().BeTrue();
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

        [Fact]
        public void GetLearningFAMsCountByFAMType_CountCheckForFound()
        {
            var testLearningFAMs = new TestLearnerFAM[]
                {
                    new TestLearnerFAM
                    {
                        LearnFAMType = LearnerFAMTypeConstants.EHC
                    },
                    new TestLearnerFAM
                    {
                        LearnFAMType = LearnerFAMTypeConstants.EHC
                    },
                    new TestLearnerFAM
                    {
                        LearnFAMType = LearnerFAMTypeConstants.DLA
                    },
                };

            NewService().GetLearnerFAMsCountByFAMType(testLearningFAMs, LearnerFAMTypeConstants.EHC).Should().Be(2);
        }

        [Fact]
        public void GetLearningDeliveryFAMsCountByFAMType_CountCheckForNotFound()
        {
            var testLearningFAMs = new TestLearnerFAM[]
            {
                new TestLearnerFAM
                {
                    LearnFAMType = LearnerFAMTypeConstants.EHC
                },
                new TestLearnerFAM
                {
                    LearnFAMType = LearnerFAMTypeConstants.HNS
                },
                new TestLearnerFAM
                {
                    LearnFAMType = LearnerFAMTypeConstants.DLA
                },
            };

            NewService().GetLearnerFAMsCountByFAMType(testLearningFAMs, LearnerFAMTypeConstants.FME).Should().Be(0);
        }

        [Fact]
        public void GetLearningDeliveryFAMsCountByFAMType_NullCheck()
        {
            NewService().GetLearnerFAMsCountByFAMType(null, LearnerFAMTypeConstants.FME).Should().Be(0);
        }

        private ILearnerFAM[] SetupLearnerFams()
        {
            var learnerFams = new TestLearnerFAM[]
            {
                new TestLearnerFAM() { LearnFAMType = "famA", LearnFAMCode = 1 },
                new TestLearnerFAM() { LearnFAMType = "FAmB", LearnFAMCode = 2 },
                new TestLearnerFAM() { LearnFAMType = "FaMC", LearnFAMCode = 3 },
                new TestLearnerFAM() { LearnFAMType = "FAmc", LearnFAMCode = 5 },
            };

            return learnerFams;
        }

        private LearnerFAMQueryService NewService()
        {
            return new LearnerFAMQueryService();
        }
    }
}
