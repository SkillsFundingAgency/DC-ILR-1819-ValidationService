using System;
using System.Configuration;
using System.Data.SqlClient;
using Xunit;
using Xunit.Abstractions;
using TechTalk.SpecFlow;

namespace ESFA.DC.ILR.ValidationService.AcceptanceTests
{
    [Binding]
    [Trait("Category", "SmokeTest")]
    public class TestConnectionToLogDbSteps
    {
        String configItemName = "LoggerConnectionString";
        String DbConnectionString = string.Empty;

        private readonly ITestOutputHelper output;

        public TestConnectionToLogDbSteps(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Given(@"app config ""(.*)"" from the file")]
        public void GivenAppConfigFromTheFile(string ConfigItemName)
        {
            Assert.NotNull(ConfigItemName);
            Assert.NotEmpty(ConfigItemName);
            configItemName = ConfigItemName;

            output.WriteLine("ConfigItemName : {ConfigItemName}");

            Assert.NotNull(ConfigurationManager.AppSettings[configItemName]);
            DbConnectionString = ConfigurationManager.AppSettings[configItemName];

            //output.WriteLine("ConnectionString  : {DbConnectionString}");
        }

        [When(@"I try and Get the Datetime Now")]
        public void WhenITryAndGetTheDatetimeNow()
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT GetDate()", conn);
                output.WriteLine("Get System Date from SQL Server");
                var DateNow = command.ExecuteScalar();
                output.WriteLine("Value Returned : {0}", DateNow.ToString());
                Assert.NotNull(DateNow);
            }
        }
    }
}
