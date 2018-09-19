using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.ServiceFabric;
using ESFA.DC.Auditing;
using ESFA.DC.Auditing.Dto;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.Modules.Stateless;
using ESFA.DC.ILR.ValidationService.Stateless.Configuration;
using ESFA.DC.ILR.ValidationService.Stateless.Handlers;
using ESFA.DC.ILR.ValidationService.Stateless.Mapper;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.IO.Redis;
using ESFA.DC.IO.Redis.Config;
using ESFA.DC.IO.Redis.Config.Interfaces;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobStatus.Dto;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Mapping.Interface;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.ServiceFabric.Helpers;

namespace ESFA.DC.ILR.ValidationService.Stateless
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.
               var builder = BuildContainer();

                // Register the Autofac magic for Service Fabric support.
                builder.RegisterServiceFabricSupport();

                // Register the stateless service.
                builder.RegisterStatelessService<Stateless>("ESFA.DC.ILR.ValidationService.StatelessType");

                using (var container = builder.Build())
                {
                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Stateless).Name);

                    // Prevents this host process from terminating so services keep running.
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static ContainerBuilder BuildContainer()
        {
            Console.WriteLine($"BuildContainer:1");
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<PreValidationServiceModule>();

            Console.WriteLine($"BuildContainer:2");
            // get ServiceBus, Azurestorage config values and register container
            var configHelper = new ConfigurationHelper();
            var serviceBusOptions =
                configHelper.GetSectionValues<ServiceBusOptions>("ServiceBusSettings");
            containerBuilder.RegisterInstance(serviceBusOptions).As<ServiceBusOptions>().SingleInstance();

            Console.WriteLine($"BuildContainer:3");
            var azureStorageOptions =
                configHelper.GetSectionValues<AzureStorageModel>("AzureStorageSection");
            containerBuilder.RegisterInstance(azureStorageOptions).As<AzureStorageModel>().SingleInstance();
            containerBuilder.RegisterInstance(azureStorageOptions).As<IAzureStorageKeyValuePersistenceServiceConfig>().SingleInstance();

            Console.WriteLine($"BuildContainer:4");
            // register logger
            var loggerOptions =
                configHelper.GetSectionValues<LoggerOptions>("LoggerSection");
            containerBuilder.RegisterInstance(loggerOptions).As<LoggerOptions>().SingleInstance();
            containerBuilder.RegisterModule<LoggerModule>();

            Console.WriteLine($"BuildContainer:5");
            var azureRedisCacheOptions = configHelper.GetSectionValues<AzureRedisCacheOptions>("AzureRedisSection");
            containerBuilder.Register(c => new RedisKeyValuePersistenceServiceConfig()
            {
                ConnectionString = azureRedisCacheOptions.RedisCacheConnectionString,
                KeyExpiry = new TimeSpan(14, 0, 0, 0)
            }).As<IRedisKeyValuePersistenceServiceConfig>().SingleInstance();
            containerBuilder.RegisterType<RedisKeyValuePersistenceService>()
                .Keyed<IKeyValuePersistenceService>(PersistenceStorageKeys.Redis)
                .As<IKeyValuePersistenceService>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AzureStorageKeyValuePersistenceService>()
                .Keyed<IKeyValuePersistenceService>(PersistenceStorageKeys.AzureStorage)
                .As<IKeyValuePersistenceService>()
                .As<IStreamableKeyValuePersistenceService>()
                .InstancePerLifetimeScope();

            Console.WriteLine($"BuildContainer:6");
            // service bus queue configuration
            var queueSubscriptionConfig = new ServiceBusQueueConfig(
                serviceBusOptions.ServiceBusConnectionString,
                serviceBusOptions.JobsQueueName,
                Environment.ProcessorCount);

            Console.WriteLine($"BuildContainer:7");
            var topicPublishConfig = new ServiceBusTopicConfiguration(
                serviceBusOptions.ServiceBusConnectionString,
                serviceBusOptions.TopicName,
                serviceBusOptions.FundingCalcSubscriptionName,
                Environment.ProcessorCount);

            Console.WriteLine($"BuildContainer:8");
            var auditPublishConfig = new ServiceBusQueueConfig(
                serviceBusOptions.ServiceBusConnectionString,
                serviceBusOptions.AuditQueueName,
                Environment.ProcessorCount);

            Console.WriteLine($"BuildContainer:9");
            // register queue services
            containerBuilder.Register(c =>
            {
                var queueSubscriptionService =
                    new QueueSubscriptionService<JobContextDto>(
                        queueSubscriptionConfig,
                        c.Resolve<IJsonSerializationService>(),
                        c.Resolve<ILogger>());
                return queueSubscriptionService;
            }).As<IQueueSubscriptionService<JobContextDto>>();

            Console.WriteLine($"BuildContainer:10");
            containerBuilder.Register(c =>
            {
                var topicPublishService =
                    new TopicPublishService<JobContextDto>(
                        topicPublishConfig,
                        c.Resolve<IJsonSerializationService>());
                return topicPublishService;
            }).As<ITopicPublishService<JobContextDto>>();

            Console.WriteLine($"BuildContainer:11");
            containerBuilder.Register(c => new QueuePublishService<AuditingDto>(
                    auditPublishConfig,
                    c.Resolve<IJsonSerializationService>()))
                .As<IQueuePublishService<AuditingDto>>();

            Console.WriteLine($"BuildContainer:12");
            // Job Status Update Service
            var jobStatusPublishConfig = new JobStatusQueueConfig(
                serviceBusOptions.ServiceBusConnectionString,
                serviceBusOptions.JobStatusQueueName,
                Environment.ProcessorCount);

            Console.WriteLine($"BuildContainer:13");
            containerBuilder.Register(c => new QueuePublishService<JobStatusDto>(
                    jobStatusPublishConfig,
                    c.Resolve<IJsonSerializationService>()))
                .As<IQueuePublishService<JobStatusDto>>();
            containerBuilder.RegisterType<JobStatus.JobStatus>().As<IJobStatus>();

            Console.WriteLine($"BuildContainer:14");
            // register job context manager
            containerBuilder.RegisterType<Auditor>().As<IAuditor>();
            containerBuilder.RegisterType<JobContextMessageMapper>()
                .As<IMapper<JobContextMessage, JobContextMessage>>();

            Console.WriteLine($"BuildContainer:15");
            // register Job Status
            containerBuilder.Register(c => new JobStatus.JobStatus(
                c.Resolve<IQueuePublishService<JobStatusDto>>()))
                .As<IJobStatus>();

            Console.WriteLine($"BuildContainer:16");
            containerBuilder.RegisterType<MessageHandler>().As<IMessageHandler>();

            Console.WriteLine($"BuildContainer:17");
            // register the  callback handle when a new message is received from ServiceBus
            containerBuilder.Register<Func<JobContextMessage, CancellationToken, Task<bool>>>(c => c.Resolve<IMessageHandler>().Handle);

            containerBuilder.RegisterType<JobContextManagerForQueue<JobContextMessage>>().As<IJobContextManager<JobContextMessage>>()
                .InstancePerLifetimeScope();

            Console.WriteLine($"BuildContainer:19");
            containerBuilder.RegisterType<JobContextMessage>().As<IJobContextMessage>()
                .InstancePerLifetimeScope();

            Console.WriteLine($"BuildContainer:20");

            return containerBuilder;
        }
    }
}
