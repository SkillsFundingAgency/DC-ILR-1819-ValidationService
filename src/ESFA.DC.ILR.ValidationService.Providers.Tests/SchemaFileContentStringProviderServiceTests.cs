using FluentAssertions;
using System;
using System.IO;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class SchemaFileContentStringProviderServiceTests
    {
        private readonly string schemaFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files\ILR-2018-19-v2.xsd");

        [Fact]
        public void Provide()
        {

            var xsdFileContentString = File.ReadAllText(schemaFilePath);
            
            NewService().Provide().Should().Equals(xsdFileContentString);
        }

        private SchemaFileContentStringProviderService NewService()
        {
            return new SchemaFileContentStringProviderService();
        }
    }
}
