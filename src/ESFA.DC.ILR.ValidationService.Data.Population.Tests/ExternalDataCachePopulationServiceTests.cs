using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.Data.Postcodes.Model.Interfaces;
using ESFA.DC.Data.ULN.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Keys;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class ExternalDataCachePopulationServiceTests
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

        [Fact]
        public void UniquePostcodesFromMessage_LearnerPostcodes_NullLearners()
        {
            var message = new TestMessage();

            NewService().UniquePostcodesFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniquePostcodesFromMessage_LearnerPostcodes_Distinct()
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

            var result = NewService().UniquePostcodesFromMessage(message).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain("ABC");
            result.Should().Contain("DEF");
        }

        [Fact]
        public void UniquePostcodesFromMessage_LearnerPostcodePriors_NullLearners()
        {
            var message = new TestMessage();

            NewService().UniquePostcodesFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniquePostcodesFromMessage_LearnerPostcodePriors_Distinct()
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

            var result = NewService().UniquePostcodesFromMessage(message).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain("ABC");
            result.Should().Contain("DEF");
        }

        [Fact]
        public void UniquePostcodesFromMessage_LearningDeliveryLocationPostcodes_NullLearners()
        {
            var message = new TestMessage();

            NewService().UniquePostcodesFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniquePostcodesFromMessage_LearningDeliveryLocationPostcodes_Distinct()
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

            var result = NewService().UniquePostcodesFromMessage(message).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain("ABC");
            result.Should().Contain("DEF");
        }

        private ExternalDataCachePopulationService NewService(IExternalDataCache externalDataCache = null, ICache<IMessage> messageCache = null, ILARS lars = null, IULN uln = null, IPostcodes postcodes = null)
        {
            return new ExternalDataCachePopulationService(externalDataCache, messageCache, lars, uln, postcodes);
        }
    }
}
