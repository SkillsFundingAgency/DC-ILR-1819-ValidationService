using System.Collections.Generic;
using System.Linq;
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

        private PostcodesDataRetrievalService NewService(IPostcodes postcodes = null, ICache<IMessage> messageCache = null)
        {
            return new PostcodesDataRetrievalService(postcodes, messageCache);
        }
    }
}
