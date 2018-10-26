using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearningDeliveryWorkPlacementQueryServicesTests
    {
        [Fact]
        public void HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate_True()
        {
            DateTime? learnActEndDate = new DateTime(2018, 11, 1);

            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 10, 1) },
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 09, 1) },
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 12, 1) },
            };

            NewService().HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(learningDeliveryWorkPlacements, learnActEndDate).Should().BeTrue();
        }

        [Fact]
        public void HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate_False()
        {
            DateTime? learnActEndDate = new DateTime(2018, 11, 1);

            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 10, 1) },
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 09, 1) },
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = null },
            };

            NewService().HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(learningDeliveryWorkPlacements, learnActEndDate).Should().BeFalse();
        }

        [Fact]
        public void HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate_FalseNull()
        {
            DateTime? learnActEndDate = null;

            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = null },
            };

            NewService().HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(learningDeliveryWorkPlacements, learnActEndDate).Should().BeFalse();
        }

        private LearningDeliveryWorkPlacementQueryService NewService()
        {
            return new LearningDeliveryWorkPlacementQueryService();
        }
    }
}
