using Autofac;
using ESFA.DC.Data.ILR.ValidationErrors.Model;
using ESFA.DC.Data.ILR.ValidationErrors.Model.Interfaces;
using ESFA.DC.Data.LARS.Model;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.Data.Postcodes.Model;
using ESFA.DC.Data.Postcodes.Model.Interfaces;
using ESFA.DC.Data.ULN.Model;
using ESFA.DC.Data.ULN.Model.Interfaces;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Configuration;
using ESFA.DC.ILR.ValidationService.Data.Population.Configuration.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Stubs;
using ESFA.DC.ServiceFabric.Helpers;

namespace ESFA.DC.ILR.ValidationService.Modules.Stateless
{
    public class PreValidationDataModule : BaseDataModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configHelper = new ConfigurationHelper();

            var referenceDataOptions = configHelper.GetSectionValues<ReferenceDataOptions>("ReferenceDataSection");
            builder.RegisterInstance(referenceDataOptions).As<IReferenceDataOptions>().SingleInstance();

            builder.RegisterType<PreValidationPopulationService>().As<IPreValidationPopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<AzureInternalDataCachePopulationServiceStub>().As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();

            builder.Register(c => new LARS(c.Resolve<IReferenceDataOptions>().LARSConnectionString)).As<ILARS>().InstancePerLifetimeScope();
            builder.Register(c => new ULN(c.Resolve<IReferenceDataOptions>().ULNConnectionstring)).As<IULN>().InstancePerLifetimeScope();
            builder.Register(c => new Postcodes(c.Resolve<IReferenceDataOptions>().PostcodesConnectionString)).As<IPostcodes>().InstancePerLifetimeScope();
            builder.Register(c => new ValidationErrors(c.Resolve<IReferenceDataOptions>().ValidationErrorsConnectionString)).As<IValidationErrors>();

            base.Load(builder);
        }
    }
}
