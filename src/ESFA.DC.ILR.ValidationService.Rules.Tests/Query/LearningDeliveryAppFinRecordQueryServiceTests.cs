using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearningDeliveryAppFinRecordQueryServiceTests
    {
        [Fact]
        public void HasAnyLearningDeliveryAFinCodesForType_True()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
            {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
            };

            var codes = new int[] { 1, 3 };

            NewService().HasAnyLearningDeliveryAFinCodesForType(learningDeliveryAppFinRecords, "TypeTwo", codes).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodesForType_FalseNull()
        {
            var codes = new int[] { 1, 3 };

            NewService().HasAnyLearningDeliveryAFinCodesForType(null, "TypeTwo", codes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodesForType_False_CodesNull()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
           {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
           };

            NewService().HasAnyLearningDeliveryAFinCodesForType(learningDeliveryAppFinRecords, "TypeTwo", null).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodesForType_False_Mismatch()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
           {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
           };

            var codes = new int[] { 2, 3 };

            NewService().HasAnyLearningDeliveryAFinCodesForType(learningDeliveryAppFinRecords, "TypeOne", codes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodeForType_True()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
            {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
            };

            NewService().HasAnyLearningDeliveryAFinCodeForType(learningDeliveryAppFinRecords, "TypeTwo", 2).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodeForType_FalseNull()
        {
            NewService().HasAnyLearningDeliveryAFinCodeForType(null, "TypeTwo", 1).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodeForType_False_CodeNull()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
           {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
           };

            NewService().HasAnyLearningDeliveryAFinCodeForType(learningDeliveryAppFinRecords, "TypeTwo", null).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodeForType_False_Mismatch()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
           {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
           };

            NewService().HasAnyLearningDeliveryAFinCodeForType(learningDeliveryAppFinRecords, "TypeOne", 2).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodes_True()
        {
            var appFinRecords = new TestAppFinRecord[]
            {
                new TestAppFinRecord() { AFinCode = 1 },
                new TestAppFinRecord() { AFinCode = 2 },
                new TestAppFinRecord() { AFinCode = 3 },
            };

            var codes = new int[] { 1, 3 };

            NewService().HasAnyLearningDeliveryAFinCodes(appFinRecords, codes).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodes_False()
        {
            var appFinRecords = new TestAppFinRecord[]
            {
                new TestAppFinRecord() { AFinCode = 1 },
                new TestAppFinRecord() { AFinCode = 2 },
                new TestAppFinRecord() { AFinCode = 3 },
            };

            var codes = new int[] { 4, 5 };

            NewService().HasAnyLearningDeliveryAFinCodes(appFinRecords, codes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodes_FalseNull()
        {
            var codes = new int[] { 4, 5 };

            NewService().HasAnyLearningDeliveryAFinCodes(null, codes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodes_FalseCodeNull()
        {
            var appFinRecords = new TestAppFinRecord[]
            {
                new TestAppFinRecord() { AFinCode = 1 },
                new TestAppFinRecord() { AFinCode = 2 },
                new TestAppFinRecord() { AFinCode = 3 },
            };

            NewService().HasAnyLearningDeliveryAFinCodes(appFinRecords, null).Should().BeFalse();
        }

        private LearningDeliveryAppFinRecordQueryService NewService()
        {
            return new LearningDeliveryAppFinRecordQueryService();
        }
    }
}
