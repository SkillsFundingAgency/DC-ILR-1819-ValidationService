using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.CampId;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.CampId
{
    public class CampId_02RuleTests : AbstractRuleTests<CampId_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("CampId_02");
        }

        [Theory]
        [InlineData("Camp1", 10023139)]
        public void ConditionMet_True(string campId, long ukprn)
        {
            var organisationDataServiceMockup = new Mock<IOrganisationDataService>();

            organisationDataServiceMockup.Setup(p => p.CampIdMatchForUkprn(campId, ukprn)).Returns(false);

            NewRule(organisationDataService: organisationDataServiceMockup.Object).ConditionMet(campId, ukprn).Should().BeTrue();
        }

        [Theory]
        [InlineData("Camp2", 10023139)]
        public void ConditionMet_False(string campId, long ukprn)
        {
            var organisationDataServiceMockup = new Mock<IOrganisationDataService>();

            organisationDataServiceMockup.Setup(p => p.CampIdMatchForUkprn(campId, ukprn)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, organisationDataService: organisationDataServiceMockup.Object).ConditionMet(campId, ukprn).Should().BeFalse();
            }
        }

        [Fact]
        public void Validate_Error()
        {
            int ukprn = 10023139;

            var testLearner = new TestLearner()
            {
                CampId = "camp1"
            };

            var fileDataCacheMock = new Mock<IFileDataCache>();

            fileDataCacheMock.SetupGet(c => c.UKPRN).Returns(ukprn);

            var organisationDataServiceMockup = new Mock<IOrganisationDataService>();

            organisationDataServiceMockup.Setup(p => p.CampIdMatchForUkprn("camp1", ukprn)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, organisationDataService: organisationDataServiceMockup.Object, fileDataCache: fileDataCacheMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            int ukprn = 10023139;

            var testLearner = new TestLearner()
            {
                CampId = "camp2"
            };

            var fileDataCacheMock = new Mock<IFileDataCache>();

            fileDataCacheMock.SetupGet(c => c.UKPRN).Returns(ukprn);

            var organisationDataServiceMockup = new Mock<IOrganisationDataService>();

            organisationDataServiceMockup.Setup(p => p.CampIdMatchForUkprn("camp2", ukprn)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, organisationDataService: organisationDataServiceMockup.Object, fileDataCache: fileDataCacheMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            string campId = "camp3";

            int ukprn = 10023139;

            var fileDataCacheMock = new Mock<IFileDataCache>();

            fileDataCacheMock.SetupGet(c => c.UKPRN).Returns(ukprn);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.CampId, campId)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(campId, ukprn);

            validationErrorHandlerMock.Verify();
        }

        private CampId_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IOrganisationDataService organisationDataService = null, IFileDataCache fileDataCache = null)
        {
            return new CampId_02Rule(organisationDataService: organisationDataService, validationErrorHandler: validationErrorHandler, fileDataCache: fileDataCache);
        }
    }
}
