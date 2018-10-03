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

namespace ESFA.DC.ILR.ValidationService.Providers.PreValidation
{
    public class ValidateXMLSchemaService : AbstractRule, IValidateXMLSchemaService
    {
        private readonly string _learnRefNumber = "LearnRefNumber";
        private readonly IList<IErrorMessageParameter> _validationErrors = new List<IErrorMessageParameter>();
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

                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema
                };

                XmlReader xsdReader = XmlReader.Create(new StringReader(xsdFileContent), xmlReaderSettings);
                XmlReader xmlReader = XmlReader.Create(new StringReader(xmlFileContent));

                ValidateSchema(xsdReader, xmlReader);
            }
            catch (Exception e)
            {
                _validationErrors.Add(new ErrorMessageParameter(string.Empty, "The XML is not well formed."));
                _validationErrors.Add(new ErrorMessageParameter(string.Empty, e.Message));
            }

            if (_validationErrors.Count() > 0)
            {
                bool errorExist = true;
                foreach (var error in _validationErrors)
                {
                    HandleValidationError(null, null, BuildErrorMessageParameters(error.PropertyName, error.Value));
                    errorExist = false;
                }

                return errorExist;
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
            if (validationEventArgs.Severity == XmlSeverityType.Error)
            {
                string learnRefNumber = string.Empty;
                if (sender != null
                    && sender.GetType() == typeof(XElement))
                {
                    learnRefNumber = GetLearnRefNumberFromXML((XElement)sender);
                }

                if (validationEventArgs.Message.Contains("has invalid child element"))
                {
                    _validationErrors.Add(new ErrorMessageParameter(learnRefNumber, string.Format(
                    "Line: {0} Position: {1} - {2}",
                    validationEventArgs.Exception?.LineNumber,
                    validationEventArgs.Exception?.LinePosition,
                    validationEventArgs.Message)));
                }
            }
        }
    }
}
