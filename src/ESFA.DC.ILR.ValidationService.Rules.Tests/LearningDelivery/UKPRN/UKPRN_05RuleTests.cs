using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.UKPRN
{
    public class UKPRN_05RuleTests : AbstractRuleTests<UKPRN_05Rule>
    {
        private readonly int _fundModel = 70;
        private readonly IEnumerable<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
            FundingStreamPeriodCodeConstants.ESF1420
        };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("UKPRN_05");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("2018-08-01")]
        public void LearnActEndDateConditionMet_True(string learnActEndDateString)
        {
            DateTime? learnActEndDate = learnActEndDateString != null ? DateTime.Parse(learnActEndDateString) : (DateTime?)null;

            DateTime academicYear = new DateTime(2018, 8, 1);

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            if (learnActEndDate != null)
            {
                academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(false);
            }

            NewRule(academicYearQueryService: academicYearQueryServiceMock.Object).LearnActEndDateConditionMet(learnActEndDate, academicYear).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateConditionMet_False()
        {
            DateTime? learnActEndDate = new DateTime(2018, 5, 1);
            DateTime academicYear = new DateTime(2018, 8, 1);

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(true);

            NewRule(academicYearQueryService: academicYearQueryServiceMock.Object).LearnActEndDateConditionMet(learnActEndDate, academicYear).Should().BeFalse();
        }

        [Theory]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(true, true, false)]
        [InlineData(false, false, true)]
        public void FCTFundingConditionMet(bool fcsMock1, bool fcsMock2, bool assertion)
        {
            var conRefNumber = "ConRef";
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(ds => ds.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(fcsMock1);
            fcsDataServiceMock.Setup(ds => ds.ConRefNumberExists(conRefNumber)).Returns(fcsMock2);

            NewRule(fCSDataService: fcsDataServiceMock.Object).FCTFundingConditionMet(conRefNumber).Should().Be(assertion);
        }

        [Fact]
        public void ConditionMet_True()
        {
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime? learnActEndDate = new DateTime(2018, 8, 1);
            var conRefNumber = "ConRef";

            var rule = NewRuleMock();

            rule.Setup(r => r.LearnActEndDateConditionMet(learnActEndDate, academicYear)).Returns(true);
            rule.Setup(r => r.FCTFundingConditionMet(conRefNumber)).Returns(true);

            rule.Object.ConditionMet(academicYear, learnActEndDate, conRefNumber).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void ConditionMet_False(bool condition1, bool condition2)
        {
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime? learnActEndDate = new DateTime(2018, 8, 1);
            var conRefNumber = "ConRef";

            var rule = NewRuleMock();

            rule.Setup(r => r.LearnActEndDateConditionMet(learnActEndDate, academicYear)).Returns(condition1);
            rule.Setup(r => r.FCTFundingConditionMet(conRefNumber)).Returns(condition2);

            rule.Object.ConditionMet(academicYear, learnActEndDate, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            int ukprn = 12345678;
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime? learnActEndDate = new DateTime(2018, 8, 1);
            string conRefNumber = "ConRef";

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = _fundModel,
                        LearnStartDate = startDate,
                        LearnActEndDateNullable = learnActEndDate,
                        ConRefNumber = conRefNumber
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            if (learnActEndDate != null)
            {
                academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(false);
            };
            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(startDate);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(false);
            fcsDataServiceMock.Setup(qs => qs.ConRefNumberExists(conRefNumber)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    fcsDataServiceMock.Object,
                    academicYearDataServiceMock.Object,
                    academicYearQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            int ukprn = 12345678;
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime? learnActEndDate = new DateTime(2018, 8, 1);
            string conRefNumber = "ConRef";

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = _fundModel,
                        LearnStartDate = startDate,
                        LearnActEndDateNullable = learnActEndDate,
                        ConRefNumber = conRefNumber
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            if (learnActEndDate != null)
            {
                academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(false);
            };
            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(startDate);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(true);
            fcsDataServiceMock.Setup(qs => qs.ConRefNumberExists(conRefNumber)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    fcsDataServiceMock.Object,
                    academicYearDataServiceMock.Object,
                    academicYearQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var conRefNumber = "ConRef";

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, _fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, conRefNumber)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(_fundModel, conRefNumber);

            validationErrorHandlerMock.Verify();
        }

        private UKPRN_05Rule NewRule(
            IFCSDataService fCSDataService = null,
            IAcademicYearDataService academicYearDataService = null,
            IAcademicYearQueryService academicYearQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new UKPRN_05Rule(fCSDataService, academicYearDataService, academicYearQueryService, validationErrorHandler);
        }
    }
}
