using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.External
{
    public class ValidationErrorsDataServiceTests
    {
        [Fact]
        public void SeverityForRuleName_Warning()
        {
            var referenceDataCacheMock = new Mock<IExternalDataCache>();

            referenceDataCacheMock.SetupGet(rdc => rdc.ValidationErrors).Returns(new Dictionary<string, ValidationError>
            {
                { "rulename", new ValidationError() { RuleName = "rulename", Severity = Severity.Warning } }
            });

            NewService(referenceDataCacheMock.Object).SeverityForRuleName("rulename").Should().Be(Severity.Warning);
        }

        [Fact]
        public void SeverityForRuleName_Error()
        {
            var referenceDataCacheMock = new Mock<IExternalDataCache>();

            referenceDataCacheMock.SetupGet(rdc => rdc.ValidationErrors).Returns(new Dictionary<string, ValidationError>
            {
                { "rulename", new ValidationError() { RuleName = "rulename", Severity = Severity.Error } }
            });

            NewService(referenceDataCacheMock.Object).SeverityForRuleName("rulename").Should().Be(Severity.Error);
        }

        [Fact]
        public void SeverityForRuleName_Error_CaseSensitivityCheck()
        {
            var referenceDataCacheMock = new Mock<IExternalDataCache>();

            referenceDataCacheMock.SetupGet(rdc => rdc.ValidationErrors).Returns(new Dictionary<string, ValidationError>
            {
                { "RULENAME", new ValidationError() { RuleName = "rulename", Severity = Severity.Error } }
            }.ToCaseInsensitiveDictionary());

            NewService(referenceDataCacheMock.Object).SeverityForRuleName("rulename").Should().Be(Severity.Error);
        }

        [Fact]
        public void SeverityForRuleName_Missing()
        {
            var referenceDataCacheMock = new Mock<IExternalDataCache>();

            referenceDataCacheMock.SetupGet(rdc => rdc.ValidationErrors).Returns(new Dictionary<string, ValidationError>());

            NewService(referenceDataCacheMock.Object).SeverityForRuleName("rulename").Should().BeNull();
        }

        [Fact]
        public void MessageForRuleName_Missing()
        {
            var referenceDataCacheMock = new Mock<IExternalDataCache>();

            referenceDataCacheMock.SetupGet(rdc => rdc.ValidationErrors).Returns(new Dictionary<string, ValidationError>());

            NewService(referenceDataCacheMock.Object).MessageforRuleName("rulename").Should().BeNull();
        }

        [Fact]
        public void MessageForRuleName()
        {
            var referenceDataCacheMock = new Mock<IExternalDataCache>();

            referenceDataCacheMock.SetupGet(rdc => rdc.ValidationErrors).Returns(new Dictionary<string, ValidationError>
            {
                { "rulename", new ValidationError() { RuleName = "rulename", Message = "message" } }
            });

            NewService(referenceDataCacheMock.Object).MessageforRuleName("rulename").Should().Be("message");
        }

        private ValidationErrorsDataService NewService(IExternalDataCache externalDataCache = null)
        {
            return new ValidationErrorsDataService(externalDataCache);
        }
    }
}
