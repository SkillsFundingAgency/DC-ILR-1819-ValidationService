using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R106Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _augFirst2016 = new DateTime(2016, 8, 1);
        private readonly string _famTypeLSF = LearningDeliveryFAMTypeConstants.LSF;

        public R106Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R106)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var learningDeliveryFAMs = objectToValidate.LearningDeliveries
                .Where(l => l.LearningDeliveryFAMs != null)
                .SelectMany(ld => ld.LearningDeliveryFAMs);
            if (ConditionMet(learningDeliveryFAMs))
            {
                HandleValidationError(objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            if (learningDeliveryFAMs != null)
            {
                return learningDeliveryFAMs.Where(ld => ld.LearnDelFAMDateFromNullable >= _augFirst2016).Any()
                        ? learningDeliveryFAMs.Where(ld => ld.LearnDelFAMType == _famTypeLSF).Count() > 1
                        : false;
            }

            return false;
        }
    }
}
