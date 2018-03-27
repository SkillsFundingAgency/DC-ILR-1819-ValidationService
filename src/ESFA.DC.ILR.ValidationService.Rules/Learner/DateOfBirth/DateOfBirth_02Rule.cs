using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        private readonly IEnumerable<long> _fundModels = new HashSet<long> { 10, 99 };

        public DateOfBirth_02Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(ld => !Exclude(ld)))
                {
                    if (ConditionMet(learningDelivery.FundModelNullable, objectToValidate.DateOfBirthNullable))
                    {
                        HandleValidationError(RuleNameConstants.DateOfBirth_02, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                    }
                }
            }
        }

        public bool ConditionMet(long? fundModel, DateTime? dateOfBirth)
        {
            return !dateOfBirth.HasValue
                && fundModel.HasValue
                && _fundModels.Contains(fundModel.Value);
        }

        public bool Exclude(ILearningDelivery learningDelivery)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ADL);
        }
    }
}