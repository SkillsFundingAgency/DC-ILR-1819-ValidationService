using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_53Rule : AbstractRule, IRule<ILearner>
    {
        private readonly List<string> _fundingStreamPeriodCodes = new List<string>
        {
            FundingStreamPeriodCodeConstants.ALLBC1819
        };

        private readonly IFCSDataService _fcsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnDelFAMType_53Rule(
            IFCSDataService fcsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_53)
        {
            _fcsDataService = fcsDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.LearningDeliveryFAMs != null))
            {
                if (ConditionMet(learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(LearningDeliveryFAMTypeConstants.ALB));
                }
            }
        }

        public bool ConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return LearningDeliveryFAMsConditionMet(learningDeliveryFAMs) && FCTFundingConditionMet();
        }

        public virtual bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ALB, "1")
                || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ALB, "3");
        }

        public virtual bool FCTFundingConditionMet()
        {
            return _fcsDataService.FundingRelationshipFCTExists(_fundingStreamPeriodCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learningDelFAMType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learningDelFAMType)
            };
        }
    }
}
