using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R64Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> ValidFundModels = new HashSet<int>() { 35, 36 };
        private readonly HashSet<int> ValidComponentTypes = new HashSet<int>() { 1, 3 };
        private const int ValidAimType = 3;
        private const int ExcludedProgType = 25;

        public R64Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R64)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.AimType))
                {
                    HandleValidationError(learnRefNumber: objectToValidate.LearnRefNumber, aimSequenceNumber: learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            var filteredLearningDeliveries = learningDeliveries.Where(x => x.AimType == ValidAimType &&
                                                                           ValidFundModels.Contains(x.FundModel) &&
                                                                           ValidComponentTypes.Contains(x.)
                                                                           )
        }

        public bool ExcludeConditionMet(int? progType)
        {
            return progType.HasValue && progType == ExcludedProgType;
        }
    }
}
