using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R91Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _fundModel = TypeOfFunding.EuropeanSocialFund;

        public R91Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R91)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel,  objectToValidate.LearningDeliveries))
                {
                    HandleValidationError(learnRefNumber: objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(
                        learningDelivery.FundModel,
                        learningDelivery.ConRefNumber,
                        learningDelivery.CompStatus));
                    return;
                }
            }
        }

        public bool ConditionMet(int fundModel, IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            return FundModelConditionMet(fundModel)
                && ConRefConditionMet(learningDeliveries);
        }

        public bool ConRefConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries.Where(l => l.FundModel == _fundModel
                && l.LearnAimRef == TypeOfAim.References.ESFLearnerStartandAssessment
                && l.CompStatus == CompletionState.HasCompleted)
                .ToList()?.GroupBy(l => new { l.FundModel, l.LearnAimRef, l.CompStatus, l.ConRefNumber })
                .Select(g => Tuple.Create(g.Key, g.Count()))
                .Any(l => l.Item2 <= 1) ?? false;
        }

        public bool FundModelConditionMet(int fundModel) => _fundModel == fundModel;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string conRefNumber, int compStatus)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, conRefNumber),
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus)
            };
        }
    }
}
