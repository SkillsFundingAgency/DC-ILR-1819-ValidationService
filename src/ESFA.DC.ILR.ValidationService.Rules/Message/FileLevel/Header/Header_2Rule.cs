using System;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Header
{
    public class Header_2Rule : AbstractRule, IRule<IMessage>
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public Header_2Rule(IValidationErrorHandler validationErrorHandler, IDateTimeProvider dateTimeProvider)
            : base(validationErrorHandler, RuleNameConstants.Header_2)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public void Validate(IMessage objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            if (ConditionMet(objectToValidate.HeaderEntity.CollectionDetailsEntity.FilePreparationDate))
            {
                HandleValidationError();
            }
        }

        public bool ConditionMet(DateTime filePreparationDate)
        {
            return _dateTimeProvider.ConvertUkToUtc(filePreparationDate) > _dateTimeProvider.GetNowUtc();
        }
    }
}
