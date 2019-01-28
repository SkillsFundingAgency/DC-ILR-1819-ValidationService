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

        private static readonly Dictionary<string, int> _eligibilityRulePriorAttainValuesMapping = new Dictionary<string, int>
        {
            { "99", 1 },
            { "9", 2 },
            { "7", 3 },
            { "1", 4 },
            { "2", 5 },
            { "3", 6 },
            { "H", 7 }
        };

        private static readonly Dictionary<int, int> _learnerPriorAttainValuesMapping = new Dictionary<int, int>
        {
            { 98, 0 },
            { 97, 0 },
            { 99, 1 },
            { 9, 2 },
            { 7, 3 },
            { 1, 4 },
            { 2, 5 },
            { 3, 6 },
            { 4, 7 },
            { 5, 8 },
            { 10, 9 },
            { 11, 10 },
            { 12, 11 },
            { 13, 12 },
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
            var contractAllocation = _fcsDataService.GetContractAllocationFor(conRefNumber);

            var minPriorAttainment = !string.IsNullOrWhiteSpace(contractAllocation?.EsfEligibilityRule?.MinPriorAttainment) ?
                                        _eligibilityRulePriorAttainValuesMapping[contractAllocation?.EsfEligibilityRule?.MinPriorAttainment] : 0;
            var maxPriorAttainment = !string.IsNullOrWhiteSpace(contractAllocation?.EsfEligibilityRule?.MaxPriorAttainment) ?
                                        _eligibilityRulePriorAttainValuesMapping[contractAllocation?.EsfEligibilityRule?.MaxPriorAttainment] : 0;

            if ((minPriorAttainment == 0 && maxPriorAttainment == 0) || !priorAttain.HasValue)
            {
                return false;
            }

            if (_learnerPriorAttainValuesMapping.TryGetValue(priorAttain.Value, out var learnerPriorAttainValue))
            {
                return learnerPriorAttainValue < minPriorAttainment || learnerPriorAttainValue > maxPriorAttainment;
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
