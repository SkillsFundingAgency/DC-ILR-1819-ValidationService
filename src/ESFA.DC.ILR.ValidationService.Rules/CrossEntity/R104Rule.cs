using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R104Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string _famTypeACT = LearningDeliveryFAMTypeConstants.ACT;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R104Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R104)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (!ACTCountConditionMet(learningDelivery.LearningDeliveryFAMs))
                {
                    return;
                }

                var learningDeliveryFAMs = ACTDateConditionMet(learningDelivery.LearningDeliveryFAMs);

                if (learningDeliveryFAMs != null)
                {
                    HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                learningDelivery.LearnPlanEndDate,
                                learningDelivery.LearnActEndDateNullable,
                                _famTypeACT,
                                learningDeliveryFAMs.LearnDelFAMDateFromNullable,
                                learningDeliveryFAMs.LearnDelFAMDateToNullable));
                }
            }
        }

        public bool ACTCountConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            if (learningDeliveryFAMs != null)
            {
                return _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsCountByFAMType(learningDeliveryFAMs as IReadOnlyCollection<ILearningDeliveryFAM>, _famTypeACT) > 1;
            }

            return false;
        }

        public ILearningDeliveryFAM ACTDateConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            if (learningDeliveryFAMs != null)
            {
                var fams = learningDeliveryFAMs.Where(f => f.LearnDelFAMType == _famTypeACT).OrderBy(ldf => ldf.LearnDelFAMDateFromNullable).ToArray();
                var famCount = fams.Count();

                if (famCount < 2)
                {
                    return null;
                }

                var i = 0;

                while (i < famCount)
                {
                    var errorConditionMet =
                        fams[i].LearnDelFAMDateToNullable == null || fams[i + 1].LearnDelFAMDateFromNullable == null
                        ? false
                        : fams[i].LearnDelFAMDateToNullable >= fams[i + 1].LearnDelFAMDateFromNullable == true;

                    if (errorConditionMet == true)
                    {
                        return fams[i];
                    }

                    i++;
                }

                return null;
            }

            return null;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnPlanEndDate, DateTime? learnActEndDate, string famType, DateTime? learnDelFamDateFrom, DateTime? learnDelFamDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, learnPlanEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateFrom, learnDelFamDateFrom),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFamDateTo)
            };
        }
    }
}
