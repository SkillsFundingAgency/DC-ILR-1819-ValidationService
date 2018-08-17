using Autofac;
using ESFA.DC.ILR.ValidationService.Data.External.LARS;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ULN;
using ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors;
using ESFA.DC.ILR.ValidationService.Data.File.FileData;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
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
using ESFA.DC.ILR.ValidationService.Data.Internal.LLDDCat;
using ESFA.DC.ILR.ValidationService.Data.Internal.LLDDCat.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.QUALENT3;
using ESFA.DC.ILR.ValidationService.Data.Internal.QUALENT3.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class DataServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileDataService>().As<IFileDataService>().InstancePerLifetimeScope();
            builder.RegisterType<LARSDataService>().As<ILARSDataService>().InstancePerLifetimeScope();
            builder.RegisterType<OrganisationDataService>().As<IOrganisationDataService>().InstancePerLifetimeScope();
            builder.RegisterType<ULNDataService>().As<IULNDataService>().InstancePerLifetimeScope();
            builder.RegisterType<PostcodesDataService>().As<IPostcodesDataService>();
            builder.RegisterType<ValidationErrorsDataService>().As<IValidationErrorsDataService>();

            builder.RegisterType<AcademicYearDataService>().As<IAcademicYearDataService>().InstancePerLifetimeScope();
            builder.RegisterType<AimTypeDataService>().As<IAimTypeDataService>().InstancePerLifetimeScope();
            builder.RegisterType<CompStatusDataService>().As<ICompStatusDataService>().InstancePerLifetimeScope();
            builder.RegisterType<EmpOutcomeDataService>().As<IEmpOutcomeDataService>().InstancePerLifetimeScope();
            builder.RegisterType<FundModelDataService>().As<IFundModelDataService>().InstancePerLifetimeScope();
            builder.RegisterType<LLDDCatDataService>().As<ILLDDCatDataService>().InstancePerLifetimeScope();
            builder.RegisterType<QUALENT3DataService>().As<IQUALENT3DataService>().InstancePerLifetimeScope();
        }
    }
}
