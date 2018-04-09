using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules.Stubs
{
    public class LearnerProviderServiceStub : IValidationItemProviderService<IEnumerable<ILearner>>
    {
        private readonly IValidationItemProviderService<IMessage> _messageValidationItemProviderService;

        public LearnerProviderServiceStub(IValidationItemProviderService<IMessage> messageValidationItemProviderService)
        {
            _messageValidationItemProviderService = messageValidationItemProviderService;
        }

        public IEnumerable<ILearner> Provide(IValidationContext validationContext)
        {
            var message = _messageValidationItemProviderService.Provide(validationContext);

            var learners = message.Learners.ToList();

            learners.AddRange(learners);
            learners.AddRange(learners);
            learners.AddRange(learners);
            learners.AddRange(learners);
            learners.AddRange(learners);
            learners.AddRange(learners);
            learners.AddRange(learners);
            learners.AddRange(learners);
            learners.AddRange(learners);
            learners.AddRange(learners);
            learners.AddRange(learners);
            learners.AddRange(learners);

            return learners;
        }
    }
}
