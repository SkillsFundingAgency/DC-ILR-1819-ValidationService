using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.CampId;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.CampId
{
    public class CampId_01RuleTests : AbstractRuleTests<CampId_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("CampId_01");
        }

        [Theory]
        [InlineData("Camp1")]
        public void ConditionMet_True(string campId)
        {
            var organisationDataServiceMockup = new Mock<IOrganisationDataService>();

            organisationDataServiceMockup.Setup(p => p.CampIdExists(campId)).Returns(false);

            NewRule(organisationDataService: organisationDataServiceMockup.Object).ConditionMet(campId).Should().BeTrue();
        }

        [Theory]
        [InlineData("Camp2")]
        public void ConditionMet_False(string campId)
        {
            var organisationDataServiceMockup = new Mock<IOrganisationDataService>();

            organisationDataServiceMockup.Setup(p => p.CampIdExists(campId)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, organisationDataService: organisationDataServiceMockup.Object).ConditionMet(campId).Should().BeFalse();
            }
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner()
            {
                CampId = "camp1"
            };

            var organisationDataServiceMockup = new Mock<IOrganisationDataService>();

            organisationDataServiceMockup.Setup(p => p.CampIdExists("camp1")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, organisationDataService: organisationDataServiceMockup.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearner = new TestLearner()
            {
                CampId = "camp2"
            };

            var organisationDataServiceMockup = new Mock<IOrganisationDataService>();

            organisationDataServiceMockup.Setup(p => p.CampIdExists("camp2")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, organisationDataService: organisationDataServiceMockup.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            string campId = "camp3";

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.CampId, campId)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(campId);

            validationErrorHandlerMock.Verify();
        }

        private CampId_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IOrganisationDataService organisationDataService = null)
        {
            return new CampId_01Rule(organisationDataService: organisationDataService, validationErrorHandler: validationErrorHandler);
        }
    }
}
