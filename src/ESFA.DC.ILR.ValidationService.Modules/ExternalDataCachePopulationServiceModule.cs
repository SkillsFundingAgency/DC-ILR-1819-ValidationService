using Autofac;
using ESFA.DC.Data.ILR.ValidationErrors.Model;
using ESFA.DC.Data.ILR.ValidationErrors.Model.Interfaces;
using ESFA.DC.Data.LARS.Model;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.Data.Postcodes.Model;
using ESFA.DC.Data.Postcodes.Model.Interfaces;
using ESFA.DC.Data.ULN.Model;
using ESFA.DC.Data.ULN.Model.Interfaces;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class ExternalDataCachePopulationServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ExternalDataCachePopulationService>().As<IExternalDataCachePopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<LARSLearningDeliveryDataRetrievalService>().As<ILARSLearningDeliveryDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<LARSFrameworkDataRetrievalService>().As<ILARSFrameworkDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<ULNDataRetrievalService>().As<IULNDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<PostcodesDataRetrievalService>().As<IPostcodesDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorsDataRetrievalService>().As<IValidationErrorsDataRetrievalService>().InstancePerLifetimeScope();
        }
    }
}
