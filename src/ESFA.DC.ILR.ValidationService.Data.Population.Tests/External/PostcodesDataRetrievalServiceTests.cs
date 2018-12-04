using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.Postcodes.Model;
using ESFA.DC.Data.Postcodes.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class PostcodesDataRetrievalServiceTests
    {
        [Fact]
        public void UniquePostcodesFromMessage()
        {
            var message = new TestMessage();

            var postcodesDataRetrievalServiceMock = new Mock<PostcodesDataRetrievalService> { CallBase = true };

            postcodesDataRetrievalServiceMock.Setup(s => s.UniqueLearnerPostcodesFromMessage(message)).Returns(new List<string>() { "abc" });
            postcodesDataRetrievalServiceMock.Setup(s => s.UniqueLearnerPostcodePriorsFromMessage(message)).Returns(new List<string>() { "abc" });
            postcodesDataRetrievalServiceMock.Setup(s => s.UniqueLearningDeliveryLocationPostcodesFromMessage(message)).Returns(new List<string>() { "abc" });

            var postcodes = postcodesDataRetrievalServiceMock.Object.UniquePostcodesFromMessage(message).ToList();

            postcodes.Should().HaveCount(1);
            postcodes.Should().Contain("abc");
        }

        [Fact]
        public void UniqueLearnerPostcodesFromMessage_NullLearners()
        {
            var message = new TestMessage();

            NewService().UniqueLearnerPostcodesFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniqueLearnerPostcodesFromMessage_Distinct()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner()
                    {
                        Postcode = "ABC"
                    },
                    new TestLearner()
                    {
                        Postcode = "ABC"
                    },
                    new TestLearner()
                    {
                        Postcode = "DEF"
                    }
                }
            };

            var result = NewService().UniqueLearnerPostcodesFromMessage(message).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain("ABC");
            result.Should().Contain("DEF");
        }

        [Fact]
        public void UniqueLearnerPostcodePriorsFromMessage_NullLearners()
        {
            var message = new TestMessage();

            NewService().UniqueLearnerPostcodePriorsFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniqueLearnerPostcodePriorssFromMessage_Distinct()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner()
                    {
                        PostcodePrior = "ABC"
                    },
                    new TestLearner()
                    {
                        PostcodePrior = "ABC"
                    },
                    new TestLearner()
                    {
                        PostcodePrior = "DEF"
                    }
                }
            };

            var result = NewService().UniqueLearnerPostcodePriorsFromMessage(message).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain("ABC");
            result.Should().Contain("DEF");
        }

        [Fact]
        public void UniqueLearningDeliveryLocationPostcodesFromMessage_NullLearners()
        {
            var message = new TestMessage();

            NewService().UniqueLearningDeliveryLocationPostcodesFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniqueLearningDeliveryLocationPostcodesFromMessage_Distinct()
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
                                DelLocPostCode = "ABC"
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                DelLocPostCode = "ABC"
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearningDeliveries = new List<TestLearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                DelLocPostCode = "DEF"
                            }
                        }
                    }
                }
            };

            var result = NewService().UniqueLearningDeliveryLocationPostcodesFromMessage(message).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain("ABC");
            result.Should().Contain("DEF");
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
                        Postcode = "CV1 2WT",
                        PostcodePrior = "cv2 3wt",
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                DelLocPostCode = "cv2 4AA"
                            }
                        }
                    }
                }
            };

            var postcodesMock = new Mock<IPostcodes>();
            var messageCacheMock = new Mock<ICache<IMessage>>();

            IEnumerable<MasterPostcode> masterPostcodeList = new List<MasterPostcode>
            {
                new MasterPostcode
                {
                    Postcode = "CV1 2WT"
                },
                new MasterPostcode
                {
                    Postcode = "CV2 3WT"
                },
                new MasterPostcode
                {
                    Postcode = "CV2 4AA"
                }
            };

            var masterPostcodesMock = masterPostcodeList.AsMockDbSet();

            postcodesMock.Setup(o => o.MasterPostcodes).Returns(masterPostcodesMock);

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var postcodes = await NewService(postcodesMock.Object, messageCacheMock.Object).RetrieveAsync(CancellationToken.None);

            postcodes.Should().HaveCount(3);
            postcodes.Should().Contain("CV1 2WT");
            postcodes.Should().Contain("CV2 3WT");
            postcodes.Should().Contain("CV2 4AA");
        }

        private PostcodesDataRetrievalService NewService(IPostcodes postcodes = null, ICache<IMessage> messageCache = null)
        {
            return new PostcodesDataRetrievalService(postcodes, messageCache);
        }
    }
}
