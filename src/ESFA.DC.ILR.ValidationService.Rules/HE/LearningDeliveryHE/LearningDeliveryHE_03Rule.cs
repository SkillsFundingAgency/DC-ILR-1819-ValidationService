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
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.LearningDeliveryHE
{
    public class LearningDeliveryHE_03Rule : AbstractRule, IRule<IMessage>
    {
        private readonly int[] _fundModels =
            {
                TypeOfFunding.Age16To19ExcludingApprenticeships,
                TypeOfFunding.AdultSkills,
                TypeOfFunding.NotFundedByESFA
            };

        private readonly string[] _notionalNVQLevels =
            {
                LARSNotionalNVQLevelV2.Level4,
                LARSNotionalNVQLevelV2.Level5,
                LARSNotionalNVQLevelV2.Level6,
                LARSNotionalNVQLevelV2.Level7,
                LARSNotionalNVQLevelV2.Level8,
                LARSNotionalNVQLevelV2.HigherLevel
            };

        private readonly IDD07 _dd07;
        private readonly ILARSDataService _lARSDataService;
        private readonly IDerivedData_27Rule _derivedData_27Rule;

        public LearningDeliveryHE_03Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_27Rule derivedData_27Rule,
            ILARSDataService lARSDataService,
            IDD07 dd07)
            : base(validationErrorHandler, RuleNameConstants.LearningDeliveryHE_03)
        {
            _derivedData_27Rule = derivedData_27Rule;
            _lARSDataService = lARSDataService;
            _dd07 = dd07;
        }

        public void Validate(IMessage objectToValidate)
        {
            if (objectToValidate == null
                    || DerivedData27ConditionMet(objectToValidate.LearningProviderEntity.UKPRN))
            {
                return;
            }

            foreach (var learner in objectToValidate.Learners)
            {
                foreach (var learningDelivery in learner.LearningDeliveries)
                {
                    if (ConditionMet(learningDelivery.FundModel, learningDelivery.LearnAimRef))
                    {
                        HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel));
                    }
                }
            }
        }

        public bool ConditionMet(int fundModel, string learnAimRef)
        {
            return FundModelConditionMet(fundModel)
                && LARSNotionalNVQLevelV2ConditionMet(learnAimRef);
        }

        public bool LARSNotionalNVQLevelV2ConditionMet(string learnAimRef) => _lARSDataService.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNVQLevels);

        public bool DerivedData27ConditionMet(int uKPRN) => _derivedData_27Rule.IsUKPRNCollegeOrGrantFundedProvider(uKPRN);

        public bool FundModelConditionMet(int fundModel) => _fundModels.Contains(fundModel);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
