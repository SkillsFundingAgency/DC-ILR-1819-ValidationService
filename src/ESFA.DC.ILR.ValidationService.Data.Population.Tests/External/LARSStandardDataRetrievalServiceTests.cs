using ESFA.DC.Data.LARS.Model;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class LARSStandardDataRetrievalServiceTests
    {
        [Fact]
        public void UniqueSTDCodesFromMessage_NullLearners()
        {
            var message = new TestMessage();

            var result = NewService().UniqueSTDCodesFromMessage(message);

            result.Should().BeEmpty();
        }

        [Fact]
        public void UniqueSTDCodesFromMessage_NullLearningDeliveries()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner()
                }
            };

            var result = NewService().UniqueSTDCodesFromMessage(message);

            result.Should().BeEmpty();
        }

        [Fact]
        public void UniqueSTDCodesFromMessage_NullValues()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery() { StdCodeNullable = null }
                        }
                    }
                }
            };

            var result = NewService().UniqueSTDCodesFromMessage(message);

            result.Should().BeEmpty();
        }

        [Fact]
        public void UniqueSTDCodesFromMessage_Distinct()
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
                                StdCodeNullable = 1
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                StdCodeNullable = 1
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                StdCodeNullable = 2
                            }
                        }
                    }
                }
            };

            var result = NewService().UniqueSTDCodesFromMessage(message).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain(1);
            result.Should().Contain(2);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        public async Task Retrieve_NoMatch(int? stdCode)
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
                                StdCodeNullable = stdCode
                            }
                        }
                    }
                }
            };

            var larsMock = new Mock<ILARS>();
            var messageCacheMock = new Mock<ICache<IMessage>>();

            var larsStandardList = new List<LARS_Standard>()
            {
                new LARS_Standard()
                {
                    StandardCode = 1,
                    StandardSectorCode = "2",
                    NotionalEndLevel = "3",
                    EffectiveFrom = new DateTime(2018, 01, 01),
                    EffectiveTo = new DateTime(2019, 01, 01)
                },
                new LARS_Standard()
                {
                    StandardCode = 2,
                    StandardSectorCode = "2",
                    NotionalEndLevel = "3",
                    EffectiveFrom = new DateTime(2018, 01, 01),
                    EffectiveTo = new DateTime(2019, 01, 01)
                },
                new LARS_Standard()
                {
                    StandardCode = 3,
                    StandardSectorCode = "2",
                    NotionalEndLevel = "3",
                    EffectiveFrom = new DateTime(2018, 01, 01),
                    EffectiveTo = new DateTime(2019, 01, 01)
                }
            };

            var larsStandardsMock = larsStandardList.AsMockDbSet();

            larsMock.Setup(lm => lm.LARS_Standard).Returns(larsStandardsMock);

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var result = await NewService(larsMock.Object, messageCacheMock.Object).RetrieveAsync(CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Retrieve()
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
                                StdCodeNullable = 1
                            },
                            new TestLearningDelivery()
                            {
                                StdCodeNullable = 2
                            },
                            new TestLearningDelivery()
                            {
                                StdCodeNullable = 3
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                StdCodeNullable = 1
                            },
                            new TestLearningDelivery()
                            {
                                StdCodeNullable = 2
                            },
                            new TestLearningDelivery()
                            {
                                StdCodeNullable = 3
                            }
                        }
                    }
                }
            };

            var larsMock = new Mock<ILARS>();
            var messageCacheMock = new Mock<ICache<IMessage>>();

            var larsStandardList = new List<LARS_Standard>()
            {
                new LARS_Standard()
                {
                    StandardCode = 1,
                    StandardSectorCode = "2",
                    NotionalEndLevel = "3",
                    EffectiveFrom = new DateTime(2018, 01, 01),
                    EffectiveTo = new DateTime(2019, 01, 01)
                },
                new LARS_Standard()
                {
                    StandardCode = 2,
                    StandardSectorCode = "2",
                    NotionalEndLevel = "3",
                    EffectiveFrom = new DateTime(2018, 01, 01),
                    EffectiveTo = new DateTime(2019, 01, 01)
                },
                new LARS_Standard()
                {
                    StandardCode = 3
                }
            };

            var larsStandardsMock = larsStandardList.AsMockDbSet();

            larsMock.Setup(lm => lm.LARS_Standard).Returns(larsStandardsMock);

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var result = await NewService(larsMock.Object, messageCacheMock.Object).RetrieveAsync(CancellationToken.None);

            result.Should().HaveCount(3);
            result.Select(r => r.StandardCode).Should().Contain(1);
            result.Select(r => r.StandardCode).Should().Contain(2);
            result.Select(r => r.StandardCode).Should().Contain(3);
        }

        private LARSStandardDataRetrievalService NewService(ILARS lars = null, ICache<IMessage> messageCache = null)
        {
            return new LARSStandardDataRetrievalService(lars, messageCache);
        }
    }
}
