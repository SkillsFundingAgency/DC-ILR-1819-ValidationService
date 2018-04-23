using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ULN;
using ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Data.File;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AimType;
using ESFA.DC.ILR.ValidationService.Data.Internal.AimType.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.CompStatus;
using ESFA.DC.ILR.ValidationService.Data.Internal.CompStatus.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.EmpOutcome;
using ESFA.DC.ILR.ValidationService.Data.Internal.EmpOutcome.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.FundModel;
using ESFA.DC.ILR.ValidationService.Data.Internal.FundModel.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.FileData;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.Stubs;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PreValidationPopulationService>().As<IPreValidationPopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<ExternalDataCacheStub>().As<IExternalDataCache>().InstancePerLifetimeScope();
            builder.RegisterType<ExternalDataCachePopulationServiceStub>().As<IExternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<LARSDataServiceStub>().As<ILARSDataService>().InstancePerLifetimeScope();
            builder.RegisterType<OrganisationDataService>().As<IOrganisationDataService>().InstancePerLifetimeScope();
            builder.RegisterType<ULNDataService>().As<IULNDataService>().InstancePerLifetimeScope();
            builder.RegisterType<FCSDataServiceStub>().As<IFCSDataService>().InstancePerLifetimeScope();
            builder.RegisterType<PostcodesDataServiceStub>().As<IPostcodesDataService>();

            builder.RegisterType<InternalDataCache>().As<IInternalDataCache>().InstancePerLifetimeScope();
            builder.RegisterType<InternalDataCachePopulationServiceStub>().As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<AcademicYearDataService>().As<IAcademicYearDataService>().InstancePerLifetimeScope();
            builder.RegisterType<AimTypeDataService>().As<IAimTypeDataService>().InstancePerLifetimeScope();
            builder.RegisterType<CompStatusDataService>().As<ICompStatusDataService>().InstancePerLifetimeScope();
            builder.RegisterType<EmpOutcomeDataService>().As<IEmpOutcomeDataService>().InstancePerLifetimeScope();
            builder.RegisterType<FundModelDataService>().As<IFundModelDataService>().InstancePerLifetimeScope();

            builder.RegisterType<FileDataCache>().As<IFileDataCache>().InstancePerLifetimeScope();
            builder.RegisterType<FileDataCachePopulationService>().As<IFileDataCachePopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<Cache<IMessage>>().As<ICache<IMessage>>().InstancePerLifetimeScope();
            builder.RegisterType<MessageCachePopulationService>().As<IMessageCachePopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();
        }
    }
}
