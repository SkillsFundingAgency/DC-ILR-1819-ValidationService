using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers.PreValidation;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class ValidateXMLSchemaServiceTests
    {
        private readonly string schemaFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files\ILR-2018-19-v2.xsd");
        private readonly string IlrValidXmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files\ILR_Valid.xml");
        private readonly string IlrInValidXmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files\ILR_InValid.xml");

        [Fact]
        public void RuleName()
        {
            NewService().RuleName.Should().Be(RuleNameConstants.Schema);
        }

        [Fact]
        public void Validate_True()
        {
            var xsdFileContentString = File.ReadAllText(schemaFilePath);
            var xmlFileContentString = File.ReadAllText(IlrValidXmlFilePath);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var schemaStringProviderServiceMock = new Mock<ISchemaStringProviderService>();

            var fileCacheMock = new Mock<ICache<string>>();
            fileCacheMock.SetupGet(sp => sp.Item).Returns(xmlFileContentString);

            schemaStringProviderServiceMock.Setup(sp => sp.Provide()).Returns(xsdFileContentString);

            NewService(validationErrorHandlerMock.Object, fileCacheMock.Object, schemaStringProviderServiceMock.Object).Validate().Should().BeTrue();
        }

        [Fact]
        public void Validate_False()
        {
            var xsdFileContentString = File.ReadAllText(schemaFilePath);
            var xmlFileContentString = File.ReadAllText(IlrInValidXmlFilePath);

            var fileCacheMock = new Mock<ICache<string>>();
            fileCacheMock.SetupGet(sp => sp.Item).Returns(xmlFileContentString);
            var schemaStringProviderServiceMock = new Mock<ISchemaStringProviderService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            schemaStringProviderServiceMock.Setup(sp => sp.Provide()).Returns(xsdFileContentString);

            NewService(validationErrorHandlerMock.Object, fileCacheMock.Object, schemaStringProviderServiceMock.Object).Validate().Should().BeFalse();
        }

        [Fact]
        public void ValidateSchema_Valid()
        {
            var xsdFileContentString = File.ReadAllText(schemaFilePath);
            var xmlFileContentString = File.ReadAllText(IlrValidXmlFilePath);

            XmlReader xsdReader = XmlReader.Create(new StringReader(xsdFileContentString));
            XmlReader xmlReader = XmlReader.Create(new StringReader(xmlFileContentString));

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var fileCacheMock = new Mock<ICache<string>>();
            fileCacheMock.SetupGet(sp => sp.Item).Returns(xmlFileContentString);
            var schemaStringProviderServiceMock = new Mock<ISchemaStringProviderService>();

            validationErrorHandlerMock
                .Setup(veh => veh.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()))
                .Throws(new Exception("Validation Error should not be Handled."));
            schemaStringProviderServiceMock.Setup(sp => sp.Provide()).Returns(xsdFileContentString);

            NewService(validationErrorHandlerMock.Object, fileCacheMock.Object, schemaStringProviderServiceMock.Object).ValidateSchema(xsdReader, xmlReader);

            validationErrorHandlerMock.Verify();
        }

        [Fact]
        public void ValidateSchema_InValid()
        {
            var xsdFileContentString = File.ReadAllText(schemaFilePath);
            var xmlFileContentString = File.ReadAllText(IlrInValidXmlFilePath);
            IEnumerable<IErrorMessageParameter> errorMessageParameters = new List<IErrorMessageParameter>()
            {
               new ErrorMessageParameter("", "")
            };
            XmlReader xsdReader = XmlReader.Create(new StringReader(xsdFileContentString));
            XmlReader xmlReader = XmlReader.Create(new StringReader(xmlFileContentString));

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var fileCacheMock = new Mock<ICache<string>>();
            fileCacheMock.SetupGet(sp => sp.Item).Returns(xmlFileContentString);
            var schemaStringProviderServiceMock = new Mock<ISchemaStringProviderService>();

            schemaStringProviderServiceMock.Setup(sp => sp.Provide()).Returns(xsdFileContentString);
            validationErrorHandlerMock.Setup(ve => ve.Handle("Schema", null, null, errorMessageParameters));

            NewService(validationErrorHandlerMock.Object, fileCacheMock.Object, schemaStringProviderServiceMock.Object).ValidateSchema(xsdReader, xmlReader);

            validationErrorHandlerMock.Verify();
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            string validationError = "The XML is not well formed.";
            IList<string> validationErrors = new List<string>() { validationError };
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("", validationError)).Verifiable();
            
            NewService(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(validationErrors);

            validationErrorHandlerMock.Verify();
        }

        [Fact]
        public void BuildErrorMessageParameters_Null()
        {
            IList<string> validationErrors = null;
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("", "")).Verifiable();

            NewService(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(validationErrors).Should().BeNullOrEmpty();

        }

        public ValidateXMLSchemaService NewService(
            IValidationErrorHandler validationErrorHandler = null,
            ICache<string> fileCache = null,
            ISchemaStringProviderService schemaStringProviderService = null)
        {
            return new ValidateXMLSchemaService(
                validationErrorHandler: validationErrorHandler,
                fileContentCache: fileCache,
                schemaFileContentStringProviderService: schemaStringProviderService);
        }
    }
}
