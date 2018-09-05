using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers.PreValidation
{
    public class ValidateXMLSchemaService : AbstractRule, IValidateXMLSchemaService
    {
        private readonly IList<string> _validationErrors = new List<string>();
        private readonly ISchemaStringProviderService _schemaFileContentStringProviderService;
        private readonly ICache<string> _fileContentCache;

        public ValidateXMLSchemaService(
            IValidationErrorHandler validationErrorHandler,
            ISchemaStringProviderService schemaFileContentStringProviderService,
            ICache<string> fileContentCache)
            : base(validationErrorHandler, RuleNameConstants.Schema)
        {
            _schemaFileContentStringProviderService = schemaFileContentStringProviderService;
            _fileContentCache = fileContentCache;
        }

        public bool Validate()
        {
            try
            {
                string xsdFileContent = _schemaFileContentStringProviderService.Provide();
                string xmlFileContent = _fileContentCache.Item;

                XmlReader xsdReader = XmlReader.Create(new StringReader(xsdFileContent));
                XmlReader xmlReader = XmlReader.Create(new StringReader(xmlFileContent));

                ValidateSchema(xsdReader, xmlReader);
            }
            catch (Exception e)
            {
                _validationErrors.Add("The XML is not well formed.");
                _validationErrors.Add(e.Message);
            }

            if (_validationErrors.Count() > 0)
            {
                HandleValidationError(null, null, BuildErrorMessageParameters(_validationErrors));
                return false;
            }

            return true;
        }

        public void ValidateSchema(XmlReader xsdReader, XmlReader xmlReader)
        {
            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.Add(null, xsdReader);

            XDocument xmlDocument = XDocument.Load(xmlReader);
            xmlDocument.Validate(xmlSchemaSet, ValidationEventHandler);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(IList<string> validationErrors)
        {
            return validationErrors?.Select(s => BuildErrorMessageParameter(string.Empty, s)).ToArray();
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs validationEventArgs)
        {
            XmlSeverityType xmlSeverityType = XmlSeverityType.Warning;
            if (Enum.TryParse<XmlSeverityType>("Error", out xmlSeverityType))
            {
                if (xmlSeverityType == XmlSeverityType.Error)
                {
                    _validationErrors.Add(validationEventArgs.Message);
                }
            }
        }
    }
}
