using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.LARS;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.External
{
    public class LARSDataServiceTests
    {
        [Fact]
        public void FrameworkCodeExistsForFrameworkAims_True()
        {
            var learnAimRef = "LearnAimRef";
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var frameworks = new List<Framework>()
            {
                new Framework()
                {
                    FrameworkAims = new List<FrameworkAim>()
                    {
                        new FrameworkAim()
                        {
                            LearnAimRef = learnAimRef,
                            ProgType = progType,
                            FworkCode = fworkCode,
                            PwayCode = pwayCode,
                        }
                    }
                },
                new Framework()
                {
                    FrameworkAims = new List<FrameworkAim>()
                    {
                        new FrameworkAim(),
                    }
                },
                new Framework()
                {
                    FrameworkAims = null
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.Frameworks).Returns(frameworks);

            NewService(externalDataCacheMock.Object).FrameworkCodeExistsForFrameworkAims(learnAimRef, progType, fworkCode, pwayCode).Should().BeTrue();
        }

        [Fact]
        public void FrameworkCodeExistsForFrameworkAims_False()
        {
            var learnAimRef = "LearnAimRef";
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var frameworks = new List<Framework>()
            {
                new Framework()
                {
                    FrameworkAims = new List<FrameworkAim>()
                    {
                        new FrameworkAim()
                        {
                            LearnAimRef = learnAimRef,
                            ProgType = progType,
                            FworkCode = fworkCode,
                            PwayCode = pwayCode + 1,
                        }
                    }
                },
                new Framework()
                {
                    FrameworkAims = new List<FrameworkAim>()
                    {
                        new FrameworkAim(),
                    }
                },
                new Framework()
                {
                    FrameworkAims = null
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.Frameworks).Returns(frameworks);

            NewService(externalDataCacheMock.Object).FrameworkCodeExistsForFrameworkAims(learnAimRef, progType, fworkCode, pwayCode).Should().BeFalse();
        }

        [Fact]
        public void FrameworkCodeExistsForCommonComponent_True()
        {
            var learnAimRef = "LearnAimRef";
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;
            var frameworkCommonComponent = 1;

            var learningDeliveries = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        FrameworkCommonComponent = frameworkCommonComponent,
                    }
                }
            };

            var frameworks = new List<Framework>()
            {
                new Framework()
                {
                    FrameworkCommonComponents = new List<FrameworkCommonComponent>()
                    {
                        new FrameworkCommonComponent()
                        {
                            ProgType = progType,
                            FworkCode = fworkCode,
                            PwayCode = pwayCode,
                            CommonComponent = frameworkCommonComponent,
                        }
                    }
                },
                new Framework()
                {
                    FrameworkCommonComponents = new List<FrameworkCommonComponent>()
                    {
                        new FrameworkCommonComponent()
                    }
                },
                new Framework()
                {
                    FrameworkCommonComponents = null,
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveries);
            externalDataCacheMock.SetupGet(c => c.Frameworks).Returns(frameworks);

            NewService(externalDataCacheMock.Object).FrameworkCodeExistsForCommonComponent(learnAimRef, progType, fworkCode, pwayCode).Should().BeTrue();
        }

        [Fact]
        public void FrameworkCodeExistsForCommonComponent_False_NoLearningDelivery()
        {
            var learnAimRef = "LearnAimRef";

            var learningDeliveries = new Dictionary<string, LearningDelivery>()
            {
                { "MissingLearnAimRef", null }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveries);

            NewService(externalDataCacheMock.Object).FrameworkCodeExistsForCommonComponent(learnAimRef, 1, 1, 1).Should().BeFalse();
        }

        [Fact]
        public void FrameworkCodeExistsForCommonComponent_False_NoFrameworkCommonComponent()
        {
            var learnAimRef = "LearnAimRef";
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;
            var frameworkCommonComponent = 1;

            var learningDeliveries = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        FrameworkCommonComponent = frameworkCommonComponent,
                    }
                }
            };

            var frameworks = new List<Framework>()
            {
                new Framework()
                {
                    FrameworkCommonComponents = new List<FrameworkCommonComponent>()
                    {
                        new FrameworkCommonComponent()
                        {
                            ProgType = progType,
                            FworkCode = fworkCode,
                            PwayCode = pwayCode,
                            CommonComponent = frameworkCommonComponent + 1,
                        }
                    }
                },
                new Framework()
                {
                    FrameworkCommonComponents = new List<FrameworkCommonComponent>()
                    {
                        new FrameworkCommonComponent()
                    }
                },
                new Framework()
                {
                    FrameworkCommonComponents = null,
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveries);
            externalDataCacheMock.SetupGet(c => c.Frameworks).Returns(frameworks);

            NewService(externalDataCacheMock.Object).FrameworkCodeExistsForCommonComponent(learnAimRef, progType, fworkCode, pwayCode).Should().BeFalse();
        }

        [Fact]
        public void LearnAimRefExists_True()
        {
            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { "One", null },
                { "Two", null },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).LearnAimRefExists("One").Should().BeTrue();
        }

        [Fact]
        public void LearnAimRefExists_False()
        {
            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { "One", null },
                { "Two", null },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).LearnAimRefExists("Three").Should().BeFalse();
        }

        [Fact]
        public void NotionalNVQLevelMatchForLearnAimRef_True()
        {
            var learnAimRef = "LearnAimRef";
            var notionalNVQLevel = "E";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { learnAimRef, new LearningDelivery() { LearnAimRef = learnAimRef, NotionalNVQLevel = notionalNVQLevel } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).NotionalNVQLevelMatchForLearnAimRef(learnAimRef, notionalNVQLevel).Should().BeTrue();
        }

        [Fact]
        public void NotionalNVQLevelMatchForLearnAimRef_False_Null()
        {
            var learnAimRef = "LearnAimRef";
            var notionalNVQLevel = "E";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { learnAimRef, new LearningDelivery() { LearnAimRef = learnAimRef, NotionalNVQLevel = notionalNVQLevel } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).NotionalNVQLevelMatchForLearnAimRef("NotLearnAimRef", notionalNVQLevel).Should().BeFalse();
        }

        [Fact]
        public void NotionalNVQLevelMatchForLearnAimRef_False_Mismatch()
        {
            var learnAimRef = "LearnAimRef";
            var notionalNVQLevel = "E";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { learnAimRef, new LearningDelivery() { LearnAimRef = learnAimRef, NotionalNVQLevel = notionalNVQLevel } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).NotionalNVQLevelMatchForLearnAimRef(learnAimRef, "2").Should().BeFalse();
        }

        [Fact]
        public void NotionalNVQLevelV2MatchForLearnAimRef_True()
        {
            var learnAimRef = "LearnAimRef";
            var notionalNVQLevelv2 = "1";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { learnAimRef, new LearningDelivery() { LearnAimRef = learnAimRef, NotionalNVQLevelv2 = notionalNVQLevelv2 } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).NotionalNVQLevelV2MatchForLearnAimRef(learnAimRef, notionalNVQLevelv2).Should().BeTrue();
        }

        [Fact]
        public void NotionalNVQLevelV2MatchForLearnAimRef_False_Null()
        {
            var learnAimRef = "LearnAimRef";
            var notionalNVQLevelv2 = "1";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { learnAimRef, new LearningDelivery() { LearnAimRef = learnAimRef, NotionalNVQLevelv2 = notionalNVQLevelv2 } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).NotionalNVQLevelV2MatchForLearnAimRef("NotLearnAimRef", notionalNVQLevelv2).Should().BeFalse();
        }

        [Fact]
        public void NotionalNVQLevelV2MatchForLearnAimRef_False_Mismatch()
        {
            var learnAimRef = "LearnAimRef";
            var notionalNVQLevelv2 = "1";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { learnAimRef, new LearningDelivery() { LearnAimRef = learnAimRef, NotionalNVQLevelv2 = notionalNVQLevelv2 } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).NotionalNVQLevelV2MatchForLearnAimRef(learnAimRef, "2").Should().BeFalse();
        }

        [Fact]
        public void FullLevel2EntitlementCategoryMatchForLearnAimRef_True()
        {
            var learnAimRef = "LearnAimRef";
            var fullLevel2EntitlementCategory = 1;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AnnualValues = new List<AnnualValue>
                        {
                            new AnnualValue
                            {
                                FullLevel2EntitlementCategory = fullLevel2EntitlementCategory
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).FullLevel2EntitlementCategoryMatchForLearnAimRef(learnAimRef, fullLevel2EntitlementCategory).Should().BeTrue();
        }

        [Fact]
        public void FullLevel2EntitlementCategoryMatchForLearnAimRef_False_Null_LearnAimRef()
        {
            var learnAimRef = "LearnAimRef";
            var fullLevel2EntitlementCategory = 1;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AnnualValues = new List<AnnualValue>
                        {
                            new AnnualValue
                            {
                                FullLevel2EntitlementCategory = fullLevel2EntitlementCategory
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).FullLevel2EntitlementCategoryMatchForLearnAimRef("NotLearnAimRef", fullLevel2EntitlementCategory).Should().BeFalse();
        }

        [Fact]
        public void FullLevel2EntitlementCategoryMatchForLearnAimRef_False_Null_AnnualValue()
        {
            var learnAimRef = "LearnAimRef";
            var fullLevel2EntitlementCategory = 1;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).FullLevel2EntitlementCategoryMatchForLearnAimRef(learnAimRef, fullLevel2EntitlementCategory).Should().BeFalse();
        }

        [Fact]
        public void FullLevel2EntitlementCategoryMatchForLearnAimRef_False_Mismatch()
        {
            var learnAimRef = "LearnAimRef";
            var fullLevel2EntitlementCategory = 1;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AnnualValues = new List<AnnualValue>
                        {
                            new AnnualValue
                            {
                                FullLevel2EntitlementCategory = fullLevel2EntitlementCategory
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).FullLevel2EntitlementCategoryMatchForLearnAimRef(learnAimRef, 2).Should().BeFalse();
        }

        [Fact]
        public void FullLevel3EntitlementCategoryMatchForLearnAimRef_True()
        {
            var learnAimRef = "LearnAimRef";
            var fullLevel3EntitlementCategory = 1;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AnnualValues = new List<AnnualValue>
                        {
                            new AnnualValue
                            {
                                FullLevel3EntitlementCategory = fullLevel3EntitlementCategory
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).FullLevel3EntitlementCategoryMatchForLearnAimRef(learnAimRef, fullLevel3EntitlementCategory).Should().BeTrue();
        }

        [Fact]
        public void FullLevel3EntitlementCategoryMatchForLearnAimRef_False_Null_LearnAimRef()
        {
            var learnAimRef = "LearnAimRef";
            var fullLevel3EntitlementCategory = 1;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AnnualValues = new List<AnnualValue>
                        {
                            new AnnualValue
                            {
                                FullLevel3EntitlementCategory = fullLevel3EntitlementCategory
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).FullLevel3EntitlementCategoryMatchForLearnAimRef("NotLearnAimRef", fullLevel3EntitlementCategory).Should().BeFalse();
        }

        [Fact]
        public void FullLevel3EntitlementCategoryMatchForLearnAimRef_False_Null_AnnualValue()
        {
            var learnAimRef = "LearnAimRef";
            var fullLevel3EntitlementCategory = 1;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).FullLevel3EntitlementCategoryMatchForLearnAimRef(learnAimRef, fullLevel3EntitlementCategory).Should().BeFalse();
        }

        [Fact]
        public void FullLevel3EntitlementCategoryMatchForLearnAimRef_False_Mismatch()
        {
            var learnAimRef = "LearnAimRef";
            var fullLevel3EntitlementCategory = 1;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AnnualValues = new List<AnnualValue>
                        {
                            new AnnualValue
                            {
                                FullLevel3EntitlementCategory = fullLevel3EntitlementCategory
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).FullLevel3EntitlementCategoryMatchForLearnAimRef(learnAimRef, 2).Should().BeFalse();
        }

        [Fact]
        public void BasicSkillsMatchForLearnAimRef_True()
        {
            var learnAimRef = "LearnAimRef";
            var basicSkills = 1;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AnnualValues = new List<AnnualValue>
                        {
                            new AnnualValue
                            {
                                BasicSkills = basicSkills
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).BasicSkillsMatchForLearnAimRef(learnAimRef, basicSkills).Should().BeTrue();
        }

        [Fact]
        public void BasicSkillsMatchForLearnAimRef_False_Null_LearnAimRef()
        {
            var learnAimRef = "LearnAimRef";
            var basicSkills = 1;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AnnualValues = new List<AnnualValue>
                        {
                            new AnnualValue
                            {
                                BasicSkills = basicSkills
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).BasicSkillsMatchForLearnAimRef("NotLearnAimRef", basicSkills).Should().BeFalse();
        }

        [Fact]
        public void BasicSkillsMatchForLearnAimRef_False_Null_AnnualValue()
        {
            var learnAimRef = "LearnAimRef";
            var basicSkills = 1;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).BasicSkillsMatchForLearnAimRef(learnAimRef, basicSkills).Should().BeFalse();
        }

        [Fact]
        public void BasicSkillsMatchForLearnAimRef_False_Mismatch()
        {
            var learnAimRef = "LearnAimRef";
            var basicSkills = 1;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AnnualValues = new List<AnnualValue>
                        {
                            new AnnualValue
                            {
                                BasicSkills = basicSkills
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).BasicSkillsMatchForLearnAimRef(learnAimRef, 2).Should().BeFalse();
        }

        private LARSDataService NewService(IExternalDataCache externalDataCache = null)
        {
            return new LARSDataService(externalDataCache);
        }
    }
}