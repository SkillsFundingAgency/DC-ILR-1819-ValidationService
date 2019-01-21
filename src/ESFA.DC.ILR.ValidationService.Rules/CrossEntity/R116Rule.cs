using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R116Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _fundModel = TypeOfFunding.ApprenticeshipsFrom1May2017;
        private readonly HashSet<int> _progTypes = new HashSet<int>()
        {
            TypeOfLearningProgramme.AdvancedLevelApprenticeship,
            TypeOfLearningProgramme.IntermediateLevelApprenticeship,
            TypeOfLearningProgramme.HigherApprenticeshipLevel4,
            TypeOfLearningProgramme.HigherApprenticeshipLevel5,
            TypeOfLearningProgramme.HigherApprenticeshipLevel6,
            TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus
        };

        private readonly HashSet<int> _AFinTypes1And2 = new HashSet<int>()
        {
            ApprenticeshipFinancialRecord.PaymentRecordCodes.TrainingPayment,
            ApprenticeshipFinancialRecord.PaymentRecordCodes.AssessmentPayment
        };

        public R116Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R116)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            return false;
        }
    }
}
