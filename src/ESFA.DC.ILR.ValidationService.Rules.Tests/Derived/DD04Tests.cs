using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DD04Tests
    {
        [Fact]
        public void Derive()
        {
            var earliestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 1,
                FworkCodeNullable = 1,
                PwayCodeNullable = 1,
                AimType = 1,
                LearnStartDate = new DateTime(2015, 1, 1)
            };

            var latestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 1,
                FworkCodeNullable = 1,
                PwayCodeNullable = 1,
                AimType = 1,
                LearnStartDate = new DateTime(2017, 1, 1)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    latestLearningDelivery,
                    earliestLearningDelivery
                }
            };

            var dd04 = new DD04();

            dd04.Derive(learner.LearningDeliveries, latestLearningDelivery).Should().Be(new DateTime(2015, 1, 1));
        }

        [Fact]
        public void EarliestLearningDeliveryLearnStartDateFor_NullLearningDelivery()
        {
            var dd04 = new DD04();

            dd04.EarliestLearningDeliveryLearnStartDateFor(null, 1, 1, 1, 1).Should().BeNull();
        }

        [Fact]
        public void EarliestLearningDeliveryLearnStartDateFor_NoMatch()
        {
            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimType = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 1,
                    LearnStartDate = new DateTime(2017, 1, 1)
                }
            };

            var dd04 = new DD04();

            dd04.EarliestLearningDeliveryLearnStartDateFor(learningDeliveries, 1, 1, 1, 2).Should().BeNull();
        }

        [Fact]
        public void EarliestLearningDeliveryLearnStartDateFor_SingleMatch()
        {
            var learnStartDate = new DateTime(2017, 1, 1);

            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimType = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 1,
                    LearnStartDate = learnStartDate
                },
                new TestLearningDelivery()
                {
                    AimType = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 2,
                }
            };

            var dd04 = new DD04();

            dd04.EarliestLearningDeliveryLearnStartDateFor(learningDeliveries, 1, 1, 1, 1).Should().Be(learnStartDate);
        }
    }
}
