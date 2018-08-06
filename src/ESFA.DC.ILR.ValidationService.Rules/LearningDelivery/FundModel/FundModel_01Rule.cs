using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.FundModel.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel
{
    public class FundModel_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFundModelDataService _fundModelDataService;

        public FundModel_01Rule(IFundModelDataService fundModelDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FundModel_01)
        {
            _fundModelDataService = fundModelDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int fundModel)
        {
            return !_fundModelDataService.Exists(fundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
