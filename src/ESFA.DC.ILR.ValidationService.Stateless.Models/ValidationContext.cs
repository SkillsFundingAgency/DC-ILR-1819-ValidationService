using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class ValidationContext : IValidationContext
    {
        public IMessage Input { get; set; }

        public IInternalDataCache InternalDataCache { get; set; }

        public IExternalDataCache ExternalDataCache { get; set; }

        public string InvalidLearnRefNumbersKey { get; set; }

        public string ValidLearnRefNumbersKey { get; set; }

        public string ValidationErrorsKey { get; set; }

        public string ValidationErrorMessageLookupKey { get; set; }
    }
}