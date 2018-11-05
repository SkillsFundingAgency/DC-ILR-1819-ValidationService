using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R85Rule : AbstractRule, IRule<IMessage>
    {
        public R85Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R85)
        {
        }

        public void Validate(IMessage message)
        {
            if (message.Learners != null && message.LearnerDestinationAndProgressions != null)
            {
                foreach (ILearner learner in message.Learners)
                {
                    bool ok = true;
                    var matches = message.LearnerDestinationAndProgressions.
                        Where(s => s.LearnRefNumber == learner.LearnRefNumber);
                    if (matches.Count() > 0)
                    {
                        ok = matches.All(s => s.ULN == learner.ULN);
                    }

                    if (!ok)
                    {
                        HandleValidationError(learner.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(learner.ULN, matches.First().ULN, matches.First().LearnRefNumber));
                    }
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long uln, long dpUln, string dpLearnRefNumber)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, uln),
                BuildErrorMessageParameter(PropertyNameConstants.LearningDestinationAndProgressionULN, dpUln),
                BuildErrorMessageParameter(PropertyNameConstants.LearningDestinationAndProgressionLearnRefNumber, dpLearnRefNumber)
            };
        }
    }
}
