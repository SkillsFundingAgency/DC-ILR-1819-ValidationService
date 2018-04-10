using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler
{
    public class ValidationErrorHandlerOutputService : IValidationOutputService<IValidationError>
    {
        private readonly IValidationErrorHandler _validationErrorHandler;
        private readonly IValidationContext _validationContext;

        public ValidationErrorHandlerOutputService(IValidationErrorHandler validationErrorHandler, IValidationContext validationContext)
        {
            _validationErrorHandler = validationErrorHandler;
            _validationContext = validationContext;
        }

        public IEnumerable<IValidationError> Process()
        {
            IEnumerable<IValidationError> errors = ((ValidationErrorHandler)_validationErrorHandler).ErrorBag;
            StringBuilder contents = new StringBuilder();
            contents.AppendLine(@"Error\Warning,Learner Ref,Rule Name,Field Values,Error Message,Aim Sequence Number,Aim Reference Number,Software Supplier Aim ID,Funding Model,Subcontracted UKPRN,Provider Specified Learner Monitoring A,Provider Specified Learner Monitoring B,Provider Specified Learning Delivery Monitoring A,Provider Specified Learning Delivery Monitoring B,Provider Specified Learning Delivery Monitoring C,Provider Specified Learning Delivery Monitoring D,OFFICIAL-SENSITIVE");
            foreach (var error in errors)
            {
                var errorFirst = string.Empty;
                foreach (var s in error.ErrorMessageParameters)
                {
                    errorFirst += s;
                }

                contents.AppendLine($"E,{error.LearnerReferenceNumber},{error.RuleName},{errorFirst},,{error?.AimSequenceNumber},,,,");
            }

            System.IO.File.WriteAllText(_validationContext.Output, contents.ToString());

            return errors;
        }
    }
}
