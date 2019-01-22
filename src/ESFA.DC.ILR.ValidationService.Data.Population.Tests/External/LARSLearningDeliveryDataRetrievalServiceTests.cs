using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.LARS.Model;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using FluentAssertions;
using Moq;
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

        [Fact]
        public async Task Retrieve()
        {
            var message = new TestMessage
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 1
                },
                Learners = new List<TestLearner>()
                {
                    new TestLearner
                    {
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                LearnAimRef = "ABC123"
                            },
                            new TestLearningDelivery
                            {
                                LearnAimRef = "abc456"
                            },
                            new TestLearningDelivery
                            {
                                LearnAimRef = "Abc789"
                            }
                        }
                    }
                }
            };

            var larsMock = new Mock<ILARS>();
            var messageCacheMock = new Mock<ICache<IMessage>>();

            IEnumerable<LARS_LearningDelivery> larsLearningDelieries = new List<LARS_LearningDelivery>
            {
                new LARS_LearningDelivery
                {
                    LearnAimRef = "ABC123",
                },
                new LARS_LearningDelivery
                {
                    LearnAimRef = "ABC456"
                },
                new LARS_LearningDelivery
                {
                    LearnAimRef = "abc789"
                }
            };

            IEnumerable<LARS_FrameworkAims> larsFrameworkAims = new List<LARS_FrameworkAims>
            {
                new LARS_FrameworkAims
                {
                    LearnAimRef = "ABC123"
                },
                 new LARS_FrameworkAims
                {
                    LearnAimRef = "ABC123"
                },
                new LARS_FrameworkAims
                {
                    LearnAimRef = "ABC456"
                },
                new LARS_FrameworkAims
                {
                    LearnAimRef = "abc789"
                }
            };

            var larsLearningDeliveryMock = larsLearningDelieries.AsMockDbSet();
            var larsFrameworkAimsMock = larsFrameworkAims.AsMockDbSet();

            larsMock.Setup(o => o.LARS_LearningDelivery).Returns(larsLearningDeliveryMock);
            larsMock.Setup(o => o.LARS_FrameworkAims).Returns(larsFrameworkAimsMock);

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var learningDeliveries = await NewService(larsMock.Object, messageCacheMock.Object).RetrieveAsync(CancellationToken.None);

            learningDeliveries.Should().HaveCount(3);
            learningDeliveries.Keys.Should().Contain("ABC123");
            learningDeliveries.Keys.Should().Contain("abc456");
            learningDeliveries.Keys.Should().Contain("Abc456");
        }

        private LARSLearningDeliveryDataRetrievalService NewService(ILARS lars = null, ICache<IMessage> messageCache = null)
        {
            return new LARSLearningDeliveryDataRetrievalService(lars, messageCache);
        }
    }
}
