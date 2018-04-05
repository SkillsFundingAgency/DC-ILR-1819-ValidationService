using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules.Stubs
{
    public class ValidationItemProviderServiceStub : IValidationItemProviderService<ILearner>
    {
        public IEnumerable<ILearner> Provide(IValidationContext validationContext)
        {
            var learnerList = new List<ILearner>();

            for (var i = 0; i < 20000; i++)
            {
                learnerList.Add(new TestLearner()
                {
                    LearningDeliveries = new List<TestLearningDelivery>()
                });
            }

            return learnerList;
        }
    }
}
