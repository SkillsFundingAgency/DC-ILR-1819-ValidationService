using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_23Rule :
        IDerivedData_23Rule
    {
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public DerivedData_23Rule(IDateTimeQueryService dateTimeQueryService)
        {
            _dateTimeQueryService = dateTimeQueryService;
        }

        public int GetLearnersAgeAtStartOfESFContract(
            ILearner learner,
            IEnumerable<ILearningDelivery> learningDeliveries)
        {
            if (learner?.DateOfBirthNullable == null)
            {
                return default(int);
            }

            var delivery = learningDeliveries
                    ?.OrderByDescending(x => x.LearnStartDate)
                    .FirstOrDefault(ld => ld.LearnAimRef.CaseInsensitiveEquals(TypeOfAim.References.ESFLearnerStartandAssessment)
                             && ld.CompStatus == CompletionState.HasCompleted);

            if (delivery == null)
            {
                return default(int);
            }

            return _dateTimeQueryService.AgeAtGivenDate(
                learner.DateOfBirthNullable.Value,
                delivery.LearnStartDate);
        }
    }
}