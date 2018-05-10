using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class LearnerPerActorServiceStub<T, U> : ILearnerPerActorService<T,List<T>>
        where T: class 
    {
        private readonly IValidationItemProviderService<IEnumerable<T>> _validationItemProviderService;

        public LearnerPerActorServiceStub(
            ICache<IMessage> messageCache,
            IValidationItemProviderService<IEnumerable<T>> validationItemProviderService
            )
        {
            _validationItemProviderService = validationItemProviderService;
        }

        public IEnumerable<List<T>> Process()
        {
            var learners = _validationItemProviderService.Provide();
            var enumerableLearners = learners as T[] ?? learners.ToArray();
            var learnersPerActors = CalculateLearnersPerActor(enumerableLearners.Count());
            return SplitList(enumerableLearners.ToList(), learnersPerActors);
        }

        private int CalculateLearnersPerActor(int totalMessagesCount)
        {
            if (totalMessagesCount <= 500)
            {
                return 100;
            }
            if (totalMessagesCount <= 1700)
            {
                return 500;
            }
            if (totalMessagesCount <= 10000)
            {
                return 1000;
            }
            if (totalMessagesCount <= 30000)
            {
                return 5000;
            }

            return 10000;
        }
       

        private IEnumerable<List<T>> SplitList(List<T> locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }


    }
}
