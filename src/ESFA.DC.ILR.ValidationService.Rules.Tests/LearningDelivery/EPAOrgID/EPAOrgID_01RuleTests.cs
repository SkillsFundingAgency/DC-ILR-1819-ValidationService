using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EPAOrgID;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.EPAOrgID
{
    public class EPAOrgID_01RuleTests : AbstractRuleTests<EPAOrgID_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EPAOrgID_01");
        }

        [Theory]
        [InlineData(null, 1, "2018-08-01", false, false)]
        [InlineData("EPA001", null, "2018-08-01", false, false)]
        [InlineData(null, null, "2018-08-01", false, false)]
        [InlineData("EPA001", 1, "2018-08-01", true, false)]
        [InlineData("EPA001", 1, "2018-08-01", false, true)]
        public void ConditionMet_False(string epaOrgId, int? stdCode, string learnPlanEndDateString, bool mockValue, bool assertion)
        {
            var learnPlanEndDate = DateTime.Parse(learnPlanEndDateString);

            var epaOrganisationsMock = new Mock<IEPAOrganisationDataService>();

            epaOrganisationsMock.Setup(ds => ds.IsValidEpaOrg(epaOrgId, stdCode, learnPlanEndDate)).Returns(mockValue);

            NewRule(epaOrganisationsMock.Object).ConditionMet(epaOrgId, stdCode, learnPlanEndDate).Should().Be(assertion);
        }

        [Fact]
        public void ValidateError()
        {
            var epaOrgId = "epa1";
            int? stdCode = 1;
            var learnPlanEndDate = new DateTime(2018, 8, 1);

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EPAOrgID = epaOrgId,
                        StdCodeNullable = stdCode,
                        LearnPlanEndDate = learnPlanEndDate
                    }
                }
            };

            var epaOrganisationsMock = new Mock<IEPAOrganisationDataService>();

            epaOrganisationsMock.Setup(ds => ds.IsValidEpaOrg(epaOrgId, stdCode, learnPlanEndDate)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(epaOrganisationsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(3));
            }
        }

        [Fact]
        public void ValidateError_Multiple()
        {
            var epaOrgId = "epa1";
            int? stdCode = 1;
            var learnPlanEndDate = new DateTime(2018, 8, 1);

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EPAOrgID = epaOrgId,
                        StdCodeNullable = stdCode,
                        LearnPlanEndDate = learnPlanEndDate
                    },
                    new TestLearningDelivery()
                    {
                        EPAOrgID = epaOrgId,
                        StdCodeNullable = stdCode,
                        LearnPlanEndDate = learnPlanEndDate
                    }
                }
            };

            var epaOrganisationsMock = new Mock<IEPAOrganisationDataService>();

            epaOrganisationsMock.Setup(ds => ds.IsValidEpaOrg(epaOrgId, stdCode, learnPlanEndDate)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(epaOrganisationsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(6));
            }
        }

        [Fact]
        public void Validate_NoError_NoDeliveries()
        {
            var learner = new TestLearner();

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_EpaOrgIdIsValid()
        {
            var epaOrgId = "epa1";
            int? stdCode = 1;
            var learnPlanEndDate = new DateTime(2018, 8, 1);

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EPAOrgID = epaOrgId,
                        StdCodeNullable = stdCode,
                        LearnPlanEndDate = learnPlanEndDate
                    },
                    new TestLearningDelivery()
                    {
                        EPAOrgID = epaOrgId,
                        StdCodeNullable = stdCode,
                        LearnPlanEndDate = learnPlanEndDate
                    }
                }
            };

            var epaOrganisationsMock = new Mock<IEPAOrganisationDataService>();

            epaOrganisationsMock.Setup(ds => ds.IsValidEpaOrg(epaOrgId, stdCode, learnPlanEndDate)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(epaOrganisationsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_StdCodeNull()
        {
            var epaOrgId = "epa1";
            int? stdCode = null;
            var learnPlanEndDate = new DateTime(2018, 8, 1);

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EPAOrgID = epaOrgId,
                        LearnPlanEndDate = learnPlanEndDate
                    }
                }
            };

            var epaOrganisationsMock = new Mock<IEPAOrganisationDataService>();

            epaOrganisationsMock.Setup(ds => ds.IsValidEpaOrg(epaOrgId, stdCode, learnPlanEndDate)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(epaOrganisationsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_EpaOrgIdNull()
        {
            string epaOrgId = null;
            int? stdCode = null;
            var learnPlanEndDate = new DateTime(2018, 8, 1);

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        StdCodeNullable = stdCode,
                        LearnPlanEndDate = learnPlanEndDate
                    }
                }
            };

            var epaOrganisationsMock = new Mock<IEPAOrganisationDataService>();

            epaOrganisationsMock.Setup(ds => ds.IsValidEpaOrg(epaOrgId, stdCode, learnPlanEndDate)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(epaOrganisationsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var epaOrgId = "X";
            int? stdCode = 1;
            var learnPlanEndDate = new DateTime(2019, 8, 1);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("EPAOrgID", "X")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("StdCode", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnPlanEndDate", "01/08/2019")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(epaOrgId, stdCode, learnPlanEndDate);

            validationErrorHandlerMock.Verify();
        }

        private EPAOrgID_01Rule NewRule(
            IEPAOrganisationDataService epaOrganisationDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new EPAOrgID_01Rule(epaOrganisationDataService, validationErrorHandler);
        }
    }
}