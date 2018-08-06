namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface IPostcodeQueryService
    {
        bool RegexValid(string postcode);
    }
}
