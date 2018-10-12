using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.PCSLDCS
{
    public class PCSLDCS_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2009 = new DateTime(2009, 08, 01);

        private readonly ILARSDataService _lARSDataService;

        public PCSLDCS_01Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService lARSDataService)
            : base(validationErrorHandler, RuleNameConstants.PCSLDCS_01)
        {
            _lARSDataService = lARSDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.LearnStartDate,
                    learningDelivery.LearningDeliveryHEEntity,
                    learningDelivery.LearnAimRef))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(
            DateTime learnStartDate,
            ILearningDeliveryHE learningDeliveryHEEntity,
            string learnAimRef)
        {
            return StartDateConditionMet(learnStartDate)
                && LearningDeliveryHEConditionMet(learningDeliveryHEEntity)
                && LARSConditionMet(learnAimRef);
        }

        public bool LARSConditionMet(string learnAimRef)
        {
            return _lARSDataService.LearnDirectClassSystemCode2MatchForLearnAimRef(learnAimRef);
        }

        public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHEEntity)
        {
            return learningDeliveryHEEntity == null
                || (learningDeliveryHEEntity != null
                    && learningDeliveryHEEntity.PCSLDCSNullable == null);
        }

        public bool StartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _firstAugust2009;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
