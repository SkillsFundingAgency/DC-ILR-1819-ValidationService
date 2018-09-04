using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class SchemaFileContentStringProviderService : ISchemaStringProviderService
    {
        // TODO: temporary solution:
        //    in future can be changed to load the file from some package, or some specific location or contents can be provided through the calling service same as ILR xml.
        private readonly string schemaFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files\ILR-2018-19-v2.xsd");

        public SchemaFileContentStringProviderService()
        {
        }

        public string Provide()
        {
            return File.ReadAllText(schemaFilePath);
        }
    }
}
