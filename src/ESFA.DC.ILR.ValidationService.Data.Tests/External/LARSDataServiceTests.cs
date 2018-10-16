using System;
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
        public void EffectiveDatesValidforLearnAimRef_True()
        {
            var learnAimRef = "LearnAimRef";
            var effectiveFrom = new DateTime(2018, 01, 01);
            var effectiveTo = new DateTime(2019, 01, 01);
            var date = new DateTime(2018, 01, 01);

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery
                    {
                        LearnAimRef = learnAimRef,
                        EffectiveFrom = effectiveFrom,
                        EffectiveTo = effectiveTo
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).EffectiveDatesValidforLearnAimRef(learnAimRef, date).Should().BeTrue();
        }

        [Fact]
        public void EffectiveDatesValidforLearnAimRef_True_EffectiveToNull()
        {
            var learnAimRef = "LearnAimRef";
            var effectiveFrom = new DateTime(2018, 01, 01);
            var date = new DateTime(2018, 01, 01);

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery
                    {
                        LearnAimRef = learnAimRef,
                        EffectiveFrom = effectiveFrom
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).EffectiveDatesValidforLearnAimRef(learnAimRef, date).Should().BeTrue();
        }

        [Fact]
        public void EffectiveDatesValidforLearnAimRef_False()
        {
            var learnAimRef = "LearnAimRef";
            var effectiveFrom = new DateTime(2018, 01, 01);
            var effectiveTo = new DateTime(2019, 01, 01);
            var date = new DateTime(2020, 01, 01);

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery
                    {
                        LearnAimRef = learnAimRef,
                        EffectiveFrom = effectiveFrom,
                        EffectiveTo = effectiveTo
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).EffectiveDatesValidforLearnAimRef(learnAimRef, date).Should().BeFalse();
        }

        [Fact]
        public void EffectiveDatesValidforLearnAimRef_False_LearningDeliveryNull()
        {
            var learnAimRef = "LearnAimRef";
            var effectiveFrom = new DateTime(2018, 01, 01);
            var effectiveTo = new DateTime(2019, 01, 01);
            var date = new DateTime(2020, 01, 01);

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).EffectiveDatesValidforLearnAimRef(learnAimRef, date).Should().BeFalse();
        }

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
        public void NotionalNVQLevelV2MatchForLearnAimRefAndLevel_True()
        {
            var learnAimRef = "LearnAimRef";
            var notionalNVQLevelv2 = "1";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { learnAimRef, new LearningDelivery() { LearnAimRef = learnAimRef, NotionalNVQLevelv2 = notionalNVQLevelv2 } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, notionalNVQLevelv2).Should().BeTrue();
        }

        [Fact]
        public void NotionalNVQLevelV2MatchForLearnAimRefAndLevel_False_Null()
        {
            var learnAimRef = "LearnAimRef";
            var notionalNVQLevelv2 = "1";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { learnAimRef, new LearningDelivery() { LearnAimRef = learnAimRef, NotionalNVQLevelv2 = notionalNVQLevelv2 } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).NotionalNVQLevelV2MatchForLearnAimRefAndLevel("NotLearnAimRef", notionalNVQLevelv2).Should().BeFalse();
        }

        [Fact]
        public void NotionalNVQLevelV2MatchForLearnAimRefAndLevel_False_Mismatch()
        {
            var learnAimRef = "LearnAimRef";
            var notionalNVQLevelv2 = "1";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { learnAimRef, new LearningDelivery() { LearnAimRef = learnAimRef, NotionalNVQLevelv2 = notionalNVQLevelv2 } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, "2").Should().BeFalse();
        }

        [Fact]
        public void NotionalNVQLevelV2MatchForLearnAimRefAndLevels_True()
        {
            var learnAimRef = "LearnAimRef";
            var notionalNVQLevelv2 = new List<string> { "1", "2", "H" };

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { learnAimRef, new LearningDelivery() { LearnAimRef = learnAimRef, NotionalNVQLevelv2 = "2" } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, notionalNVQLevelv2).Should().BeTrue();
        }

        [Fact]
        public void NotionalNVQLevelV2MatchForLearnAimRefAndLevels_False_Null()
        {
            var learnAimRef = "LearnAimRef";
            var notionalNVQLevelv2 = new List<string> { "1", "2", "H" };

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { learnAimRef, new LearningDelivery() { LearnAimRef = learnAimRef, NotionalNVQLevelv2 = "2" } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).NotionalNVQLevelV2MatchForLearnAimRefAndLevels("NotLearnAimRef", notionalNVQLevelv2).Should().BeFalse();
        }

        [Fact]
        public void NotionalNVQLevelV2MatchForLearnAimRefAndLevels_False_Mismatch()
        {
            var learnAimRef = "LearnAimRef";
            var notionalNVQLevelv2 = new List<string> { "1", "2", "H" };

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                { learnAimRef, new LearningDelivery() { LearnAimRef = learnAimRef, NotionalNVQLevelv2 = "G" } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, notionalNVQLevelv2).Should().BeFalse();
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
        public void FullLevel3PercentForLearnAimRefAndDateAndPercentValue_True()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 8, 1);
            var fullLevel3Percent = 100m;

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
                                EffectiveFrom = new DateTime(2018, 8, 1),
                                EffectiveTo = new DateTime(2019, 8, 1),
                                FullLevel3Percent = fullLevel3Percent
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .FullLevel3PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, learnStartDate, fullLevel3Percent)
                .Should().BeTrue();
        }

        [Fact]
        public void FullLevel3PercentForLearnAimRefAndDateAndPercentValue_False_Dates()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 8, 1);
            var fullLevel3Percent = 100m;

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
                                EffectiveFrom = new DateTime(2018, 10, 1),
                                EffectiveTo = new DateTime(2019, 8, 1),
                                FullLevel3Percent = fullLevel3Percent
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .FullLevel3PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, learnStartDate, fullLevel3Percent)
                .Should().BeFalse();
        }

        [Fact]
        public void FullLevel3PercentForLearnAimRefAndDateAndPercentValue_False_NullFullLevel3Percent()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 8, 1);
            var fullLevel3Percent = 100m;

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
                                EffectiveFrom = new DateTime(2018, 8, 1),
                                EffectiveTo = new DateTime(2019, 8, 1)
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .FullLevel3PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, learnStartDate, fullLevel3Percent)
                .Should().BeFalse();
        }

        [Fact]
        public void FullLevel3PercentForLearnAimRefAndDateAndPercentValue_False_Mismatch()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 8, 1);
            var fullLevel3Percent = 100m;

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
                                EffectiveFrom = new DateTime(2018, 8, 1),
                                EffectiveTo = new DateTime(2019, 8, 1),
                                FullLevel3Percent = fullLevel3Percent
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .FullLevel3PercentForLearnAimRefAndDateAndPercentValue("NOTLearnAimRef", learnStartDate, fullLevel3Percent)
                .Should().BeFalse();
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

        [Theory]
        [InlineData("456", "NUL")]
        [InlineData("456", "")]
        public void LearnDirectClassSystemCode2MatchForLearnAimRef_False(string learnAimRef, string learnDirectClassSystemCode2)
        {
            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LearnDirectClassSystemCode2 = learnDirectClassSystemCode2
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCache: externalDataCacheMock.Object).LearnDirectClassSystemCode2MatchForLearnAimRef(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LearnDirectClassSystemCode2MatchForLearnAimRef_True()
        {
            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    "123", new LearningDelivery()
                    {
                        LearnAimRef = "123",
                        LearnDirectClassSystemCode2 = "CDE"
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCache: externalDataCacheMock.Object).LearnDirectClassSystemCode2MatchForLearnAimRef("123").Should().BeTrue();
        }

        [Theory]
        [InlineData(3, "00100309", "2018-06-01")]
        [InlineData(1, "00100310", "2018-06-01")]
        [InlineData(1, "00100309", "2017-01-01")]
        [InlineData(3, "00100310", "2017-01-01")]
        public void BasicSkillsMatchForLearnAimRefAndStartDate_False(int basicSkillType, string learnAimRef, string learnStartDateString)
        {
            IEnumerable<int> basicSkillsTypes = new List<int>() { 01, 11, 13, 20, 23, 24, 29, 31, 02, 12, 14, 19, 21, 25, 30, 32, 33, 34, 35 };
            DateTime.TryParse(learnStartDateString, out DateTime learnStartDate);

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
                                BasicSkillsType = basicSkillType,
                                EffectiveFrom = new DateTime(2018, 03, 01),
                                EffectiveTo = new DateTime(2018, 09, 01)
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).BasicSkillsMatchForLearnAimRefAndStartDate(basicSkillsTypes, "00100309", learnStartDate).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, "00100309", "2018-06-01", "2018-09-01")]
        [InlineData(13, "00100309", "2018-06-01", null)]
        public void BasicSkillsMatchForLearnAimRefAndStartDate_True(int basicSkillType, string learnAimRef, string learnStartDateString, string larsEffectiveToDateString)
        {
            IEnumerable<int> basicSkillsTypes = new List<int>() { 01, 11, 13, 20, 23, 24, 29, 31, 02, 12, 14, 19, 21, 25, 30, 32, 33, 34, 35 };
            DateTime.TryParse(learnStartDateString, out DateTime learnStartDate);
            DateTime? larsEffectiveToDate = string.IsNullOrEmpty(larsEffectiveToDateString) ? (DateTime?)null : DateTime.Parse(larsEffectiveToDateString);

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
                                BasicSkillsType = basicSkillType,
                                EffectiveFrom = new DateTime(2018, 03, 01),
                                EffectiveTo = larsEffectiveToDate
                            }
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).BasicSkillsMatchForLearnAimRefAndStartDate(basicSkillsTypes, "00100309", learnStartDate).Should().BeTrue();
        }

        private LARSDataService NewService(IExternalDataCache externalDataCache = null)
        {
            return new LARSDataService(externalDataCache);
        }
    }
}