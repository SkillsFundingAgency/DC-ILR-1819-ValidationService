using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidateXMLSchemaService
    {
        bool Validate();

        void ValidateSchema(XmlReader xsdReader, XmlReader xmlReader);
    }
}
