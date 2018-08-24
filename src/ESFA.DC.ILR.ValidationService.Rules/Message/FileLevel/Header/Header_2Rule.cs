using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Header
{
    public class Header_2Rule : AbstractRule, IRule<IMessage>
    {
        public Header_2Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Header_2)
        {
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
            return filePreparationDate > DateTime.Now;
        }
    }
}
