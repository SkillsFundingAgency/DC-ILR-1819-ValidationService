using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.AddLine1
{
    /// <summary>
    /// If Learner.AddLine1 is unknown
    /// </summary>
    public class AddLine1_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public AddLine1_03Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService)
            : base(validationErrorHandler)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.AddLine1) && !Exclude(objectToValidate.LearningDeliveries, objectToValidate.PlanLearnHoursNullable))
            {
                HandleValidationError(RuleNameConstants.AddLine1_03Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(string addLine1)
        {
            return string.IsNullOrWhiteSpace(addLine1);
        }

        public bool Exclude(IReadOnlyCollection<ILearningDelivery> learningDeliveries, long? planLearnHours)
        {
            return learningDeliveries != null &&
                    ExcludeConditionPlannedLearnHours(planLearnHours) &&
                   learningDeliveries.All(x => ExcludeConditionFamValueMet(x.FundModelNullable, x.LearningDeliveryFAMs));
        }

        public bool ExcludeConditionFamValueMet(long? fundModel, IReadOnlyCollection<ILearningDeliveryFAM> fams)
        {
            return fundModel.HasValue &&
                   (fundModel.Value == 10 ||
                       (fundModel.Value == 99 && _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(fams, LearningDeliveryFAMTypeConstants.SOF, "108")));
        }

        public bool ExcludeConditionPlannedLearnHours(long? planLearnHours)
        {
            return planLearnHours.HasValue && planLearnHours.Value <= 10;
        }
    }
}