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
                AimTypeNullable = 1,
                LearnStartDateNullable = new DateTime(2015, 1, 1)
            };

            var latestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 1,
                FworkCodeNullable = 1,
                PwayCodeNullable = 1,
                AimTypeNullable = 1,
                LearnStartDateNullable = new DateTime(2017, 1, 1)
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
                    AimTypeNullable = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 1,
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
                    AimTypeNullable = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 1,
                    LearnStartDateNullable = learnStartDate
                },
                new TestLearningDelivery()
                {
                    AimTypeNullable = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 2,
                }
            };

            var dd04 = new DD04();

            dd04.EarliestLearningDeliveryLearnStartDateFor(learningDeliveries, 1, 1, 1, 1).Should().Be(learnStartDate);
        }

        [Fact]
        public void EarliestLearningDeliveryLearnStartDateFor_NullLearningDeliveries()
        {
            var learnStartDate = new DateTime(2017, 1, 1);

            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimTypeNullable = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 1,
                },
                new TestLearningDelivery()
                {
                    AimTypeNullable = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 2,
                }
            };

            var dd04 = new DD04();

            dd04.EarliestLearningDeliveryLearnStartDateFor(learningDeliveries, 1, 1, 1, 1).Should().BeNull();
        }

        [Fact]
        public void EarliestLearningDeliveryLearnStartDateFor_OrderedMatch_WithNull()
        {
            var earliestLearnStartDate = new DateTime(2017, 1, 1);
            var latestLearnStartDate = new DateTime(2018, 1, 1);

            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimTypeNullable = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 1,
                    LearnStartDateNullable = earliestLearnStartDate
                },
                new TestLearningDelivery()
                {
                    AimTypeNullable = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 1,
                    LearnStartDateNullable = latestLearnStartDate
                },
                new TestLearningDelivery()
                {
                    AimTypeNullable = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 1,
                }
            };

            var dd04 = new DD04();

            dd04.EarliestLearningDeliveryLearnStartDateFor(learningDeliveries, 1, 1, 1, 1).Should().Be(earliestLearnStartDate);
        }
    }
}
