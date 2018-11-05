using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Header
{
    public class Header_3Rule : AbstractRule, IRule<IMessage>
    {
        private readonly IFileDataService _fileDataService;

        public Header_3Rule(
            IFileDataService fileDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Header_3)
        {
            _fileDataService = fileDataService;
        }

        public void Validate(IMessage objectToValidate)
        {
            if (ConditionMet(objectToValidate.HeaderEntity.SourceEntity.UKPRN, _fileDataService.FileNameUKPRN()))
            {
                HandleValidationError();
            }
        }

        public bool ConditionMet(int headerUKPRN, int? fileNameUKPRN)
        {
            return !fileNameUKPRN.HasValue
                || (fileNameUKPRN.HasValue && headerUKPRN != fileNameUKPRN);
        }
    }
}
