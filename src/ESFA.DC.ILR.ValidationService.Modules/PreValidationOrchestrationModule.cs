using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.ILR.ValidationService.Stubs;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Xml;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class PreValidationOrchestrationModule : BaseValidationOrchestrationModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PreValidationOrchestrationSfService<ILearner, IValidationError>>()
                .As<IPreValidationOrchestrationService<ILearner, IValidationError>>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            builder.RegisterType<MessageAzureStorageProviderService>()
                .As<IValidationItemProviderService<IMessage>>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            builder.RegisterType<LearnerPerActorServiceStub<ILearner, IEnumerable<ILearner>>>()
                .As<ILearnerPerActorService<ILearner, IEnumerable<ILearner>>>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
