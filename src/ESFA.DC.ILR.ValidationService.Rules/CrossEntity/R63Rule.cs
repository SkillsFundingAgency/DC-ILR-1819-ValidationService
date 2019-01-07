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
    public class R63Rule : AbstractRule, IRule<ILearner>
    {
        public R63Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R63)
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

        public bool ConditionMet(int fundModel, int aimType) => fundModel == TypeOfFunding.Age16To19ExcludingApprenticeships
            && !(aimType == TypeOfAim.CoreAim16To19ExcludingApprenticeships);
    }
}
