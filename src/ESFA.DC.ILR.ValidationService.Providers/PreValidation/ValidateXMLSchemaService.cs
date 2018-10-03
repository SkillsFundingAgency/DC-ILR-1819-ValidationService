using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers.PreValidation
{
    public class ValidateXMLSchemaService : AbstractRule, IValidateXMLSchemaService
    {
        private readonly string _learnRefNumber = "LearnRefNumber";
        private readonly IList<IErrorMessageParameter> _validationErrors = new List<IErrorMessageParameter>();
        private readonly ISchemaStringProviderService _schemaFileContentStringProviderService;
        private readonly ICache<string> _fileContentCache;
        private readonly ILogger _logger;

        /// <summary>
        /// Flag indicating whether a generic error has occurred, and we should report it, but don't have any context to support it.
        /// </summary>
        private bool needRaiseError;

        public ValidateXMLSchemaService(
            IValidationErrorHandler validationErrorHandler,
            ISchemaStringProviderService schemaFileContentStringProviderService,
            ICache<string> fileContentCache,
            ILogger logger)
            : base(validationErrorHandler, RuleNameConstants.Schema)
        {
            _schemaFileContentStringProviderService = schemaFileContentStringProviderService;
            _fileContentCache = fileContentCache;
            _logger = logger;
        }

        public bool Validate()
        {
            try
            {
                string xsdFileContent = _schemaFileContentStringProviderService.Provide();
                string xmlFileContent = _fileContentCache.Item;

                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema
                };

                XmlReader xsdReader = XmlReader.Create(new StringReader(xsdFileContent), xmlReaderSettings);
                XmlReader xmlReader = XmlReader.Create(new StringReader(xmlFileContent));

                ValidateSchema(xsdReader, xmlReader);
            }
            catch (Exception ex)
            {
                _logger.LogError("Schema validation caught unexpected exception, it will be reported to user", ex);
                HandleValidationError();
            }

            if (_validationErrors.Any())
            {
                foreach (IErrorMessageParameter error in _validationErrors)
                {
                    HandleValidationError(errorMessageParameters: BuildErrorMessageParameters(error.PropertyName, error.Value));
                }

                return false;
            }

            if (needRaiseError)
            {
                HandleValidationError();
                return false;
            }

            return true;
        }

        public void ValidateSchema(XmlReader xsdReader, XmlReader xmlReader)
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();

            schemaSet.Add(XmlSchema.Read(xsdReader, null));

            schemaSet.CompilationSettings = new XmlSchemaCompilationSettings();
            schemaSet.Compile();

            XmlReaderSettings settings = new XmlReaderSettings
            {
                CloseInput = true,
                ValidationType = ValidationType.Schema,
                Schemas = schemaSet,
                ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings |
                    XmlSchemaValidationFlags.ProcessIdentityConstraints |
                    XmlSchemaValidationFlags.ProcessInlineSchema |
                    XmlSchemaValidationFlags.ProcessSchemaLocation
            };
            settings.ValidationEventHandler += ValidationEventHandler;

            using (XmlReader validatingReader = XmlReader.Create(xmlReader, settings))
            {
                XmlDocument x = new XmlDocument();
                x.Load(validatingReader);

                while (validatingReader.Read())
                {
                }
            }
        }

        public string GetLearnRefNumberFromXML(XElement xElement)
        {
            string learnRefNumberValue = GetLearnRefNumberFromElement(xElement);
            if (string.IsNullOrEmpty(learnRefNumberValue)
                && xElement != null)
            {
                foreach (XNode xNode in xElement.DescendantNodes())
                {
                    if (xNode.GetType() == typeof(XElement))
                    {
                        learnRefNumberValue = GetLearnRefNumberFromElement((XElement)xNode);
                        if (!string.IsNullOrEmpty(learnRefNumberValue))
                        {
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(learnRefNumberValue))
                {
                    learnRefNumberValue = GetLearnRefNumberFromElement(xElement.Parent);
                    if (string.IsNullOrEmpty(learnRefNumberValue)
                        && xElement.Parent != null)
                    {
                        foreach (XNode xNode in xElement.Parent.DescendantNodes())
                        {
                            if (xNode.GetType() == typeof(XElement))
                            {
                                learnRefNumberValue = GetLearnRefNumberFromElement((XElement)xNode);
                                if (!string.IsNullOrEmpty(learnRefNumberValue))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return learnRefNumberValue;
        }

        public string GetLearnRefNumberFromElement(XElement xElement)
        {
            string learnRefNumber = string.Empty;
            if (xElement != null
                && xElement.Name.LocalName == _learnRefNumber)
            {
                learnRefNumber = xElement.Value;
            }

            return learnRefNumber;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string propertyName, string value)
        {
            return new[]
            {
                BuildErrorMessageParameter(propertyName, value)
            };
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs validationEventArgs)
        {
            if (validationEventArgs.Severity != XmlSeverityType.Error)
            {
                return;
            }

            needRaiseError = true;

            if (!validationEventArgs.Message.Contains("has invalid child element"))
            {
                return;
            }

            string learnRefNumber = string.Empty;
            try
            {
                if (sender != null
                    && sender.GetType() == typeof(XElement))
                {
                    learnRefNumber = GetLearnRefNumberFromXML((XElement)sender);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't get learn ref number from invalid XML, will continue without it", ex);
            }

            _validationErrors.Add(new ErrorMessageParameter(
                learnRefNumber,
                $"Line: {validationEventArgs.Exception?.LineNumber} Position: {validationEventArgs.Exception?.LinePosition} - {validationEventArgs.Message}"));
        }
    }
}
