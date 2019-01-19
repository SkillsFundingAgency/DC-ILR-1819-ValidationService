using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_19RuleTests
    {
        [Fact]
        public void LearningDeliveriesForProgrammeAim_ReturnsNull_NoDeliveries()
        {
            NewRule().LearningDeliveriesForProgrammeAim(null).Should().BeNullOrEmpty();
        }

        [Fact]
        public void LearningDeliveriesForProgrammeAim_ReturnsNull_MisMatch()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery()
                {
                    ProgTypeNullable = 25,
                    StdCodeNullable = 1,
                    AimType = 2,
                    LearnPlanEndDate = new DateTime(2015, 1, 1)
                }
            };

            NewRule().LearningDeliveriesForProgrammeAim(learningDeliveries).Should().BeNullOrEmpty();
        }

        [Fact]
        public void LearningDeliveriesForProgrammeAim_Returns()
        {
            var learningDeliveryOne = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                StdCodeNullable = 1,
                AimType = 1,
                LearnPlanEndDate = new DateTime(2015, 1, 1)
            };

            var learningDeliveryTwo = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                StdCodeNullable = 1,
                AimType = 2,
                LearnPlanEndDate = new DateTime(2015, 1, 1)
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                learningDeliveryOne,
                learningDeliveryTwo
            };

            NewRule().LearningDeliveriesForProgrammeAim(learningDeliveries).Should().BeEquivalentTo(learningDeliveryOne);
        }

        [Fact]
        public void LearningDeliveriesForProgrammeAim_ReturnsMultiple()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery()
                {
                    ProgTypeNullable = 25,
                    StdCodeNullable = 1,
                    AimType = 1,
                    LearnPlanEndDate = new DateTime(2015, 1, 1)
                },
                new TestLearningDelivery()
                {
                    ProgTypeNullable = 26,
                    StdCodeNullable = 1,
                    AimType = 1,
                    LearnPlanEndDate = new DateTime(2015, 1, 1)
                }
            };

            NewRule().LearningDeliveriesForProgrammeAim(learningDeliveries).Should().BeEquivalentTo(learningDeliveries);
        }

        [Fact]
        public void LearningDeliveryHasApprenticeshipStandardType_False_Null()
        {
            NewRule().LearningDeliveryHasApprenticeshipStandardType(null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHasApprenticeshipStandardType_False()
        {
            var learningelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 26,
                StdCodeNullable = 1,
                AimType = 1,
                LearnPlanEndDate = new DateTime(2015, 1, 1)
            };

            NewRule().LearningDeliveryHasApprenticeshipStandardType(learningelivery).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHasApprenticeshipStandardType_True()
        {
            var learningelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                StdCodeNullable = 1,
                AimType = 1,
                LearnPlanEndDate = new DateTime(2015, 1, 1)
            };

            NewRule().LearningDeliveryHasApprenticeshipStandardType(learningelivery).Should().BeTrue();
        }

        [Fact]
        public void LatestLearningDeliveryLearnPlanEndDateFor_ReturnsLaterDate()
        {
            int? progType = 25;
            int? stdCode = 1;

            var earliestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = progType,
                StdCodeNullable = progType,
                AimType = 1,
                LearnPlanEndDate = new DateTime(2015, 1, 1)
            };

            var latestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = progType,
                StdCodeNullable = stdCode,
                AimType = 1,
                LearnPlanEndDate = new DateTime(2017, 1, 1)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    latestLearningDelivery,
                    earliestLearningDelivery
                }
            };

            NewRule().LatestLearningDeliveryLearnPlanEndDateFor(learner.LearningDeliveries, progType, stdCode).Should().Be(new DateTime(2017, 1, 1));
        }

        [Fact]
        public void LatestLearningDeliveryLearnPlanEndDateFor_ReturnsEarlierDate()
        {
            int? progType = 25;
            int? stdCode = 1;

            var earliestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = progType,
                StdCodeNullable = stdCode,
                AimType = 1,
                LearnPlanEndDate = new DateTime(2015, 1, 1)
            };

            var latestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = progType,
                StdCodeNullable = 2,
                AimType = 1,
                LearnPlanEndDate = new DateTime(2017, 1, 1)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    latestLearningDelivery,
                    earliestLearningDelivery
                }
            };

            NewRule().LatestLearningDeliveryLearnPlanEndDateFor(learner.LearningDeliveries, progType, stdCode).Should().Be(new DateTime(2015, 1, 1));
        }

        [Fact]
        public void LatestLearningDeliveryLearnPlanEndDateFor_Returns_Null()
        {
            int? progType = 25;
            int? stdCode = 1;

            var earliestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = progType,
                StdCodeNullable = 2,
                AimType = 1,
                LearnPlanEndDate = new DateTime(2015, 1, 1)
            };

            var latestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = progType,
                StdCodeNullable = 2,
                AimType = 1,
                LearnPlanEndDate = new DateTime(2017, 1, 1)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    latestLearningDelivery,
                    earliestLearningDelivery
                }
            };

            NewRule().LatestLearningDeliveryLearnPlanEndDateFor(learner.LearningDeliveries, progType, stdCode).Should().BeNull();
        }

        [Fact]
        public void Derive_Returns_Null_NoDeliveries()
        {
            NewRule().Derive(null, null).Should().BeNull();
        }

        [Fact]
        public void Derive_Returns_Null_SingleDelivery()
        {
            var earliestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                StdCodeNullable = 1,
                AimType = 2,
                LearnPlanEndDate = new DateTime(2015, 1, 1)
            };

            var latestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                StdCodeNullable = 1,
                AimType = 2,
                LearnPlanEndDate = new DateTime(2017, 1, 1)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    latestLearningDelivery,
                    earliestLearningDelivery
                }
            };

            NewRule().Derive(null, latestLearningDelivery).Should().BeNull();
        }

        [Fact]
        public void Derive_Returns_Null_ProgType()
        {
            var earliestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 26,
                StdCodeNullable = 1,
                AimType = 1,
                LearnPlanEndDate = new DateTime(2015, 1, 1)
            };

            var latestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 26,
                StdCodeNullable = 1,
                AimType = 1,
                LearnPlanEndDate = new DateTime(2017, 1, 1)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    latestLearningDelivery,
                    earliestLearningDelivery
                }
            };

            NewRule().Derive(learner.LearningDeliveries, latestLearningDelivery).Should().BeNull();
        }

        [Fact]
        public void Derive_Returns_Null_NoProgAims()
        {
            var earliestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                StdCodeNullable = 1,
                AimType = 2,
                LearnPlanEndDate = new DateTime(2015, 1, 1)
            };

            var latestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                StdCodeNullable = 1,
                AimType = 2,
                LearnPlanEndDate = new DateTime(2017, 1, 1)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    latestLearningDelivery,
                    earliestLearningDelivery
                }
            };

            NewRule().Derive(learner.LearningDeliveries, latestLearningDelivery).Should().BeNull();
        }

        private DerivedData_19Rule NewRule()
        {
            return new DerivedData_19Rule();
        }
    }
}
