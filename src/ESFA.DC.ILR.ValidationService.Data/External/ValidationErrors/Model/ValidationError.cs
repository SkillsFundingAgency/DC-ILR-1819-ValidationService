using ESFA.DC.ILR.ValidationService.Interface.Enum;

namespace ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model
{
    public class ValidationError
    {
        public string RuleName { get; set; }

        public Severity Severity { get; set; }

        public string Message { get; set; }
    }
}
