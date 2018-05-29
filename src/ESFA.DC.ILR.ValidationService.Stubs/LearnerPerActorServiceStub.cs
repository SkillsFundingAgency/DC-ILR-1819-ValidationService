using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class LearnerPerActorServiceStub<T> : ILearnerPerActorService<T, IEnumerable<T>>
        where T : class
    {
        private readonly IValidationItemProviderService<IEnumerable<T>> _validationItemProviderService;

        public LearnerPerActorServiceStub(IValidationItemProviderService<IEnumerable<T>> validationItemProviderService)
        {
            _validationItemProviderService = validationItemProviderService;
        }

        public IEnumerable<IEnumerable<T>> Process()
        {
            var learners = _validationItemProviderService.Provide().ToList();

            var learnersPerActors = CalculateLearnersPerActor(learners.Count);

            return SplitList(learners, learnersPerActors);
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

        private IEnumerable<IEnumerable<T>> SplitList(IEnumerable<T> learners, int nSize = 30)
        {
            var learnerList = learners.ToList();

            for (var i = 0; i < learnerList.Count; i += nSize)
            {
                yield return learnerList.GetRange(i, Math.Min(nSize, learnerList.Count - i));
            }
        }
    }
}
