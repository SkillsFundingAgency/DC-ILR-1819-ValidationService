using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R116Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _fundModel = TypeOfFunding.ApprenticeshipsFrom1May2017;
        private readonly int _aimType = TypeOfAim.ProgrammeAim;
        private readonly HashSet<int> _aFinCodes3 = new HashSet<int>()
        {
            ApprenticeshipFinancialRecord.PaymentRecordCodes.EmployerPaymentReimbursedByProvider
        };

        private readonly HashSet<int> _aFinCodes1And2 = new HashSet<int>()
        {
            ApprenticeshipFinancialRecord.PaymentRecordCodes.TrainingPayment,
            ApprenticeshipFinancialRecord.PaymentRecordCodes.AssessmentPayment
        };

        private readonly HashSet<int?> _progTypes = new HashSet<int?>()
        {
            TypeOfLearningProgramme.AdvancedLevelApprenticeship,
            TypeOfLearningProgramme.IntermediateLevelApprenticeship,
            TypeOfLearningProgramme.HigherApprenticeshipLevel4,
            TypeOfLearningProgramme.HigherApprenticeshipLevel5,
            TypeOfLearningProgramme.HigherApprenticeshipLevel6,
            TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus
        };

        public R116Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R116)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var groupSums in objectToValidate.LearningDeliveries
                .Where(
                d => d.FundModel == _fundModel
                && d.AimType == _aimType
                && _progTypes.Contains(d.ProgTypeNullable)
                && d.AppFinRecords != null)
                .GroupBy(d => new
                {
                    progType = d.ProgTypeNullable,
                    fWorkCode = d.FworkCodeNullable,
                    pWayCode = d.PwayCodeNullable
                }).ToList()
                .Select(d => new
                {
                    aFinCode1And2Sum = GetSumsForAFinCode(d, _aFinCodes1And2),
                    aFinCode3Sum = GetSumsForAFinCode(d, _aFinCodes3),
                    aimSeqNumber = d.FirstOrDefault().AimSeqNumber
                }))
            {
                if (IsAFinCodeSumNegative(groupSums.aFinCode1And2Sum, groupSums.aFinCode3Sum))
                {
                    HandleValidationError(
                        learnRefNumber: objectToValidate.LearnRefNumber,
                        aimSequenceNumber: groupSums.aimSeqNumber);
                }
            }
        }

        public bool IsAFinCodeSumNegative(int aFinCode1And2Sum, int aFinCode3Sum)
        {
            return (aFinCode1And2Sum - aFinCode3Sum) < 0;
        }

        public int GetSumsForAFinCode(IEnumerable<ILearningDelivery> learningDeliveries, HashSet<int> aFinCodes)
        {
            return learningDeliveries.SelectMany(a => a.AppFinRecords)
                    .Where(f => f.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.PaymentRecord)
                        && aFinCodes.Contains(f.AFinCode)).Sum(f => f.AFinAmount);
        }
    }
}
