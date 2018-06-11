using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class OneHundredThousandLearnerProvider : LearnerProviderService, IValidationItemProviderService<IEnumerable<ILearner>>
    {
        private const int LearnerCount = 100000;

        public OneHundredThousandLearnerProvider(ICache<IMessage> messageCache)
            : base(messageCache)
        {
        }

        public IEnumerable<ILearner> Provide()
        {
            var learnerList = base.Provide().ToList();

            while (learnerList.Count < LearnerCount)
            {
                learnerList.AddRange(learnerList);
            }

            return learnerList.Take(LearnerCount);
        }
    }
}
