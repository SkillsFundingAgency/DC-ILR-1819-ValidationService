using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Data.ULN.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class ULNDataRetrievalServiceTests
    {
        [Fact]
        public void UniqueULNsFromMessage_NullLearners()
        {
            var message = new TestMessage();

            var result = NewService().UniqueULNsFromMessage(message);

            result.Should().BeEmpty();
        }

        [Fact]
        public void UniqueULNsFromMessage_Distinct()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner()
                    {
                        ULN = 1
                    },
                    new TestLearner()
                    {
                        ULN = 1
                    },
                    new TestLearner()
                    {
                        ULN = 2
                    }
                },
                LearnerDestinationAndProgressions = new List<TestLearnerDestinationAndProgression>
                {
                    new TestLearnerDestinationAndProgression
                    {
                        ULN = 1
                    },
                     new TestLearnerDestinationAndProgression
                    {
                        ULN = 2
                    },
                      new TestLearnerDestinationAndProgression
                    {
                        ULN = 3
                    }
                }
            };

            var result = NewService().UniqueULNsFromMessage(message).ToList();

            result.Should().HaveCount(3);
            result.Should().Contain(1);
            result.Should().Contain(2);
            result.Should().Contain(3);
        }

        [Fact]
        public void UniqueULNsFromMessage_LearnersOnly()
        {
            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner()
                    {
                        ULN = 1
                    },
                    new TestLearner()
                    {
                        ULN = 1
                    },
                    new TestLearner()
                    {
                        ULN = 2
                    }
                }
            };

            var result = NewService().UniqueULNsFromMessage(message).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain(1);
            result.Should().Contain(2);
        }

        [Fact]
        public void UniqueULNsFromMessage_LearnerDPOnly()
        {
            var message = new TestMessage()
            {
                LearnerDestinationAndProgressions = new List<TestLearnerDestinationAndProgression>
                {
                    new TestLearnerDestinationAndProgression
                    {
                        ULN = 1
                    },
                     new TestLearnerDestinationAndProgression
                    {
                        ULN = 2
                    },
                      new TestLearnerDestinationAndProgression
                    {
                        ULN = 2
                    }
                }
            };

            var result = NewService().UniqueULNsFromMessage(message).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain(1);
            result.Should().Contain(2);
        }

        private ULNDataRetrievalService NewService(IULN uln = null, ICache<IMessage> messageCache = null)
        {
            return new ULNDataRetrievalService(uln, messageCache);
        }
    }
}
