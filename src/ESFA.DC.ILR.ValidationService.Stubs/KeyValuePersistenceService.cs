using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.IO.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class KeyValuePersistenceService : IKeyValuePersistenceService
    {
        private readonly ConcurrentDictionary<string, string> _dictionary = new ConcurrentDictionary<string, string>();

        public Task<string> GetAsync(string key)
        {
            return Task.FromResult(_dictionary[key]);
        }

        public Task RemoveAsync(string key)
        {
            return Task.Run(() => _dictionary.TryRemove(key, out var value));
        }

        public Task SaveAsync(string key, string value)
        {
            return Task.Run(() => _dictionary.TryAdd(key, value));
        }
    }
}
