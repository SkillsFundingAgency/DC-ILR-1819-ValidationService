using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;
using DateTime = System.DateTime;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_17RuleTests
    {
        [Fact]
        public void IsCapMoreThanTotalStandardsValue_True()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(x => x.GetCoreGovContributionCapForStandard(2, DateTime.MinValue)).Returns(33.333m);

            NewRule(larsDataServiceMock.Object).IsAFilTotalMoreThanCapValue(2, 51, DateTime.MinValue).Should().BeTrue();
        }

        [Fact]
        public void IsCapMoreThanTotalStandardsValue_False_NullCap()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(x => x.GetCoreGovContributionCapForStandard(It.IsAny<int>(), DateTime.MinValue)).Returns((decimal?)null);

            NewRule(larsDataServiceMock.Object).IsAFilTotalMoreThanCapValue(It.IsAny<int>(), It.IsAny<int>(), DateTime.MinValue).Should().BeFalse();
        }

        [Fact]
        public void IsCapMoreThanTotalStandardsValue_False_NullDateTime()
        {
            NewRule().IsAFilTotalMoreThanCapValue(2, 51, null).Should().BeFalse();
        }

        [Fact]
        public void IsCapMoreThanTotalStandardsValue_False()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(x => x.GetCoreGovContributionCapForStandard(2, DateTime.MinValue)).Returns(33.333m);

            NewRule(larsDataServiceMock.Object).IsAFilTotalMoreThanCapValue(2, 49, DateTime.MinValue).Should().BeFalse();
        }

        [Fact]
        public void IsCapMoreThanTotalStandardsValue_Trueuccess()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(x => x.GetCoreGovContributionCapForStandard(2, DateTime.MinValue)).Returns(33.333m);

            NewRule(larsDataServiceMock.Object).IsAFilTotalMoreThanCapValue(2, 51, DateTime.MinValue).Should().BeTrue();
        }

        [Fact]
        public void GetAFinTotalValues_NUll_LearningDeliveries()
        {
            NewRule().GetAFinTotalValues(null).Should().BeEmpty();
        }

        [Fact]
        public void GetAFinTotalValues_Success()
        {
            var appFinRecord1 = new TestAppFinRecord
            {
                AFinCode = 1,
                AFinType = "TNP",
                AFinAmount = 5
            };

            var appFinRecord2 = new TestAppFinRecord
            {
                AFinCode = 2,
                AFinType = "TNP",
                AFinAmount = 10
            };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        appFinRecord1,
                        appFinRecord2
                    }
                },
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        appFinRecord1,
                        appFinRecord2
                    }
                },
                new TestLearningDelivery()
                {
                    StdCodeNullable = 3,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        appFinRecord2
                    }
                },
                new TestLearningDelivery()
                {
                    StdCodeNullable = 4
                }
            };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(x => x.GetLatestAppFinRecord(It.IsAny<List<IAppFinRecord>>(), "TNP", 1)).Returns(appFinRecord1);

            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(x => x.GetLatestAppFinRecord(It.IsAny<List<IAppFinRecord>>(), "TNP", 2)).Returns(appFinRecord2);

            var result = NewRule(null, learningDeliveryAppFinRecordQueryServiceMock.Object)
                .GetAFinTotalValues(learningDeliveries);

            result.Should().NotBeEmpty();
            result.Keys.Count.Should().Be(2);
            result.ContainsKey(4).Should().BeFalse();
            result[1].Should().Be(30);
            result[3].Should().Be(15);
        }

        [Fact]
        public void GetApplicableDateForCapChecking_Success_NoOrigStartDate()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    LearnStartDate = new DateTime(2017, 10, 10)
                },
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    LearnStartDate = new DateTime(2018, 10, 10)
                },
                new TestLearningDelivery()
                {
                    StdCodeNullable = 2,
                    LearnStartDate = new DateTime(2019, 10, 10)
                },
            };

            var rule = NewRule();
            rule.GetApplicableDateForCapChecking(learningDeliveries, 1).Should().Be(new DateTime(2017, 10, 10));
            rule.GetApplicableDateForCapChecking(learningDeliveries, 2).Should().Be(new DateTime(2019, 10, 10));
        }

        [Fact]
        public void GetApplicableDateForCapChecking_Success_EarlierOrigStartDate()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    LearnStartDate = new DateTime(2017, 10, 10),
                    OrigLearnStartDateNullable = new DateTime(2015, 10, 10)
                },
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    LearnStartDate = new DateTime(2018, 10, 10)
                },
                new TestLearningDelivery()
                {
                    StdCodeNullable = 2,
                    LearnStartDate = new DateTime(2018, 10, 10),
                    OrigLearnStartDateNullable = new DateTime(2020, 10, 10)
                },
                new TestLearningDelivery()
                {
                    StdCodeNullable = 2,
                    LearnStartDate = new DateTime(2018, 10, 10),
                    OrigLearnStartDateNullable = new DateTime(2016, 10, 10)
                },
            };

            var rule = NewRule();
            rule.GetApplicableDateForCapChecking(learningDeliveries, 1).Should().Be(new DateTime(2015, 10, 10));
            rule.GetApplicableDateForCapChecking(learningDeliveries, 2).Should().Be(new DateTime(2016, 10, 10));
        }

        [Fact]
        public void IsTotalNegotiatedPriceMoreThanCapForStandards_False_NullLearningDeliveries()
        {
            NewRule().IsTotalNegotiatedPriceMoreThanCapForStandards(null).Should().BeFalse();
        }

        [Theory]
        [InlineData(null, 25, 81, 1)]
        [InlineData(1, 100, 81, 1)]
        [InlineData(1, 25, 100, 1)]
        [InlineData(1, 25, 81, 3)]
        public void IsTotalNegotiatedPriceMoreThanCapForStandards_False_NoApplicableLearningDeliveries(int? standardCode, int? progType, int fundModel, int aimType)
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 100,
                    AimType = TypeOfAim.CoreAim16To19ExcludingApprenticeships,
                    FundModel = 10
                },
                new TestLearningDelivery()
                {
                    StdCodeNullable = standardCode,
                    AimType = aimType,
                    ProgTypeNullable = progType,
                    FundModel = fundModel
                },
            };

            NewRule().IsTotalNegotiatedPriceMoreThanCapForStandards(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void IsTotalNegotiatedPriceMoreThanCapForStandards_True()
        {
            var appFinRecord1 = new TestAppFinRecord
            {
                AFinCode = 1,
                AFinType = "TNP",
                AFinAmount = 15
            };

            var appFinRecord2 = new TestAppFinRecord
            {
                AFinCode = 2,
                AFinType = "TNP",
                AFinAmount = 10
            };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 50,
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    FundModel = 81,
                    ProgTypeNullable = 25,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        appFinRecord1,
                        appFinRecord2
                    }
                },
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    AimType = TypeOfAim.ProgrammeAim,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        appFinRecord1,
                        appFinRecord2
                    }
                },
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    AimType = TypeOfAim.ProgrammeAim,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        appFinRecord1,
                        appFinRecord2
                    }
                }
           };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(x => x.GetLatestAppFinRecord(It.IsAny<List<IAppFinRecord>>(), "TNP", 1)).Returns(appFinRecord1);

            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(x => x.GetLatestAppFinRecord(It.IsAny<List<IAppFinRecord>>(), "TNP", 2)).Returns(appFinRecord2);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(x => x.GetCoreGovContributionCapForStandard(1, It.IsAny<DateTime>())).Returns(33.00m);

            var result = NewRule(larsDataServiceMock.Object, learningDeliveryAppFinRecordQueryServiceMock.Object)
                .IsTotalNegotiatedPriceMoreThanCapForStandards(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void IsTotalNegotiatedPriceMoreThanCapForStandards_False()
        {
            var appFinRecord1 = new TestAppFinRecord
            {
                AFinCode = 1,
                AFinType = "TNP",
                AFinAmount = 15
            };

            var appFinRecord2 = new TestAppFinRecord
            {
                AFinCode = 2,
                AFinType = "TNP",
                AFinAmount = 10
            };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    AimType = TypeOfAim.ProgrammeAim,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        appFinRecord1,
                        appFinRecord2
                    }
                }
           };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(x => x.GetLatestAppFinRecord(It.IsAny<List<IAppFinRecord>>(), "TNP", 1)).Returns(appFinRecord1);

            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(x => x.GetLatestAppFinRecord(It.IsAny<List<IAppFinRecord>>(), "TNP", 2)).Returns(appFinRecord2);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(x => x.GetCoreGovContributionCapForStandard(2, DateTime.MinValue)).Returns(33.3333m);

            var result = NewRule(larsDataServiceMock.Object, learningDeliveryAppFinRecordQueryServiceMock.Object)
                .IsTotalNegotiatedPriceMoreThanCapForStandards(learningDeliveries).Should().BeFalse();
        }

        private DerivedData_17Rule NewRule(
            ILARSDataService larsDataService = null,
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService = null)
        {
            return new DerivedData_17Rule(larsDataService, learningDeliveryAppFinRecordQueryService);
        }
    }
}
