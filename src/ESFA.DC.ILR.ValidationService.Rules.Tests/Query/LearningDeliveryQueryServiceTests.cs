using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearningDeliveryQueryServiceTests
    {
        [Fact]
        public void LearningDaysForLearningDelivery_SameDay()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2018, 1, 1),
                LearnPlanEndDate = new DateTime(2018, 1, 1)
            };

            NewService().LearningDaysForLearningDelivery(learningDelivery).Should().Be(1);
        }

        [Fact]
        public void LearningDaysForLearningDelivery_Negative()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2018, 1, 1),
                LearnPlanEndDate = new DateTime(2017, 1, 1)
            };

            NewService().LearningDaysForLearningDelivery(learningDelivery).Should().Be(-364);
        }

        [Fact]
        public void LearningDaysForLearningDelivery_Positive()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2018, 1, 1),
                LearnPlanEndDate = new DateTime(2019, 1, 1)
            };

            NewService().LearningDaysForLearningDelivery(learningDelivery).Should().Be(366);
        }

        [Theory]
        [InlineData(null, 20, null)]
        [InlineData(20, 20, 1)]
        [InlineData(20, 10, 2)]
        [InlineData(6, 5, 1.2)]
        [InlineData(5, 4, 1.25)]
        public void AverageHoursPerLearningDay(int? addHours, int learningDays, double? averageHours)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                AddHoursNullable = addHours
            };

            var serviceMock = new Mock<LearningDeliveryQueryService>();

            serviceMock.Setup(s => s.LearningDaysForLearningDelivery(learningDelivery)).Returns(learningDays);
            serviceMock.CallBase = true;

            serviceMock.Object.AverageAddHoursPerLearningDay(learningDelivery).Should().Be(averageHours);
        }

        private LearningDeliveryQueryService NewService()
        {
            return new LearningDeliveryQueryService();
        }
    }
}
