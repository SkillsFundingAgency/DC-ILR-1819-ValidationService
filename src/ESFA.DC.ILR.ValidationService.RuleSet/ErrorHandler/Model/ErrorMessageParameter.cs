using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model
{
    public struct ErrorMessageParameter : IErrorMessageParameter
    {
        public ErrorMessageParameter(string propertyName, string value)
        {
            PropertyName = propertyName;
            Value = value;
        }

        public string PropertyName { get; set; }

        public string Value { get; set; }
    }
}
