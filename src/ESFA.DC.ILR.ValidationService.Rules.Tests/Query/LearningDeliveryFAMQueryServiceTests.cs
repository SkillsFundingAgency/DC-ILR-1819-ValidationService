using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
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

            NewService().HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "TypeTwo", codes).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearningDeliveryFAMCodesForType_FalseNull()
        {
            var codes = new string[] { "CodeOne", "CodeThree" };

            NewService().HasAnyLearningDeliveryFAMCodesForType(null, "TypeTwo", codes).Should().BeFalse();
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

            NewService().HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "TypeTwo", null).Should().BeFalse();
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

            NewService().HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "TypeOne", codes).Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_True()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne", LearnDelFAMCode = "CodeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeTwo" },
            };

            NewService().HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "TypeTwo", "CodeTwo").Should().BeTrue();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_FalseNull()
        {
            NewService().HasLearningDeliveryFAMCodeForType(null, "TypeTwo", "CodeTwo").Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_FalseMismatch()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne", LearnDelFAMCode = "CodeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeTwo" },
            };

            NewService().HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "TypeOne", "CodeThree").Should().BeFalse();
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

            NewService().HasLearningDeliveryFAMType(learningDeliveryFAMs, "TypeOne");
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

            NewService().HasLearningDeliveryFAMType(learningDeliveryFAMs, "TypeThree");
        }

        [Fact]
        public void HasLearningDeliveryFAMType_False_Null()
        {
            NewService().HasLearningDeliveryFAMType(null, "Doesn't Matter");
        }

        [Fact]
        public void HasAnyLearningDeliveryFAMTypes_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "A" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "B" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "C" }
            };

            var famTypes = new List<string>() { "A", "D" };

            NewService().HasAnyLearningDeliveryFAMTypes(learningDeliveryFAMs, famTypes).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearningDeliveryFAMTypes_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "A" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "B" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "C" }
            };

            var famTypes = new List<string>() { "D", "E" };

            NewService().HasAnyLearningDeliveryFAMTypes(learningDeliveryFAMs, famTypes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryFAMTypes_False_NullLearningDeliveryFAMs()
        {
            NewService().HasAnyLearningDeliveryFAMTypes(null, new List<string>());
        }

        [Fact]
        public void HasAnyLearningDeliveryFAMTypes_False_NullFAMTypes()
        {
            NewService().HasAnyLearningDeliveryFAMTypes(new List<ILearningDeliveryFAM>(), null);
        }

        [Fact]
        public void GetLearningDeliveryFAMByTypeAndLatestByDateFrom_ValidReturn()
        {
            var learningDeliveryFAM = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                LearnDelFAMDateToNullable = new DateTime(2018, 06, 01)
            };

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    learningDeliveryFAM,
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL,
                        LearnDelFAMDateFromNullable = new DateTime(2010, 02, 26),
                        LearnDelFAMDateToNullable = new DateTime(2012, 02, 26)
                    }
                };

            NewService().GetLearningDeliveryFAMByTypeAndLatestByDateFrom(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT).Should().Be(learningDeliveryFAM);
        }

        [Fact]
        public void GetLearningDeliveryFAMByTypeAndLatestByDateFrom_NullReturn()
        {
            var learningDeliveryFAM = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF,
                LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                LearnDelFAMDateToNullable = new DateTime(2018, 06, 01)
            };

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    learningDeliveryFAM,
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL,
                        LearnDelFAMDateFromNullable = new DateTime(2010, 02, 26),
                        LearnDelFAMDateToNullable = new DateTime(2012, 02, 26)
                    }
                };

            NewService().GetLearningDeliveryFAMByTypeAndLatestByDateFrom(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT).Should().BeNull();
        }

        [Fact]
        public void GetLearningDeliveryFAMByTypeAndLatestByDateFrom_NullParameterFAMs()
        {
            NewService().GetLearningDeliveryFAMByTypeAndLatestByDateFrom(null, LearningDeliveryFAMTypeConstants.ACT).Should().BeNull();
        }

        private LearningDeliveryFAMQueryService NewService()
        {
            return new LearningDeliveryFAMQueryService();
        }
    }
}
