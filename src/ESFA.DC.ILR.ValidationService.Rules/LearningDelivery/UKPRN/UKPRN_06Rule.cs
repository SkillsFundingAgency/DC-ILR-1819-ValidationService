using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
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
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IAcademicYearQueryService _academicYearQueryService;
        private readonly IDerivedData_07Rule _dd07;
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { 35 };
        private readonly IEnumerable<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
            FundingStreamPeriodCodeConstants.AEBC1819,
            FundingStreamPeriodCodeConstants.AEBTO_LS1819,
            FundingStreamPeriodCodeConstants.AEBTO_TOL1819
        };

        public UKPRN_06Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IFCSDataService fcsDataService,
            IDerivedData_07Rule dd07,
            IAcademicYearDataService academicYearDataService,
            IAcademicYearQueryService academicYearQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_06)
        {
            _fcsDataService = fcsDataService;
            _academicYearDataService = academicYearDataService;
            _academicYearQueryService = academicYearQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _dd07 = dd07;
        }

        public UKPRN_06Rule()
          : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            var academicYearStart = _academicYearDataService.Start();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => _fundModels.Contains(d.FundModel)))
            {
                if (ConditionMet(learningDelivery.ProgTypeNullable, academicYearStart, learningDelivery.LearnActEndDateNullable, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int? progType, DateTime academicYearStart, DateTime? learnActEndDate, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)
                && DD07ConditionMet(progType, learningDeliveryFAMs)
                && LearnActEndDateConditionMet(learnActEndDate, academicYearStart)
                && FCTFundingConditionMet();
        }

        public virtual bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034");
        }

        public virtual bool DD07ConditionMet(int? progType, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !(_dd07.IsApprenticeship(progType)
                && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "357"));
        }

        public virtual bool LearnActEndDateConditionMet(DateTime? learnActEndDate, DateTime academicYearStart)
        {
            return learnActEndDate == null ? true : !_academicYearQueryService.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYearStart);
        }

        public virtual bool FCTFundingConditionMet()
        {
            return !_fcsDataService.FundingRelationshipFCTExists(_fundingStreamPeriodCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
