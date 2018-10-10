using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class ExternalDataCachePopulationServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ExternalDataCachePopulationService>().As<IExternalDataCachePopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<LARSStandardValidityDataRetrievalService>().As<ILARSStandardValidityDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<LARSLearningDeliveryDataRetrievalService>().As<ILARSLearningDeliveryDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<LARSFrameworkDataRetrievalService>().As<ILARSFrameworkDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<ULNDataRetrievalService>().As<IULNDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<PostcodesDataRetrievalService>().As<IPostcodesDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<OrganisationsDataRetrievalService>().As<IOrganisationsDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<EPAOrganisationsDataRetrievalService>().As<IEPAOrganisationsDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorsDataRetrievalService>().As<IValidationErrorsDataRetrievalService>().InstancePerLifetimeScope();
        }
    }
}
