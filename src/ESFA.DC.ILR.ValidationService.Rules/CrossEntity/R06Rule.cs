using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R06Rule : AbstractRule,
        IRule<IMessage>
    {
        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="R06Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public R06Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R06)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(IMessage objectToValidate)
        {
            if (objectToValidate?.Learners == null)
            {
                return;
            }

            HashSet<string> allLearnRefs = new HashSet<string>();
            foreach (ILearner learner in objectToValidate.Learners)
            {
                if (string.IsNullOrEmpty(learner.LearnRefNumber))
                {
                    continue;
                }
                else if (allLearnRefs.Contains(learner.LearnRefNumber))
                {
                    RaiseValidationMessage(learner.LearnRefNumber);
                }
                else
                {
                    allLearnRefs.Add(learner.LearnRefNumber);
                }
            }
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        public void RaiseValidationMessage(string learnRefNumber)
        {
            _messageHandler.Handle(RuleName, learnRefNumber, null, null);
        }
    }
}