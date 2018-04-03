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
using ESFA.DC.ILR.ValidationService.InternalData.AcademicYearCalendarService;
using ESFA.DC.ILR.ValidationService.InternalData.ContPrefType;
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
            builder.RegisterType<ReferenceDataCacheStub>().As<IReferenceDataCache>();
            builder.RegisterType<ReferenceDataCachePopulationServiceStub>().As<IReferenceDataCachePopulationService<ILearner>>();
            builder.RegisterType<FileDataService>().As<IFileDataService>();
            builder.RegisterType<OrganisationReferenceDataService>().As<IOrganisationReferenceDataService>();
            builder.RegisterType<ULNReferenceDataService>().As<IULNReferenceDataService>();

            builder.RegisterType<ContactPreferenceInternalDataService>().As<IContactPreferenceInternalDataService>();
            builder.RegisterType<LearnFAMTypeCodeInternalDataService>().As<ILearnFAMTypeCodeInternalDataService>();
            builder.RegisterType<LlddCatInternalDataService>().As<ILlddCatInternalDataService>();
            builder.RegisterType<PriorAttainInternalDataService>().As<IPriorAttainInternalDataService>();

            builder.RegisterType<ValidationDataService>().As<IValidationDataService>();
            builder.RegisterType<AcademicYearCalendarService>().As<IAcademicYearCalendarService>();
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>();
        }
    }
}
