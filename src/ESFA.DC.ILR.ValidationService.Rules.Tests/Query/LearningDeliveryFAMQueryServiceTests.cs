using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearningDeliveryFAMQueryServiceTests
    {
        [Fact]
        public void HasAnyLearningDeliveryFAMCodesForType_True()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne", LearnDelFAMCode = "CodeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeTwo" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeThree" },
            };

            var codes = new string[] { "CodeOne", "CodeThree" };

            var queryService = new LearningDeliveryFAMQueryService();

            queryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "TypeTwo", codes).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearningDeliveryFAMCodesForType_FalseNull()
        {
            var codes = new string[] { "CodeOne", "CodeThree" };

            var queryService = new LearningDeliveryFAMQueryService();

            queryService.HasAnyLearningDeliveryFAMCodesForType(null, "TypeTwo", codes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryFAMCodesForType_False_CodesNull()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne", LearnDelFAMCode = "CodeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeTwo" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeThree" },
            };

            var queryService = new LearningDeliveryFAMQueryService();

            queryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "TypeTwo", null).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryFAMCodesForType_False_Mismatch()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne", LearnDelFAMCode = "CodeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeTwo" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeThree" },
            };

            var codes = new string[] { "CodeTwo", "CodeThree" };

            var queryService = new LearningDeliveryFAMQueryService();

            queryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "TypeOne", codes).Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_True()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne", LearnDelFAMCode = "CodeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeTwo" },
            };

            var queryService = new LearningDeliveryFAMQueryService();

            queryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "TypeTwo", "CodeTwo").Should().BeTrue();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_FalseNull()
        {
            var queryService = new LearningDeliveryFAMQueryService();

            queryService.HasLearningDeliveryFAMCodeForType(null, "TypeTwo", "CodeTwo").Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_FalseMismatch()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne", LearnDelFAMCode = "CodeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeTwo" },
            };

            var queryService = new LearningDeliveryFAMQueryService();

            queryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "TypeOne", "CodeThree").Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMType_True()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo" }
            };

            var queryService = new LearningDeliveryFAMQueryService();

            queryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, "TypeOne");
        }

        [Fact]
        public void HasLearningDeliveryFAMType_False()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo" }
            };

            var queryService = new LearningDeliveryFAMQueryService();

            queryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, "TypeThree");
        }

        [Fact]
        public void HasLearningDeliveryFAMType_False_Null()
        {
            var queryService = new LearningDeliveryFAMQueryService();

            queryService.HasLearningDeliveryFAMType(null, "Doesn't Matter");
        }
    }
}
