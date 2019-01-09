using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_72Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFCSDataService _fCSDataService;
        private readonly ILARSDataService _lARSDataService;

        public LearnAimRef_72Rule(
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fCSDataService,
            ILARSDataService lARSDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_72)
        {
            _fCSDataService = fCSDataService;
            _lARSDataService = lARSDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.ConRefNumber, learningDelivery.LearnAimRef))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.ConRefNumber));
                }
            }
        }

        public bool ConditionMet(int fundModel, string conRefNumber, string learnAimRef)
        {
            return FundModelConditionMet(fundModel)
                && FCSConditionMet(conRefNumber)
                && LARSConditionMet(conRefNumber, learnAimRef)
                && LearnAimRefConditionMet(learnAimRef);
        }

        public bool LARSConditionMet(string conRefNumber, string learnAimRef)
        {
            string notionalNVQLevel2String = _lARSDataService.GetNotionalNVQLevelv2ForLearnAimRef(learnAimRef);

            if (string.IsNullOrEmpty(notionalNVQLevel2String)
                || !int.TryParse(notionalNVQLevel2String, out int notionalNVQLevel2))
            {
                return false;
            }

            return _fCSDataService.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(notionalNVQLevel2, conRefNumber);
        }

        public bool LearnAimRefConditionMet(string learnAimRef) => !learnAimRef.CaseInsensitiveEquals(ValidationConstants.ZESF0001);

        public bool FCSConditionMet(string conRefNumber) => _fCSDataService.IsSectorSubjectAreaCodeNullForContract(conRefNumber);

        public bool FundModelConditionMet(int fundModel) => fundModel == TypeOfFunding.EuropeanSocialFund;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string conRefNumber)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, conRefNumber)
            };
        }
    }
}
