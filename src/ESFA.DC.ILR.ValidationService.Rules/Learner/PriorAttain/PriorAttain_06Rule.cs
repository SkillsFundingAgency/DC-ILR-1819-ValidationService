using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain
{
    public class PriorAttain_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFCSDataService _fcsDataService;

        private static readonly Dictionary<string, string> _priorAttainLevelsMapping = new Dictionary<string, string>
        {
            { "99", "1" },
            { "9", "2" },
            { "7", "3" },
            { "1", "4" },
            { "2", "5" },
            { "3", "6" },
            { "H", "7" }
        };

        public PriorAttain_06Rule(
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fcsDataService)
            : base(validationErrorHandler, RuleNameConstants.PriorAttain_06)
        {
            _fcsDataService = fcsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    objectToValidate.PriorAttainNullable,
                    learningDelivery.FundModel,
                    learningDelivery.ConRefNumber))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int? priorAttain, int fundModel, string conRefNumber)
        {
            return PriorAttainConditionMet(priorAttain, conRefNumber)
                   && FundModelConditionMet(fundModel);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == TypeOfFunding.EuropeanSocialFund;
        }

        public bool PriorAttainConditionMet(int? priorAttain, string conRefNumber)
        {
            var minPriorAttainment = _fcsDataService.GetMinPriorAttainment(conRefNumber);
            var maxPriorAttainment = _fcsDataService.GetMaxPriorAttainment(conRefNumber);

            if (string.IsNullOrEmpty(minPriorAttainment) && string.IsNullOrEmpty(maxPriorAttainment))
            {
                return false;
            }

            var mappedPriorAttainValue = _priorAttainLevelsMapping.FirstOrDefault(x => x.Key == priorAttain.ToString()).Value;

            // todo: create an extension to convert string to int??
            if (Convert.ToInt16(mappedPriorAttainValue) < Convert.ToInt16(minPriorAttainment) ||
                Convert.ToInt16(mappedPriorAttainValue) > Convert.ToInt16(maxPriorAttainment))
            {
                return true;
            }

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? priorAttain, int fundModel, string conRefNumber)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, priorAttain),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, conRefNumber)
            };
        }
    }
}
