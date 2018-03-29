using System;
using ESFA.DC.ILR.ValidationService.InternalData.LearnFAMTypeCode;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.InternalData.Tests.LearnFAMTypeCode
{
    public class LearnFAMTypeCodeInternalDataServiceTests
    {
        [Theory]
        [InlineData("HNS")]
        [InlineData("EHC")]
        [InlineData("DLA")]
        [InlineData("LSR")]
        [InlineData("NLM")]
        [InlineData("FME")]
        [InlineData("PPE")]
        [InlineData("SEN")]
        [InlineData("EDF")]
        [InlineData("MCF")]
        [InlineData("ECF")]
        public void TypeExists_True(string type)
        {
            var learnFAMTypeCodeInternalDataService = new LearnFAMTypeCodeInternalDataService();

            learnFAMTypeCodeInternalDataService.TypeExists(type).Should().BeTrue();
        }

        [Theory]
        [InlineData("FFFF")]
        [InlineData("XYZ")]
        [InlineData(null)]
        [InlineData("")]
        public void TypeExists_False(string type)
        {
            var learnFAMTypeCodeInternalDataService = new LearnFAMTypeCodeInternalDataService();

            learnFAMTypeCodeInternalDataService.TypeExists(type).Should().BeFalse();
        }

        [Theory]
        [InlineData("HNS", 1)]
        [InlineData("EHC", 1)]
        [InlineData("DLA", 1)]
        [InlineData("LSR", 36)]
        [InlineData("LSR", 55)]
        [InlineData("LSR", 56)]
        [InlineData("LSR", 57)]
        [InlineData("LSR", 58)]
        [InlineData("LSR", 59)]
        [InlineData("LSR", 60)]
        [InlineData("LSR", 61)]
        [InlineData("NLM", 17)]
        [InlineData("NLM", 18)]
        [InlineData("FME", 1)]
        [InlineData("FME", 2)]
        [InlineData("PPE", 1)]
        [InlineData("PPE", 2)]
        [InlineData("SEN", 1)]
        [InlineData("EDF", 1)]
        [InlineData("EDF", 2)]
        [InlineData("MCF", 1)]
        [InlineData("MCF", 2)]
        [InlineData("MCF", 3)]
        [InlineData("MCF", 4)]
        [InlineData("ECF", 1)]
        [InlineData("ECF", 2)]
        [InlineData("ECF", 3)]
        [InlineData("ECF", 4)]
        public void TypeForCodeExists_True(string type, long? code)
        {
            var service = new LearnFAMTypeCodeInternalDataService();

            service.TypeCodeExists(type, code).Should().BeTrue();
        }

        [Theory]
        [InlineData("NLM", 10)]
        [InlineData("MCF", 50)]
        [InlineData("EDF", 150)]
        [InlineData("LSR", 0)]
        [InlineData("", 0)]
        public void TypeForCodeExists_False(string type, long? code)
        {
            var service = new LearnFAMTypeCodeInternalDataService();

            service.TypeCodeExists(type, code).Should().BeFalse();
        }

        [Theory]
        [InlineData("HNS", 1, "2099-12-31")]
        [InlineData("EHC", 1, "2099-12-31")]
        [InlineData("DLA", 1, "2099-12-31")]
        [InlineData("LSR", 36, "2099-12-31")]
        [InlineData("LSR", 55, "2099-12-31")]
        [InlineData("LSR", 56, "2099-12-31")]
        [InlineData("LSR", 57, "2099-12-31")]
        [InlineData("LSR", 58, "2099-12-31")]
        [InlineData("LSR", 59, "2099-12-31")]
        [InlineData("LSR", 60, "2099-12-31")]
        [InlineData("LSR", 61, "2099-12-31")]
        [InlineData("NLM", 17, "2099-12-31")]
        [InlineData("NLM", 18, "2099-12-31")]
        [InlineData("FME", 1, "2099-12-31")]
        [InlineData("FME", 2, "2099-12-31")]
        [InlineData("PPE", 1, "2099-12-31")]
        [InlineData("PPE", 2, "2099-12-31")]
        [InlineData("SEN", 1, "2099-12-31")]
        [InlineData("EDF", 1, "2099-12-31")]
        [InlineData("EDF", 2, "2099-12-31")]
        [InlineData("MCF", 1, "2099-12-31")]
        [InlineData("MCF", 2, "2099-12-31")]
        [InlineData("MCF", 3, "2099-12-31")]
        [InlineData("MCF", 4, "2099-12-31")]
        [InlineData("ECF", 1, "2099-12-31")]
        [InlineData("ECF", 2, "2099-12-31")]
        [InlineData("ECF", 3, "2099-12-31")]
        [InlineData("ECF", 4, "2099-12-31")]
        public void TypeForCodeExists_DateTime_True(string contactPreferrenceType, long? code, string datetime)
        {
            var learnFAMTypeCodeInternalDataService = new LearnFAMTypeCodeInternalDataService();
            var validToDate = DateTime.ParseExact(datetime, "yyyy-MM-dd", null);

            learnFAMTypeCodeInternalDataService.TypeCodeForDateExists(contactPreferrenceType, code, validToDate).Should().BeTrue();
        }

        [Theory]
        [InlineData("HNS", 1, "2100-01-01")]
        [InlineData("EHC", 1, "2100-01-01")]
        [InlineData("EDF", 1, "2100-01-01")]
        [InlineData("ECF", 2, "2100-01-01")]
        [InlineData("MCF", 4, "2100-01-01")]
        [InlineData("SEN", 4, "2100-01-01")]
        [InlineData("LSR", 99, "2100-01-01")]
        public void TypeForCodeExists_DateTime_False(string type, long? code, string datetime)
        {
            var learnFAMTypeCodeInternalDataService = new LearnFAMTypeCodeInternalDataService();
            var validToDate = DateTime.ParseExact(datetime, "yyyy-MM-dd", null);

            learnFAMTypeCodeInternalDataService.TypeCodeForDateExists(type, code, validToDate).Should().BeFalse();
        }
    }
}