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
        public void HasAnyLearningDeliveryFAMCodesForType_FalseNull()
        {
            var codes = new int[] { 1, 3 };

            NewService().HasAnyLearningDeliveryAFinCodesForType(null, "TypeTwo", codes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryFAMCodesForType_False_CodesNull()
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
        public void HasAnyLearningDeliveryFAMCodesForType_False_Mismatch()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
           {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
           };

            var codes = new int[] { 1, 3 };

            NewService().HasAnyLearningDeliveryAFinCodesForType(learningDeliveryAppFinRecords, "TypeOne", codes).Should().BeFalse();
        }

        private LearningDeliveryAppFinRecordQueryService NewService()
        {
            return new LearningDeliveryAppFinRecordQueryService();
        }
    }
}
