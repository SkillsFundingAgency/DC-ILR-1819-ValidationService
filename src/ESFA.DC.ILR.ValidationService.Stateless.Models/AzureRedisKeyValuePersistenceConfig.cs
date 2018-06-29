using System;
using ESFA.DC.IO.Redis.Config.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class AzureRedisKeyValuePersistenceConfig : IRedisKeyValuePersistenceServiceConfig
    {
        public AzureRedisKeyValuePersistenceConfig(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        public TimeSpan? KeyExpiry => TimeSpan.FromDays(14);
    }
}
