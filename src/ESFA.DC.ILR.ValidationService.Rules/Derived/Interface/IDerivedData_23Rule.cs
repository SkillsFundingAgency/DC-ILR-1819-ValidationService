using ESFA.DC.ILR.Model.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_23Rule
    {
        int GetLearnersAgeAtStartOfESFContract(
            ILearner learner,
            IEnumerable<ILearningDelivery> learningDeliveries);
    }
}
