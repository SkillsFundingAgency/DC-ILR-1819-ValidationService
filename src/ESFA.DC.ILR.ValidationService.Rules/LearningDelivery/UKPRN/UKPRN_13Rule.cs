using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_13Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
            FundingStreamPeriodCodeConstants.C1618_NLAP2018,
            FundingStreamPeriodCodeConstants.ANLAP2018
        };

        private readonly DateTime _firstJan2018 = new DateTime(2018, 01, 01);

        private readonly IFileDataService _fileDataService;
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IAcademicYearQueryService _academicYearQueryService;
        private readonly IFCSDataService _fcsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public UKPRN_13Rule(
            IFileDataService fileDataService,
            IAcademicYearDataService academicYearDataService,
            IAcademicYearQueryService academicYearQueryService,
            IFCSDataService fcsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_13)
        {
            _fileDataService = fileDataService;
            _academicYearDataService = academicYearDataService;
            _academicYearQueryService = academicYearQueryService;
            _fcsDataService = fcsDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public UKPRN_13Rule()
           : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var ukprn = _fileDataService.UKPRN();
            var academicYearStart = _academicYearDataService.Start();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.LearningDeliveryFAMs != null))
            {
                if (ConditionMet(learningDelivery.LearnStartDate, academicYearStart, learningDelivery.LearnActEndDateNullable, learningDelivery.AimType, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate, ukprn, "ACT", "2", learningDelivery.AimType));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, DateTime academicYearStart, DateTime? learnActEndDate, int aimType, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return AimTypeConditionMet(aimType)
                && LearnActEndDateConditionMet(learnActEndDate, academicYearStart)
                && LearnStartDateConditionMet(learnStartDate)
                && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)
                && FCTFundingConditionMet();
        }

        public virtual bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public virtual bool LearnActEndDateConditionMet(DateTime? learnActEndDate, DateTime academicYearStart)
        {
            return learnActEndDate == null ? true : !_academicYearQueryService.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYearStart);
        }

        public virtual bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _firstJan2018;
        }

        public virtual bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "ACT", "2")
                && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "358")
                && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "RES", "1");
        }

        public virtual bool FCTFundingConditionMet()
        {
            return !_fcsDataService.FundingRelationshipFCTExists(_fundingStreamPeriodCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int ukprn, string learningDelFAMType, string learningDelFAMCode, int aimType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ukprn),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learningDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learningDelFAMCode),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType)
            };
        }
    }
}
