using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearnerQueryServiceTests
    {
        [Fact]
        public void HasLearningDeliveryFAMCodeForType_True()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = "ABC",
                                LearnDelFAMType = "DEF"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = "GHI",
                                LearnDelFAMType = "JKL"
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = "MNO",
                                LearnDelFAMType = "PQR"
                            }
                        }
                    }
                }
            };

            NewService().HasLearningDeliveryFAMCodeForType(learner, "PQR", "MNO").Should().BeTrue();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_False()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = "ABC",
                                LearnDelFAMType = "DEF"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = "GHI",
                                LearnDelFAMType = "JKL"
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = "MNO",
                                LearnDelFAMType = "PQR"
                            }
                        }
                    }
                }
            };

            NewService().HasLearningDeliveryFAMCodeForType(learner, "STU", "VWX").Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_False_NoLearningDeliveries()
        {
            var learner = new TestLearner();

            NewService().HasLearningDeliveryFAMCodeForType(learner, "A", "B").Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_False_NoLearningDeliveryFams()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery(),
                    new TestLearningDelivery(),
                }
            };
            NewService().HasLearningDeliveryFAMCodeForType(learner, "A", "B").Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_False_MixedLearningDeliveryFams()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = "ABC",
                                LearnDelFAMType = "DEF"
                            }
                        }
                    },
                    new TestLearningDelivery(),
                }
            };

            NewService().HasLearningDeliveryFAMCodeForType(learner, "A", "B").Should().BeFalse();
        }

        private LearnerQueryService NewService()
        {
            return new LearnerQueryService();
        }
    }
}
