using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_73Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFCSDataService _fCSDataService;
        private readonly ILARSDataService _lARSDataService;

        public LearnAimRef_73Rule(
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fCSDataService,
            ILARSDataService lARSDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_73)
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
            var larsLearningDelivery = _lARSDataService.GetLearningDeliveryForLearnAimRef(learnAimRef);
            if (larsLearningDelivery == null)
            {
                return false;
            }

            decimal? sectorSubjectAreaTier1 = larsLearningDelivery.SectorSubjectAreaTier1;
            decimal? sectorSubjectAreaTier2 = larsLearningDelivery.SectorSubjectAreaTier2;
            string notionalNVQLevel2String = larsLearningDelivery.NotionalNVQLevelv2;

            if (string.IsNullOrEmpty(notionalNVQLevel2String)
                || !int.TryParse(notionalNVQLevel2String, out int notionalNVQLevel2))
            {
                return false;
            }

            return !_fCSDataService.IsSectorSubjectAreaTiersMatchingSubjectAreaCode(conRefNumber, sectorSubjectAreaTier1, sectorSubjectAreaTier2)
                || _fCSDataService.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(notionalNVQLevel2, conRefNumber);
        }

        public bool LearnAimRefConditionMet(string learnAimRef) => !(learnAimRef == ValidationConstants.ZESF0001);

        public bool FCSConditionMet(string conRefNumber) => _fCSDataService.IsSubjectAreaAndMinMaxLevelsExistsForContract(conRefNumber);

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
