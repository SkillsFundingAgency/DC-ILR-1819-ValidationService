using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.ExternalData.FileDataService;
using ESFA.DC.ILR.ValidationService.ExternalData.FileDataService.Interface;
using ESFA.DC.ILR.ValidationService.ExternalData.Interface;
using ESFA.DC.ILR.ValidationService.ExternalData.Organisation;
using ESFA.DC.ILR.ValidationService.ExternalData.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.ExternalData.ULN;
using ESFA.DC.ILR.ValidationService.ExternalData.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData;
using ESFA.DC.ILR.ValidationService.InternalData.AcademicYearCalendarService;
using ESFA.DC.ILR.ValidationService.InternalData.AimType;
using ESFA.DC.ILR.ValidationService.InternalData.AimType.Interfaces;
using ESFA.DC.ILR.ValidationService.InternalData.ContPrefType;
using ESFA.DC.ILR.ValidationService.InternalData.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.LearnFAMTypeCode;
using ESFA.DC.ILR.ValidationService.InternalData.LLDDCat;
using ESFA.DC.ILR.ValidationService.InternalData.PriorAttain;
using ESFA.DC.ILR.ValidationService.InternalData.ValidationDataService;
using ESFA.DC.ILR.ValidationService.Modules.Stubs;
using ESFA.DC.ILR.ValidationService.RuleSet;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ReferenceDataCacheStub>().As<IReferenceDataCache>().InstancePerLifetimeScope();
            builder.RegisterType<ReferenceDataCachePopulationServiceStub>().As<IReferenceDataCachePopulationService<ILearner>>().InstancePerLifetimeScope();
            builder.RegisterType<FileDataService>().As<IFileDataService>().InstancePerLifetimeScope();
            builder.RegisterType<OrganisationReferenceDataService>().As<IOrganisationReferenceDataService>().InstancePerLifetimeScope();
            builder.RegisterType<ULNReferenceDataService>().As<IULNReferenceDataService>().InstancePerLifetimeScope();

            builder.RegisterType<InternalDataCache>().As<IInternalDataCache>().InstancePerLifetimeScope();
            builder.RegisterType<InternalDataCachePopulationServiceStub>().As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<AimTypeInternalDataService>().As<IAimTypeInternalDataService>().InstancePerLifetimeScope();

            builder.RegisterType<ValidationDataService>().As<IValidationDataService>().InstancePerLifetimeScope();
            builder.RegisterType<AcademicYearCalendarService>().As<IAcademicYearCalendarService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();
        }
    }
}
