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
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.ILR.ValidationService.Stubs;

namespace ESFA.DC.ILR.ValidationService.Modules.PreValidation
{
    public class PreValidationDataModule : BaseDataModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<PreValidationPopulationService>().As<IPreValidationPopulationService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AzureInternalDataCachePopulationServiceStub>()
                .As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();

            builder.Register(c =>
            {
                var referenceDataOptions = c.Resolve<ReferenceDataOptions>();
                return new LARS(referenceDataOptions.LARSConnectionString);
            }).As<ILARS>().InstancePerLifetimeScope();

            builder.Register(c =>
            {
                var referenceDataOptions = c.Resolve<ReferenceDataOptions>();
                return new ULN(referenceDataOptions.ULNConnectionstring);
            }).As<IULN>().InstancePerLifetimeScope();

            builder.Register(c =>
            {
                var referenceDataOptions = c.Resolve<ReferenceDataOptions>();
                return new Postcodes(referenceDataOptions.PostcodesConnectionString);
            }).As<IPostcodes>().InstancePerLifetimeScope();

            builder.Register(c =>
            {
                var referenceDataOptions = c.Resolve<ReferenceDataOptions>();
                return new ValidationErrors(referenceDataOptions.ValidationErrorsConnectionString);
            }).As<IValidationErrors>();
        }
    }
}
