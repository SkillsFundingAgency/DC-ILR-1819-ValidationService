using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;
using ESFA.DC.ILR.ValidationService.Stubs;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler
{
    public class AcceptanceTestsValidationErrorHandlerOutputService : IValidationOutputService<IValidationError>
    {
        private readonly IValidationErrorHandler _validationErrorHandler;
        private readonly IValidationContext _validationContext;
        private readonly ISerializationService _serializationService;

        public AcceptanceTestsValidationErrorHandlerOutputService(IValidationErrorHandler validationErrorHandler, IValidationContext validationContext, ISerializationService serializationService)
        {
            _validationErrorHandler = validationErrorHandler;
            _validationContext = validationContext;
            _serializationService = serializationService;
        }

        public IEnumerable<IValidationError> Process()
        {
            need to trasfrom from one type to another.
            IEnumerable<IValidationError> errors = ((ValidationErrorHandler)_validationErrorHandler).ErrorBag;
            List<ValidationError> errorList = new List<ValidationError>(errors);
            string errorsAsString = _serializationService.Serialize<List<ValidationError>>(errorList);

            //StringBuilder contents = new StringBuilder();
            //contents.AppendLine(@"Error\Warning,Learner Ref,Rule Name,Field Values,Error Message,Aim Sequence Number,Aim Reference Number,Software Supplier Aim ID,Funding Model,Subcontracted UKPRN,Provider Specified Learner Monitoring A,Provider Specified Learner Monitoring B,Provider Specified Learning Delivery Monitoring A,Provider Specified Learning Delivery Monitoring B,Provider Specified Learning Delivery Monitoring C,Provider Specified Learning Delivery Monitoring D,OFFICIAL-SENSITIVE");
            //foreach (var error in errors)
            //{
            //    var errorFirst = string.Empty;
            //    foreach (var s in error.ErrorMessageParameters)
            //    {
            //        errorFirst += $"{s.PropertyName}='{s.Value}' ";
            //    }

            //    contents.AppendLine($"E,{error.LearnerReferenceNumber},{error.RuleName},{errorFirst},,{error?.AimSequenceNumber},,,,");
            //}

            ((ValidationContextStub)_validationContext).Output = errorsAsString;

            return errors;
        }
    }
}
