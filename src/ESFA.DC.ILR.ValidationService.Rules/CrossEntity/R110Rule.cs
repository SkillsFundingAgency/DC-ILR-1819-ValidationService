using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R110Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public R110Rule(ILearningDeliveryFAMQueryService learningDeliveryFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R110)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
        }

        // If an apprenticeship programme is funded through a contract for services with an employer
        //     , the learner must have an employment status of 'Employed
        // "Where FundModel = 36 and LearnDelFAMType = ACT and LearnDelFAMCode = 1
        // Error if the EmpStat applicable on LearnDelFAMDateFrom is not EmpStat = 10"

        public bool IsApprenticeshipProgrammeFundedThroughContract(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017
                   && _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, Monitoring.Delivery.Types.ApprenticeshipContract, "1")
         }
    }
}
