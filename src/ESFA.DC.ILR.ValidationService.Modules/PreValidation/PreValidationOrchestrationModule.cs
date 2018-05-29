using System.Collections.Generic;
using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.Stubs;

namespace ESFA.DC.ILR.ValidationService.Modules.PreValidation
{
    public class PreValidationOrchestrationModule : BaseValidationOrchestrationModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PreValidationOrchestrationSfService<ILearner, IValidationError>>().As<IPreValidationOrchestrationService<ILearner, IValidationError>>().WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<MessageAzureStorageProviderService>().As<IValidationItemProviderService<IMessage>>().WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<LearnerPerActorServiceStub<ILearner>>().As<ILearnerPerActorService<ILearner, IEnumerable<ILearner>>>().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
