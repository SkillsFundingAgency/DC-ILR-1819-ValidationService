using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.FamilyName
{
    public class FamilyName_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IULNDataService _ulnDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public FamilyName_04Rule(IULNDataService ulnDataService, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FamilyName_04)
        {
            _ulnDataService = ulnDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.FamilyName, objectToValidate.PlanLearnHoursNullable, objectToValidate.ULN, objectToValidate.LearningDeliveries))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.FamilyName, objectToValidate.PlanLearnHoursNullable));
            }
        }

        public bool ConditionMet(string familyName, int? planLearnHours, long uln, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return FamilyNameConditionMet(familyName)
                && PlanLearnHoursConditionMet(planLearnHours)
                && ULNConditionMet(uln)
                && CrossLearningDeliveryConditionMet(learningDeliveries);
        }

        public bool FamilyNameConditionMet(string familyName)
        {
            return string.IsNullOrWhiteSpace(familyName);
        }

        public bool PlanLearnHoursConditionMet(int? planLearnHours)
        {
            return planLearnHours.HasValue && planLearnHours <= 10;
        }

        public bool ULNConditionMet(long uln)
        {
            return uln != ValidationConstants.TemporaryULN
                && _ulnDataService.Exists(uln);
        }

        public bool CrossLearningDeliveryConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries.All(ld => ld.FundModel == 10
                || learningDeliveries.All(ldf => ldf.FundModel == 99 && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(ld.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "108")));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string familyName, int? planLearnHours)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FamilyName, familyName),
                BuildErrorMessageParameter(PropertyNameConstants.PlanLearnHours, planLearnHours)
            };
        }
    }
}