using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using ESFA.DC.ILR.ValidationService.Data.Population.Keys;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class LARSFrameworkDataRetrievalServiceTests
    {
        [Fact]
        public void UniqueFrameworksFromMessage_NullLearners()
        {
            var message = new TestMessage();

            NewService().UniqueFrameworksFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniqueFrameworksFromMessage_NullLearningDeliveries()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner()
                }
            };

            NewService().UniqueFrameworksFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniqueFrameworksFromMessage_NullValues()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery() { FworkCodeNullable = 1, ProgTypeNullable = 1, PwayCodeNullable = null},
                            new TestLearningDelivery() { FworkCodeNullable = 1, ProgTypeNullable = null, PwayCodeNullable = 1 },
                            new TestLearningDelivery() { FworkCodeNullable = null, ProgTypeNullable = 1, PwayCodeNullable = 1 },
                        }
                    }
                }
            };

            NewService().UniqueFrameworksFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniqueFrameworksFromMessage_Group()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery() { FworkCodeNullable = 1, ProgTypeNullable = 1, PwayCodeNullable = 1 },
                            new TestLearningDelivery() { FworkCodeNullable = 1, ProgTypeNullable = 1, PwayCodeNullable = 1 },
                            new TestLearningDelivery() { FworkCodeNullable = 2, ProgTypeNullable = 1, PwayCodeNullable = 1 },
                        }
                    }
                }
            };

            var result = NewService().UniqueFrameworksFromMessage(message).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain(new FrameworkKey(1, 1, 1));
            result.Should().Contain(new FrameworkKey(2, 1, 1));
        }

        [Fact]
        public void UniqueLearnAimRefsFromMessage_NoLearners()
        {
            var message = new TestMessage();

            NewService().UniqueLearnAimRefsFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniqueLearnAimRefsFromMessage_NoLearningDeliveries()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner(),
                    new TestLearner(),
                }
            };

            NewService().UniqueLearnAimRefsFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniqueLearnAimRefsFromMessage_NoLearnAimRefs()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()
                        }
                    },
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()
                        }
                    },
                }
            };

            NewService().UniqueLearnAimRefsFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniqueLearnAimRefsFromMessage_LearnAimRefs()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                LearnAimRef =  "A"
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()

                            {
                                LearnAimRef =  "B"
                            }
                        }
                    },
                }
            };

            var result = NewService().UniqueLearnAimRefsFromMessage(message).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain("A");
            result.Should().Contain("B");
        }

        [Fact]
        public void UniqueLearnAimRefsFromMessage_DistinctLearnAimRefs()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                LearnAimRef =  "A"
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()

                            {
                                LearnAimRef =  "A"
                            }
                        }
                    },
                }
            };

            var result = NewService().UniqueLearnAimRefsFromMessage(message).ToList();

            result.Should().HaveCount(1);
            result.Should().Contain("A");
        }

        private LARSFrameworkDataRetrievalService NewService(ILARS lars = null, ICache<IMessage> messageCache = null)
        {
            return new LARSFrameworkDataRetrievalService(lars, messageCache);
        }

    }
}
