using System.Collections.Generic;
using System.Collections.ObjectModel;
using DateTime = System.DateTime;

namespace ESFA.DC.ILR.ValidationService.InternalData.LLDDCat
{
    public class LlddCatInternalDataService : ILlddCatInternalDataService
    {
        private readonly IReadOnlyDictionary<long, DateTime> _validLlddCategories;

        public LlddCatInternalDataService()
        {
            var date2015 = new DateTime(2015, 7, 31);
            var date2099 = new DateTime(2099, 12, 31);
            var categories = new Dictionary<long, DateTime>
            {
                { 1, date2015 },
                { 2, date2015 },
                { 3, date2015 },

                { 4, date2099 },
                { 5, date2099 },
                { 6, date2099 },
                { 7, date2099 },
                { 8, date2099 },
                { 9, date2099 },
                { 10, date2099 },
                { 11, date2099 },
                { 12, date2099 },
                { 13, date2099 },
                { 14, date2099 },
                { 15, date2099 },
                { 16, date2099 },
                { 17, date2099 },

                { 93, date2099 },
                { 94, date2099 },
                { 95, date2099 },
                { 96, date2099 },
                { 97, date2099 },
                { 98, date2099 },
                { 99, date2099 }
            };

            _validLlddCategories = new ReadOnlyDictionary<long, DateTime>(categories);
        }

        public bool CategoryExists(long? category)
        {
            if (!category.HasValue)
            {
                return false;
            }
            return _validLlddCategories.ContainsKey(category.Value);
        }

        public bool CategoryExistForDate(long? category, DateTime? validTo)
        {
            if (!category.HasValue || !validTo.HasValue)
            {
                return false;
            }
            return _validLlddCategories.ContainsKey(category.Value) &&
                validTo.Value <= _validLlddCategories[category.Value];
        }
    }
}