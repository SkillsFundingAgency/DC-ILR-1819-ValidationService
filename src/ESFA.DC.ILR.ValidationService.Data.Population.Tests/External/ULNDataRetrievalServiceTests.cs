using ESFA.DC.ILR.Tests.Model;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using Xunit;
using ESFA.DC.Data.ULN.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;

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
