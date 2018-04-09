using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ULN;
using ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.FileData;
using ESFA.DC.ILR.ValidationService.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData;
using ESFA.DC.ILR.ValidationService.InternalData.AcademicYearCalendarService;
using ESFA.DC.ILR.ValidationService.InternalData.AimType;
using ESFA.DC.ILR.ValidationService.InternalData.AimType.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.CompStatus;
using ESFA.DC.ILR.ValidationService.InternalData.CompStatus.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.ValidationDataService;
using ESFA.DC.ILR.ValidationService.Modules.Stubs;
using ESFA.DC.ILR.ValidationService.Rules.File.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ReferenceDataCacheStub>().As<IExternalDataCache>().InstancePerLifetimeScope();
            builder.RegisterType<ReferenceDataCachePopulationServiceStub>().As<IExternalDataCachePopulationService<ILearner>>().InstancePerLifetimeScope();
            builder.RegisterType<OrganisationReferenceDataService>().As<IOrganisationReferenceDataService>().InstancePerLifetimeScope();
            builder.RegisterType<ULNReferenceDataService>().As<IULNReferenceDataService>().InstancePerLifetimeScope();

            builder.RegisterType<InternalDataCache>().As<IInternalDataCache>().InstancePerLifetimeScope();
            builder.RegisterType<InternalDataCachePopulationServiceStub>().As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<AimTypeInternalDataService>().As<IAimTypeInternalDataService>().InstancePerLifetimeScope();
            builder.RegisterType<CompStatusInternalDataService>().As<ICompStatusInternalDataService>().InstancePerLifetimeScope();

            builder.RegisterType<FileDataCache>().As<IFileDataCache>().InstancePerLifetimeScope();
            builder.RegisterType<FileDataCachePopulationService>().As<IFileDataCachePopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<ValidationDataService>().As<IValidationDataService>().InstancePerLifetimeScope();
            builder.RegisterType<AcademicYearCalendarService>().As<IAcademicYearCalendarService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();
        }
    }
}
