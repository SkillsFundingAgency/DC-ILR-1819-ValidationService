using System;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_23Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public DateOfBirth_23Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
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
                    if (ConditionMet(objectToValidate.DateOfBirthNullable, learningDelivery.FundModelNullable, _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ADL)))
                    {
                        HandleValidationError(RuleNameConstants.DateOfBirth_23, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                    }
                }
            }
        }

        public bool Exclude(ILearningDelivery learningDelivery)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034");
        }

        public bool ConditionMet(DateTime? dateofBirth, long? fundModel, bool hasADL)
        {
            return !dateofBirth.HasValue
                && fundModel.HasValue
                && fundModel == 99
                && hasADL;
        }
    }
}