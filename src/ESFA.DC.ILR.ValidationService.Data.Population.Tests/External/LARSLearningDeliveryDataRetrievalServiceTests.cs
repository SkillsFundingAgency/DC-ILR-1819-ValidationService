using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class LARSLearningDeliveryDataRetrievalServiceTests
    {
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
                                LearnAimRef = "A"
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                LearnAimRef = "B"
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
                                LearnAimRef = "A"
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                LearnAimRef = "A"
                            }
                        }
                    },
                }
            };

            var result = NewService().UniqueLearnAimRefsFromMessage(message).ToList();

            result.Should().HaveCount(1);
            result.Should().Contain("A");
        }

        private LARSLearningDeliveryDataRetrievalService NewService(ILARS lars = null, ICache<IMessage> messageCache = null)
        {
            return new LARSLearningDeliveryDataRetrievalService(lars, messageCache);
        }
    }
}
