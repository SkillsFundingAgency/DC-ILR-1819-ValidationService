﻿using ESFA.DC.ILR.ValidationService.Data.External.LARS;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
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
                        LARSValidities = new List<LARSValidity>
                        {
                            new LARSValidity
                            {
                                 StartDate = effectiveFrom,
                                 EndDate = effectiveTo
                            }
                        }
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
                [learnAimRef] = new LearningDelivery
                {
                    LARSValidities = new List<LARSValidity>
                    {
                        new LARSValidity
                        {
                                StartDate = effectiveFrom
                        }
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
        public void FrameworkCodeExistsForFrameworkAims_CaseSensitive_True()
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
                            LearnAimRef = "LEARNAIMREF",
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
            // a tenant of the dictionary is that the key will always point to a lars delivery...
            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                ["One"] = new Mock<LearningDelivery>().Object,
                ["Two"] = new Mock<LearningDelivery>().Object,
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            // let's make it lower case and see if it finds it...
            NewService(externalDataCacheMock.Object).LearnAimRefExists("one").Should().BeTrue();
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
                        LearnDirectClassSystemCode2 = new LearnDirectClassSystemCode(learnDirectClassSystemCode2)
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
                        LearnDirectClassSystemCode2 = new LearnDirectClassSystemCode("CDE")
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

        [Fact]
        public void BasicSkillsMatchForLearnAimRefAndStartDate_False_NullCheck()
        {
            IEnumerable<int> basicSkillsTypes = new List<int>() { 01, 11, 13, 20, 23, 24, 29, 31, 02, 12, 14, 19, 21, 25, 30, 32, 33, 34, 35 };

            Dictionary<string, LearningDelivery> learningDeliveriesDictionary = null;

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object).BasicSkillsMatchForLearnAimRefAndStartDate(basicSkillsTypes, "00100309", new DateTime(2013, 07, 01)).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateGreaterFrameworkThanEffectiveTo_True()
        {
            var effectiveTo = new DateTime(2018, 09, 01);

            var learnStartDate = new DateTime(2018, 10, 01);
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var frameworks = new List<Framework>()
            {
                new Framework()
                {
                    EffectiveTo = effectiveTo,
                    ProgType = progType,
                    FworkCode = fworkCode,
                    PwayCode = pwayCode
                },
                new Framework()
                {
                    EffectiveTo = effectiveTo,
                    ProgType = 15,
                    FworkCode = 16,
                    PwayCode = 0
                },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.Frameworks).Returns(frameworks);

            NewService(externalDataCacheMock.Object).LearnStartDateGreaterThanFrameworkEffectiveTo(learnStartDate, progType, fworkCode, pwayCode).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateGreaterThanFrameworkEffectiveTo_False()
        {
            DateTime? effectiveTo = new DateTime(2018, 11, 01);

            var learnStartDate = new DateTime(2018, 10, 01);
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var frameworks = new List<Framework>()
            {
                new Framework()
                {
                    EffectiveTo = effectiveTo,
                    ProgType = progType,
                    FworkCode = fworkCode,
                    PwayCode = pwayCode
                },
                new Framework()
                {
                    EffectiveTo = effectiveTo,
                    ProgType = 15,
                    FworkCode = 16,
                    PwayCode = 0
                },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.Frameworks).Returns(frameworks);

            NewService(externalDataCacheMock.Object).LearnStartDateGreaterThanFrameworkEffectiveTo(learnStartDate, progType, fworkCode, pwayCode).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateGreaterThanEffectiveTo_FalseNull()
        {
            DateTime? effectiveTo = null;

            var learnStartDate = new DateTime(2018, 10, 01);
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var frameworks = new List<Framework>()
            {
                new Framework()
                {
                    EffectiveTo = effectiveTo,
                    ProgType = progType,
                    FworkCode = fworkCode,
                    PwayCode = pwayCode
                },
                new Framework()
                {
                },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.Frameworks).Returns(frameworks);

            NewService(externalDataCacheMock.Object).LearnStartDateGreaterThanFrameworkEffectiveTo(learnStartDate, null, null, null).Should().BeFalse();
        }

        [Fact]
        public void DD04DateGreaterThanFrameworkAimEffectiveTo_True()
        {
            var dd04Date = new DateTime(2018, 11, 01);
            var effectiveTo = new DateTime(2018, 10, 01);

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
                            EffectiveTo = effectiveTo
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

            NewService(externalDataCacheMock.Object)
                .DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode)
                .Should()
                .BeTrue();
        }

        // why would you introduce case sensitivity into the search when you're trying to remove it?
        /*
        [Fact]
        public void DD04DateGreaterThanFrameworkAimEffectiveTo_CaseSensitive_True()
        {
            var dd04Date = new DateTime(2018, 11, 01);
            var effectiveTo = new DateTime(2018, 10, 01);

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
                            LearnAimRef = "LEARNAIMREF",
                            ProgType = progType,
                            FworkCode = fworkCode,
                            PwayCode = pwayCode,
                            EffectiveTo = effectiveTo
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

            NewService(externalDataCacheMock.Object)
                .DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode)
                .Should()
                .BeTrue();
        }
        */

        [Fact]
        public void DD04DateGreaterThanFrameworkAimEffectiveTo_False()
        {
            var dd04Date = new DateTime(2018, 09, 01);
            var effectiveTo = new DateTime(2018, 10, 01);

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
                            EffectiveTo = effectiveTo
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

            NewService(externalDataCacheMock.Object)
                .DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void DD04DateGreaterThanFrameworkAimEffectiveTo_FalseNull()
        {
            var dd04Date = new DateTime(2018, 09, 01);
            var effectiveTo = new DateTime(2018, 10, 01);

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
                            EffectiveTo = effectiveTo
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

            NewService(externalDataCacheMock.Object)
                .DD04DateGreaterThanFrameworkAimEffectiveTo(null, null, null, null, null)
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData(TypeOfLARSValidity.Apprenticeships)]
        [InlineData(TypeOfLARSValidity.AdultSkills)]
        [InlineData(TypeOfLARSValidity.Unemployed)]
        [InlineData(TypeOfLARSValidity.OLASSAdult)]
        [InlineData(TypeOfLARSValidity.Any)]
        [InlineData(TypeOfLARSValidity.CommunityLearning)]
        [InlineData(TypeOfLARSValidity.EFAConFundEnglish)]
        [InlineData(TypeOfLARSValidity.AdvancedLearnerLoan)]
        [InlineData(TypeOfLARSValidity.EFA16To19)]
        [InlineData(TypeOfLARSValidity.EFAConFundMaths)]
        [InlineData(TypeOfLARSValidity.EuropeanSocialFund)]
        public void OrigLearnStartDateBetweenStartAndEndDateForValidityCategory_True(string larsValidityType)
        {
            var learnAimRef = "123456789";
            DateTime origLearnStartDate = new DateTime(2017, 11, 01);

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LARSValidities = new List<LARSValidity>()
                        {
                            new LARSValidity()
                            {
                                LearnAimRef = learnAimRef,
                                ValidityCategory = larsValidityType,
                                StartDate = new DateTime(2017, 10, 01),
                                EndDate = new DateTime(2018, 10, 01)
                            },
                            new LARSValidity()
                            {
                                LearnAimRef = learnAimRef,
                                ValidityCategory = larsValidityType,
                                StartDate = new DateTime(2016, 10, 01)
                            },
                            new LARSValidity()
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();
            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, larsValidityType)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory_True()
        {
            var learnAimRef = "123456789";
            DateTime origLearnStartDate = new DateTime(2017, 11, 01);

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LARSValidities = new List<LARSValidity>()
                        {
                            new LARSValidity()
                            {
                                LearnAimRef = learnAimRef,
                                ValidityCategory = TypeOfLARSValidity.Apprenticeships,
                                StartDate = new DateTime(2017, 10, 01),
                                EndDate = new DateTime(2018, 10, 01)
                            },
                            new LARSValidity()
                            {
                                LearnAimRef = learnAimRef,
                                ValidityCategory = TypeOfLARSValidity.EFA16To19,
                                StartDate = new DateTime(2016, 10, 01)
                            },
                            new LARSValidity()
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();
            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            var categorryTypesToCheck = new List<string>()
            {
                TypeOfLARSValidity.Apprenticeships,
                TypeOfLARSValidity.OLASSAdult,
                "XYZ"
            };

            NewService(externalDataCacheMock.Object)
                .OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(origLearnStartDate, learnAimRef, categorryTypesToCheck)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory_False()
        {
            var learnAimRef = "123456789";
            DateTime origLearnStartDate = new DateTime(2017, 11, 01);

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LARSValidities = new List<LARSValidity>()
                        {
                            new LARSValidity()
                            {
                                LearnAimRef = learnAimRef,
                                ValidityCategory = "ABC",
                                StartDate = new DateTime(2017, 10, 01),
                                EndDate = new DateTime(2018, 10, 01)
                            },
                            new LARSValidity()
                            {
                                LearnAimRef = learnAimRef,
                                ValidityCategory = "YYYYYY",
                                StartDate = new DateTime(2016, 10, 01)
                            },
                            new LARSValidity()
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();
            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            var categorryTypesToCheck = new List<string>()
            {
                TypeOfLARSValidity.Apprenticeships,
                TypeOfLARSValidity.OLASSAdult,
            };

            NewService(externalDataCacheMock.Object)
                .OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(origLearnStartDate, learnAimRef, categorryTypesToCheck)
                .Should()
                .BeFalse();
        }

        [Fact(Skip = "no longer relevant, incoming original learn start date cannot be null")]
        public void OrigLearnStartDateBetweenStartAndEndDateForValidityCategory_FalseNullOrigLearnStartDate()
        {
            var learnAimRef = "123456789";
            DateTime origLearnStartDate = DateTime.MaxValue; // = null;

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LARSValidities = new List<LARSValidity>()
                        {
                            new LARSValidity()
                            {
                                LearnAimRef = learnAimRef,
                                ValidityCategory = TypeOfLARSValidity.Apprenticeships,
                                StartDate = new DateTime(2017, 10, 01),
                                EndDate = new DateTime(2018, 10, 01)
                            },
                            new LARSValidity()
                            {
                                LearnAimRef = learnAimRef,
                                ValidityCategory = TypeOfLARSValidity.Apprenticeships,
                                StartDate = new DateTime(2016, 10, 01)
                            },
                            new LARSValidity()
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();
            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, TypeOfLARSValidity.Apprenticeships)
                .Should()
                .BeFalse();
        }

        [Fact(Skip = "with correct code this test was alway due to fail, as the second period is open ended")]
        public void OrigLearnStartDateBetweenStartAndEndDateForValidityCategory_FalseOrigLearnStartDateNotInRange()
        {
            var learnAimRef = "123456789";
            DateTime origLearnStartDate = new DateTime(2018, 11, 01);

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LARSValidities = new List<LARSValidity>()
                        {
                            new LARSValidity()
                            {
                                LearnAimRef = learnAimRef,
                                ValidityCategory = TypeOfLARSValidity.Apprenticeships,
                                StartDate = new DateTime(2017, 10, 01),
                                EndDate = new DateTime(2018, 10, 01)
                            },
                            new LARSValidity()
                            {
                                LearnAimRef = learnAimRef,
                                ValidityCategory = TypeOfLARSValidity.Apprenticeships,
                                StartDate = new DateTime(2016, 10, 01)
                            },
                            new LARSValidity()
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();
            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, TypeOfLARSValidity.Apprenticeships)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void OrigLearnStartDateBetweenStartAndEndDateForValidityCategory_FalseCategoryMisMatch()
        {
            var learnAimRef = "123456789";
            DateTime origLearnStartDate = new DateTime(2018, 09, 01);

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LARSValidities = new List<LARSValidity>()
                        {
                            new LARSValidity()
                            {
                                LearnAimRef = learnAimRef,
                                ValidityCategory = "XXX",
                                StartDate = new DateTime(2017, 10, 01),
                                EndDate = new DateTime(2018, 10, 01)
                            },
                            new LARSValidity()
                            {
                                LearnAimRef = learnAimRef,
                                ValidityCategory = "XXX",
                                StartDate = new DateTime(2016, 10, 01)
                            },
                            new LARSValidity()
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();
            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, TypeOfLARSValidity.Apprenticeships)
                .Should()
                .BeFalse();
        }

        [Fact(Skip = "invalid, validity learn aim ref is a foreign key")]
        public void OrigLearnStartDateBetweenStartAndEndDateForValidityCategory_FalseLearnAimRefMisMatch()
        {
            var learnAimRef = "123456789";
            DateTime origLearnStartDate = new DateTime(2018, 09, 01);

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LARSValidities = new List<LARSValidity>()
                        {
                            new LARSValidity()
                            {
                                LearnAimRef = "123",
                                ValidityCategory = TypeOfLARSValidity.Apprenticeships,
                                StartDate = new DateTime(2017, 10, 01),
                                EndDate = new DateTime(2018, 10, 01)
                            },
                            new LARSValidity()
                            {
                                LearnAimRef = "123",
                                ValidityCategory = TypeOfLARSValidity.Apprenticeships,
                                StartDate = new DateTime(2016, 10, 01)
                            },
                            new LARSValidity()
                        }
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();
            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, TypeOfLARSValidity.Apprenticeships)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void OrigLearnStartDateBetweenStartAndEndDateForValidityCategory_FalseNull()
        {
            var learnAimRef = "123456789";
            DateTime origLearnStartDate = new DateTime(2017, 11, 01);

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>();

            var externalDataCacheMock = new Mock<IExternalDataCache>();
            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, TypeOfLARSValidity.Apprenticeships)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void FrameWorkComponentTypeExistsInFrameworkAims_False()
        {
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };
            var learnAimRef = "ZESF12345";

            var frameworks = new List<Framework>()
            {
                new Framework()
                {
                    FrameworkAims = new List<FrameworkAim>()
                    {
                        new FrameworkAim()
                        {
                            LearnAimRef = "ZESF12345",
                            FrameworkComponentType = 5
                        },
                        new FrameworkAim()
                        {
                            LearnAimRef = "ZESF12345",
                            FrameworkComponentType = 2
                        },
                        new FrameworkAim()
                        {
                            LearnAimRef = "ZESF12345",
                            FrameworkComponentType = 6
                        },
                        new FrameworkAim()
                        {
                            LearnAimRef = "ZESF99887",
                            FrameworkComponentType = 3
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
            NewService(externalDataCache: externalDataCacheMock.Object)
                .FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void FrameWorkComponentTypeExistsInFrameworkAims_FalseNull()
        {
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };
            var learnAimRef = "ZESF12345";

            var frameworks = new List<Framework>()
            {
                new Framework()
                {
                    FrameworkAims = null
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.Frameworks).Returns(frameworks);
            NewService(externalDataCache: externalDataCacheMock.Object)
                .FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void FrameWorkComponentTypeExistsInFrameworkAims_False_FrameworkNull()
        {
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };
            var learnAimRef = "ZESF12345";

            List<Framework> frameworks = null;

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.Frameworks).Returns(frameworks);
            NewService(externalDataCache: externalDataCacheMock.Object)
                .FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void FrameWorkComponentTypeExistsInFrameworkAims_True()
        {
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };
            var learnAimRef = "ZESF12345";

            var frameworks = new List<Framework>()
            {
                new Framework()
                {
                    FrameworkAims = new List<FrameworkAim>()
                    {
                        new FrameworkAim()
                        {
                            LearnAimRef = "ZESF12345",
                            FrameworkComponentType = 1
                        },
                        new FrameworkAim()
                        {
                            LearnAimRef = "ZESF12345",
                            FrameworkComponentType = 2
                        },
                        new FrameworkAim()
                        {
                            LearnAimRef = "ZESF12345",
                            FrameworkComponentType = 3
                        },
                        new FrameworkAim()
                        {
                            LearnAimRef = "ZESF99887",
                            FrameworkComponentType = 3
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
            NewService(externalDataCache: externalDataCacheMock.Object)
                .FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EnglishPrescribedIdsExistsforLearnAimRef_False()
        {
            string learnAimRef = "ESF123456";
            HashSet<int?> englishPrescribedIDs = new HashSet<int?>() { 1, 2 };
            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        EnglPrscID = 3
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(e => e.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCache: externalDataCacheMock.Object)
                .EnglishPrescribedIdsExistsforLearnAimRef(learnAimRef, englishPrescribedIDs).Should().BeFalse();
        }

        [Fact]
        public void EnglishPrescribedIdsExistsforLearnAimRef_True()
        {
            string learnAimRef = "ESF123456";
            HashSet<int?> englishPrescribedIDs = new HashSet<int?>() { 1, 2 };
            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef, new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        EnglPrscID = 1
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(e => e.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCache: externalDataCacheMock.Object)
                .EnglishPrescribedIdsExistsforLearnAimRef(learnAimRef, englishPrescribedIDs).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateGreaterThanStandardsEffectiveTo_FalseNoMatch()
        {
            int stdCode = 1;
            var learnStartDate = new DateTime(2018, 10, 01);

            var standards = new List<LARSStandard>()
            {
                new LARSStandard()
                {
                    StandardCode = 2,
                    StandardSectorCode = "3",
                    NotionalEndLevel = "4",
                    EffectiveTo = new DateTime(2018, 01, 01),
                    EffectiveFrom = new DateTime(2017, 01, 01)
                },
                new LARSStandard()
                {
                    StandardCode = 3,
                    StandardSectorCode = "3",
                    NotionalEndLevel = "4",
                    EffectiveTo = new DateTime(2018, 01, 01),
                    EffectiveFrom = new DateTime(2017, 01, 01)
                },
                new LARSStandard()
                {
                    StandardCode = 4
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(dc => dc.Standards).Returns(standards);

            NewService(externalDataCacheMock.Object).LearnStartDateGreaterThanStandardsEffectiveTo(stdCode, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateGreaterThanStandardsEffectiveTo_FalseNoStandards()
        {
            int stdCode = 1;
            var learnStartDate = new DateTime(2018, 10, 01);

            var standards = new List<LARSStandard>();

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(dc => dc.Standards).Returns(standards);

            NewService(externalDataCacheMock.Object).LearnStartDateGreaterThanStandardsEffectiveTo(stdCode, learnStartDate).Should().BeFalse();
        }

        [Fact(Skip = "standard code no longer be null in this instance, as it's a 'key'")]
        public void LearnStartDateGreaterThanStandardsEffectiveTo_FalseNullSTDCode()
        {
            int stdCode = 0; // = null;
            var learnStartDate = new DateTime(2018, 10, 01);

            var standards = new List<LARSStandard>()
            {
                new LARSStandard()
                {
                    StandardCode = 2,
                    StandardSectorCode = "3",
                    NotionalEndLevel = "4",
                    EffectiveTo = new DateTime(2018, 01, 01),
                    EffectiveFrom = new DateTime(2017, 01, 01)
                },
                new LARSStandard()
                {
                    StandardCode = 3,
                    StandardSectorCode = "3",
                    NotionalEndLevel = "4",
                    EffectiveTo = new DateTime(2018, 01, 01),
                    EffectiveFrom = new DateTime(2017, 01, 01)
                },
                new LARSStandard()
                {
                    StandardCode = 4
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(dc => dc.Standards).Returns(standards);

            NewService(externalDataCacheMock.Object).LearnStartDateGreaterThanStandardsEffectiveTo(stdCode, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateGreaterThanStandardsEffectiveTo_True()
        {
            int stdCode = 1;
            var learnStartDate = new DateTime(2017, 10, 01);

            var standards = new List<LARSStandardValidity>()
            {
                new LARSStandardValidity()
                {
                    StandardCode = 1,
                    StartDate = new DateTime(2017, 01, 01),
                    EndDate = new DateTime(2018, 01, 01)
                },
                new LARSStandardValidity()
                {
                    StandardCode = 3,
                    StartDate = new DateTime(2017, 01, 01),
                    EndDate = new DateTime(2018, 01, 01)
                },
                new LARSStandardValidity()
                {
                    StandardCode = 4,
                    StartDate = new DateTime(2020, 01, 01),
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(dc => dc.StandardValidities).Returns(standards);

            NewService(externalDataCacheMock.Object).LearnStartDateGreaterThanStandardsEffectiveTo(stdCode, learnStartDate).Should().BeTrue();
        }

        /// <summary>
        /// Gets the notional NVQ levelv2 for learn aim reference.
        /// and also (not by design) conducts a case insensitive request (test) from the cache...
        /// </summary>
        [Fact]
        public void GetNotionalNVQLevelv2ForLearnAimRef()
        {
            string learnAimRef = "ESF123456";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    "esf123456",
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        NotionalNVQLevelv2 = "1"
                    }
                },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(e => e.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCache: externalDataCacheMock.Object)
                .GetNotionalNVQLevelv2ForLearnAimRef(learnAimRef).Should().Be("1");
        }

        [Fact]
        public void GetNotionalNVQLevelv2ForLearnAimRef_NullCheck()
        {
            string learnAimRef = "ESF09876";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    "esf223344",
                    new LearningDelivery()
                    {
                        LearnAimRef = "esf223344",
                        NotionalNVQLevelv2 = "223"
                    }
                },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(e => e.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCache: externalDataCacheMock.Object).GetNotionalNVQLevelv2ForLearnAimRef(learnAimRef).Should().BeNullOrEmpty();
        }

        [Fact]
        public void HasAnyLearningDeliveryForLearnAimRefAndTypes_True()
        {
            var learnAimRefTypes = new[] { "1111", "2222", "3333" };
            var learnAimRef = "123456789";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LearnAimRefType = "2222"
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();
            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, learnAimRefTypes)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void HasAnyLearningDeliveryForLearnAimRefAndTypes_False_NullType()
        {
            var learnAimRefTypes = new[] { "1111", "2222", "3333" };
            var learnAimRef = "123456789";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();
            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, learnAimRefTypes)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryForLearnAimRefAndTypes_False_NoMatch()
        {
            var learnAimRefTypes = new[] { "1111", "2222", "3333" };
            var learnAimRef = "123456789";

            var learningDeliveriesDictionary = new Dictionary<string, LearningDelivery>()
            {
                {
                    learnAimRef,
                    new LearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LearnAimRefType = "4444"
                    }
                }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();
            externalDataCacheMock.SetupGet(c => c.LearningDeliveries).Returns(learningDeliveriesDictionary);

            NewService(externalDataCacheMock.Object)
                .HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, learnAimRefTypes)
                .Should()
                .BeFalse();
        }

        private LARSDataService NewService(IExternalDataCache externalDataCache = null)
        {
            return new LARSDataService(externalDataCache);
        }
    }
}