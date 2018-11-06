using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
                    var misMatched = message.LearnerDestinationAndProgressions.Where(ldap => ldap.LearnRefNumber == learner.LearnRefNumber && ldap.ULN != learner.ULN);
                    if (misMatched.Count() == 0)
                    {
                        misMatched = message.LearnerDestinationAndProgressions
                            .Where(ldap => ldap.ULN == learner.ULN && ldap.LearnRefNumber != learner.LearnRefNumber);
                    }

                    if (misMatched.Count() > 0)
                    {
                        HandleValidationError(learner.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(learner.ULN, misMatched.First().ULN, misMatched.First().LearnRefNumber));
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
