﻿using Autofac;
using ESFA.DC.Data.ILR.ValidationErrors.Model;
using ESFA.DC.Data.ILR.ValidationErrors.Model.Interfaces;
using ESFA.DC.Data.LARS.Model;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.Data.Postcodes.Model;
using ESFA.DC.Data.Postcodes.Model.Interfaces;
using ESFA.DC.Data.ULN.Model;
using ESFA.DC.Data.ULN.Model.Interfaces;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Configuration;
using ESFA.DC.ILR.ValidationService.Data.Population.Configuration.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ReferenceData.Employers.Model;
using ESFA.DC.ReferenceData.Employers.Model.Interface;
using ESFA.DC.ReferenceData.EPA.Model;
using ESFA.DC.ReferenceData.EPA.Model.Interface;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.ReferenceData.Organisations.Model;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using ESFA.DC.ServiceFabric.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ValidationService.Modules.Stateless
{
    public class PreValidationDataModule : BaseDataModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configHelper = new ConfigurationHelper();

            var referenceDataOptions = configHelper.GetSectionValues<ReferenceDataOptions>("ReferenceDataSection");
            builder.RegisterInstance(referenceDataOptions).As<IReferenceDataOptions>().SingleInstance();

            builder.RegisterType<PreValidationPopulationService>().As<IPopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<InternalDataCachePopulationService>().As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<InternalDataCachePopulationService>().As<ICreateInternalDataCache>().InstancePerLifetimeScope();

            builder.Register(c => new LARS(c.Resolve<IReferenceDataOptions>().LARSConnectionString)).As<ILARS>().InstancePerLifetimeScope();
            builder.Register(c => new ULN(c.Resolve<IReferenceDataOptions>().ULNConnectionstring)).As<IULN>().InstancePerLifetimeScope();
            builder.Register(c => new Postcodes(c.Resolve<IReferenceDataOptions>().PostcodesConnectionString)).As<IPostcodes>().InstancePerLifetimeScope();

            builder.Register(c =>
            {
                DbContextOptions<OrganisationsContext> options = new DbContextOptionsBuilder<OrganisationsContext>()
                    .UseSqlServer(c.Resolve<IReferenceDataOptions>().OrganisationsConnectionString).Options;

                return new OrganisationsContext(options);
            }).As<IOrganisationsContext>().InstancePerLifetimeScope();

            builder.Register(c =>
            {
                DbContextOptions<EmployersContext> options = new DbContextOptionsBuilder<EmployersContext>()
                    .UseSqlServer(c.Resolve<IReferenceDataOptions>().EmployersConnectionString).Options;

                return new EmployersContext(options);
            }).As<IEmployersContext>().InstancePerLifetimeScope();

            builder.Register(c => new FcsContext(c.Resolve<IReferenceDataOptions>().FCSConnectionString)).As<IFcsContext>().InstancePerLifetimeScope();
            builder.Register(c => new EpaContext(c.Resolve<IReferenceDataOptions>().EPAConnectionString)).As<IEpaContext>().InstancePerLifetimeScope();
            builder.Register(c => new ValidationErrors(c.Resolve<IReferenceDataOptions>().ValidationErrorsConnectionString)).As<IValidationErrors>().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
