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
using ESFA.DC.ILR.ValidationService.Data.Population.Keys;
using FluentAssertions;
using Moq;
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
                            new TestLearningDelivery() { FworkCodeNullable = 1, ProgTypeNullable = 1, PwayCodeNullable = null },
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
                                LearnAimRef = "ABC123",
                                FworkCodeNullable = 1,
                                ProgTypeNullable = 2,
                                PwayCodeNullable = 3
                            },
                            new TestLearningDelivery
                            {
                                LearnAimRef = "abc456",
                                FworkCodeNullable = 2,
                                ProgTypeNullable = 2,
                                PwayCodeNullable = 3
                            },
                            new TestLearningDelivery
                            {
                                LearnAimRef = "Abc789",
                                FworkCodeNullable = 1,
                                ProgTypeNullable = 3,
                                PwayCodeNullable = 3
                            }
                        }
                    }
                }
            };

            var larsMock = new Mock<ILARS>();
            var messageCacheMock = new Mock<ICache<IMessage>>();

            IEnumerable<LARS_Framework> larsFrameworks = new List<LARS_Framework>
            {
                new LARS_Framework
                {
                     FworkCode = 1,
                     ProgType = 2,
                     PwayCode = 3,
                     EffectiveFrom = new System.DateTime(2018, 8, 1),
                     LARS_FrameworkAims = new List<LARS_FrameworkAims>
                     {
                         new LARS_FrameworkAims
                         {
                             LearnAimRef = "ABC123",
                             FworkCode = 1,
                             ProgType = 2,
                             PwayCode = 3,
                             EffectiveFrom = new System.DateTime(2018, 8, 1),
                         }
                     },
                     LARS_FrameworkCmnComp = new List<LARS_FrameworkCmnComp>
                     {
                         new LARS_FrameworkCmnComp
                         {
                             FworkCode = 1,
                             ProgType = 2,
                             PwayCode = 3,
                             EffectiveFrom = new System.DateTime(2018, 8, 1),
                             CommonComponent = 1
                         }
                     }
                },
                new LARS_Framework
                {
                    FworkCode = 2,
                    ProgType = 2,
                    PwayCode = 3,
                    EffectiveFrom = new System.DateTime(2018, 8, 1),
                    LARS_FrameworkAims = new List<LARS_FrameworkAims>
                     {
                         new LARS_FrameworkAims
                         {
                             LearnAimRef = "ABC456",
                             FworkCode = 2,
                             ProgType = 2,
                             PwayCode = 3,
                             EffectiveFrom = new System.DateTime(2018, 8, 1),
                         }
                     },
                     LARS_FrameworkCmnComp = new List<LARS_FrameworkCmnComp>
                     {
                         new LARS_FrameworkCmnComp
                         {
                             FworkCode = 2,
                             ProgType = 2,
                             PwayCode = 3,
                             EffectiveFrom = new System.DateTime(2018, 8, 1),
                             CommonComponent = 2
                         }
                     }
                },
                new LARS_Framework
                {
                    FworkCode = 1,
                    ProgType = 3,
                    PwayCode = 3,
                    EffectiveFrom = new System.DateTime(2018, 8, 1),
                    LARS_FrameworkAims = new List<LARS_FrameworkAims>
                     {
                         new LARS_FrameworkAims
                         {
                             LearnAimRef = "abc789",
                             FworkCode = 1,
                             ProgType = 3,
                             PwayCode = 3,
                             EffectiveFrom = new System.DateTime(2018, 8, 1),
                         }
                     },
                     LARS_FrameworkCmnComp = new List<LARS_FrameworkCmnComp>
                     {
                         new LARS_FrameworkCmnComp
                         {
                             FworkCode = 1,
                             ProgType = 3,
                             PwayCode = 3,
                             EffectiveFrom = new System.DateTime(2018, 8, 1),
                             CommonComponent = 3
                         }
                     }
                }
            }.AsMockDbSet();

            var larsFrameworksMock = larsFrameworks.AsMockDbSet();

            larsMock.Setup(o => o.LARS_Framework).Returns(larsFrameworksMock);

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var larsFrameworksValues = await NewService(larsMock.Object, messageCacheMock.Object).RetrieveAsync(CancellationToken.None);

            larsFrameworksValues.Should().HaveCount(3);
            larsFrameworksValues.SelectMany(f => f.FrameworkAims).Should().HaveCount(3);
            larsFrameworksValues.SelectMany(f => f.FrameworkAims.Select(fa => fa.LearnAimRef)).Should().Contain("ABC123");
            larsFrameworksValues.SelectMany(f => f.FrameworkAims.Select(fa => fa.LearnAimRef)).Should().Contain("ABC456");
            larsFrameworksValues.SelectMany(f => f.FrameworkAims.Select(fa => fa.LearnAimRef)).Should().Contain("abc789");
        }

        private LARSFrameworkDataRetrievalService NewService(ILARS lars = null, ICache<IMessage> messageCache = null)
        {
            return new LARSFrameworkDataRetrievalService(lars, messageCache);
        }
    }
}
