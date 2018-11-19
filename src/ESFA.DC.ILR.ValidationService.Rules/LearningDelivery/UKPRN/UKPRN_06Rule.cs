using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IFCSDataService _fcsDataService;
        private readonly IDD07 _dd07;
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { 35 };
        private readonly IEnumerable<string> _fundingStreamPeriodCodes = new HashSet<string> { "AEBC1819" };

        public UKPRN_06Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IDD07 dd07,
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fcsDataService = null)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_06)
        {
            _fcsDataService = fcsDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => _fundModels.Contains(d.FundModel) && d.LearningDeliveryFAMs != null))
            {
                if (ConditionMet(learningDelivery.ProgTypeNullable, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, LearningDeliveryFAMTypeConstants.LDM, "034"));
                }
            }
        }

        public bool ConditionMet(int? progType, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)
                && DD07ConditionMet(progType)
                && FCTFundingConditionMet();
        }

        public bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034")
                && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "357");
        }

        public bool DD07ConditionMet(int? progType)
        {
            return !_dd07.IsApprenticeship(progType);
        }

        public bool FCTFundingConditionMet()
        {
            return _fcsDataService.FundingRelationshipFCTExists(_fundingStreamPeriodCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string learningDelFAMType, string learningDelFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learningDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learningDelFAMCode)
            };
        }
    }
}
