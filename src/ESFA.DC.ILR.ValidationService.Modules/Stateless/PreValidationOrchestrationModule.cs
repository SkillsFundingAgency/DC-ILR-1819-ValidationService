using System.Collections.Generic;
using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.Providers.Output;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.ILR.ValidationService.Stubs;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;

namespace ESFA.DC.ILR.ValidationService.Modules.Stateless
{
    public class PreValidationOrchestrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PreValidationOrchestrationSfService<IValidationError>>().As<IPreValidationOrchestrationService<IValidationError>>().WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<MessageAzureStorageProviderService>().As<IValidationItemProviderService<IMessage>>().WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<ValidationOutputService>().As<IValidationOutputService<IValidationError>>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerPerActorServiceStub>().As<ILearnerPerActorService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache<IValidationError>>().InstancePerLifetimeScope();
            builder.RegisterType<PreValidationPopulationService>().As<IPreValidationPopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerProviderService>().As<IValidationItemProviderService<IEnumerable<ILearner>>>().InstancePerLifetimeScope();
        }
    }
}
