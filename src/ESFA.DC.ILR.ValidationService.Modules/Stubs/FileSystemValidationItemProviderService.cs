using System.Collections.Generic;
using System.IO;
using System.Linq;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Modules.Stubs
{
    public class FileSystemValidationItemProviderService : IValidationItemProviderService<ILearner>
    {
        private readonly ISerializationService _serializationService;

        public FileSystemValidationItemProviderService(ISerializationService serializationService)
        {
            _serializationService = serializationService;
        }

        public IEnumerable<ILearner> Provide(IValidationContext validationContext)
        {
            var message = _serializationService.Deserialize<Message>(File.ReadAllText(@"Files/ILR.xml"));

            List<MessageLearner> learners = message.Learner.ToList();

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
