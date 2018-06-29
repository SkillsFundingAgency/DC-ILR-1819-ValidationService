using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PartnerUKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.PartnerUKPRN
{
    public class PartnerUKPRN_01RuleTests : AbstractRuleTests<PartnerUKPRN_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PartnerUKPRN_01");
        }

        [Fact]
        public void NullConditionMet_True()
        {
            NewRule().NullConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void NullConditionMet_False()
        {
            NewRule().NullConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LookupConditionMet_False()
        {
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();

            organisationDataServiceMock.Setup(ds => ds.IsPartnerUkprn(1)).Returns(true);

            NewRule(organisationDataServiceMock.Object).LookupConditionMet(1).Should().BeFalse();
        }

        [Fact]
        public void LookupConditionMet_True()
        {
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();

            organisationDataServiceMock.Setup(ds => ds.IsPartnerUkprn(1)).Returns(false);

            NewRule(organisationDataServiceMock.Object).LookupConditionMet(2).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();

            organisationDataServiceMock.Setup(ds => ds.IsPartnerUkprn(1)).Returns(true);

            NewRule(organisationDataServiceMock.Object).ConditionMet(1).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();

            organisationDataServiceMock.Setup(ds => ds.IsPartnerUkprn(1)).Returns(false);

            NewRule(organisationDataServiceMock.Object).ConditionMet(2).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NullUkprn()
        {
            NewRule().ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                    {
                        PartnerUKPRNNullable = 1
                    }
                }
            };

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();

            organisationDataServiceMock.Setup(ds => ds.IsPartnerUkprn(1)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(organisationDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                    {
                        PartnerUKPRNNullable = 1
                    }
                }
            };

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();

            organisationDataServiceMock.Setup(ds => ds.IsPartnerUkprn(1)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(organisationDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            long? partnerUKPRN = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PartnerUKPRN", partnerUKPRN)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(partnerUKPRN);

            validationErrorHandlerMock.Verify();
        }

        private PartnerUKPRN_01Rule NewRule(IOrganisationDataService organisationDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PartnerUKPRN_01Rule(organisationDataService, validationErrorHandler);
        }
    }
}
