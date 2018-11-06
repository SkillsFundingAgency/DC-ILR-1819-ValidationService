using System.Xml;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidateXMLSchemaService
    {
        bool Validate();

        void ValidateSchema(XmlReader xsdReader, XmlReader xmlReader);
    }
}
