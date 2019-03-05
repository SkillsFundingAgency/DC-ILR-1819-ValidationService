using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_67Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _fundingModel = 36;
        private readonly int _aimType = 3;

        private readonly int[] _basicSkills =
            { 01, 11, 13, 20, 23, 24, 29, 31, 02, 12, 14, 19, 21, 25, 30, 32, 33, 34, 35 };

        private readonly ILARSDataService _larsDataService;

        public LearnDelFAMType_67Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_67)
        {
            _larsDataService = larsDataService;
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="learner">The object to validate.</param>
        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.FundModel != _fundingModel
                    || learningDelivery.AimType != _aimType
                    || learningDelivery.LearningDeliveryFAMs == null)
                {
                    continue;
                }

                var basicSkill = _larsDataService
                    .BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkills, learningDelivery.LearnAimRef, learningDelivery.LearnStartDate);

                foreach (var deliveryFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if (!basicSkill && deliveryFam.LearnDelFAMType == LearningDeliveryFAMTypeConstants.LSF)
                    {
                        RaiseValidationMessage(learner.LearnRefNumber, learningDelivery, deliveryFam);
                    }
                }
            }
        }

        private void RaiseValidationMessage(string learnRefNum, ILearningDelivery learningDelivery, ILearningDeliveryFAM thisMonitor)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, thisMonitor.LearnDelFAMType)
            };

            HandleValidationError(learnRefNum, learningDelivery.AimSeqNumber, parameters);
        }
    }
}
