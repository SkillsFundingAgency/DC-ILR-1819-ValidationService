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
    public class UKPRN_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFCSDataService _fcsDataService;
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IAcademicYearQueryService _academicYearQueryService;
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { 70 };
        private readonly IEnumerable<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
            FundingStreamPeriodCodeConstants.ESF1420
        };

        public UKPRN_05Rule(
            IFCSDataService fcsDataService,
            IAcademicYearDataService academicYearDataService,
            IAcademicYearQueryService academicYearQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_05)
        {
            _fcsDataService = fcsDataService;
            _academicYearDataService = academicYearDataService;
            _academicYearQueryService = academicYearQueryService;
        }

        public UKPRN_05Rule()
          : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var academicYearStart = _academicYearDataService.Start();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => _fundModels.Contains(d.FundModel)))
            {
                if (ConditionMet(academicYearStart, learningDelivery.LearnActEndDateNullable, learningDelivery.ConRefNumber))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.ConRefNumber));
                }
            }
        }

        public bool ConditionMet(DateTime academicYearStart, DateTime? learnActEndDate, string conRefNumber)
        {
            return LearnActEndDateConditionMet(learnActEndDate, academicYearStart)
                && FCTFundingConditionMet(conRefNumber);
        }

        public virtual bool LearnActEndDateConditionMet(DateTime? learnActEndDate, DateTime academicYearStart)
        {
            return learnActEndDate == null ? true : !_academicYearQueryService.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYearStart);
        }

        public virtual bool FCTFundingConditionMet(string conRefNumber)
        {
            return !(_fcsDataService.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)
                || _fcsDataService.ConRefNumberExists(conRefNumber));
        }

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
