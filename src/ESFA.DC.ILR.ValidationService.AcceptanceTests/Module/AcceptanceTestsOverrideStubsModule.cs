using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.ILR.ValidationService.Stubs;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Xml;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class AcceptanceTestsOverrideStubsModule : Module
    {
        public const string AcceptanceTestsKey = "AcceptanceTests";

        protected override void Load(ContainerBuilder builder)
        {
            // Override stubs provided elsewhere and key to acceptance tests
            builder.RegisterType<LARSDataService>().As<ILARSDataService>()
                .Keyed<ILARSDataService>(AcceptanceTestsKey)
                .WithParameter(
                    new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IExternalDataCache),
                    (pi, ctx) => ctx.ResolveKeyed<IExternalDataCache>(AcceptanceTestsOverrideStubsModule.AcceptanceTestsKey)))
                .InstancePerLifetimeScope();

            builder.RegisterType<AcceptanceTestsExternalDataCache>().As<IExternalDataCache>()
                .Keyed<IExternalDataCache>(AcceptanceTestsKey)
                .InstancePerLifetimeScope();

            builder.RegisterType<AcceptanceTestsExternalDataCachePopulationService>()
                .As<IExternalDataCachePopulationService>()
                .WithParameter(
                    new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IExternalDataCache),
                    (pi, ctx) => ctx.ResolveKeyed<IExternalDataCache>(AcceptanceTestsOverrideStubsModule.AcceptanceTestsKey)))
                .Keyed<IExternalDataCachePopulationService>(AcceptanceTestsKey)
                .InstancePerLifetimeScope();
        }
    }
}
