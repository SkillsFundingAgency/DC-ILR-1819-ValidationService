﻿using Autofac;
using ESFA.DC.ILR.ValidationService.Data;
using ESFA.DC.ILR.ValidationService.Data.External.EDRS;
using ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
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
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
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
            builder.RegisterType<EPAOrganisationDataService>().As<IEPAOrganisationDataService>().InstancePerLifetimeScope();
            builder.RegisterType<ULNDataService>().As<IULNDataService>().InstancePerLifetimeScope();
            builder.RegisterType<PostcodesDataService>().As<IPostcodesDataService>();
            builder.RegisterType<ValidationErrorsDataService>().As<IValidationErrorsDataService>();
            builder.RegisterType<FCSDataService>().As<IFCSDataService>().InstancePerLifetimeScope();
            builder.RegisterType<EmployersDataService>().As<IEmployersDataService>().InstancePerLifetimeScope();

            builder.RegisterType<AcademicYearDataService>().As<IAcademicYearDataService>().InstancePerLifetimeScope();
            builder.RegisterType<LookupDetailsProvider>().As<IProvideLookupDetails>().InstancePerLifetimeScope();
        }
    }
}
