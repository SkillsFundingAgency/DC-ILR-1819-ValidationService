using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ReferenceData.Employers.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class EmployersDataRetrievalService : IEmployersDataRetrievalService
    {
        private readonly IEmployersContext _employersContext;
        private readonly ICache<IMessage> _messageCache;

        public EmployersDataRetrievalService(IEmployersContext employersContext, ICache<IMessage> messageCache)
        {
            _employersContext = employersContext;
            _messageCache = messageCache;
        }

        public async Task<IEnumerable<int>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var matchingEmployerIds = new List<int>();

            var employerIdShards = GetUniqueEmployerIdsFromFile(_messageCache.Item)
                .SplitList(5000);

            foreach (var shard in employerIdShards)
            {
                matchingEmployerIds.AddRange(await GetMatchingEmployerIds(shard, cancellationToken));
            }

            return matchingEmployerIds;
        }

        public async Task<IEnumerable<int>> GetMatchingEmployerIds(IEnumerable<int> employerIds, CancellationToken cancellationToken)
        {
            return await Task.Run(
                () =>
                {
                    return _employersContext.Employers
                            .Where(p => employerIds.Contains(p.Urn))
                            .Select(p => p.Urn)
                            .ToList();
                }, cancellationToken);
        }

        public IEnumerable<int> GetUniqueEmployerIdsFromFile(IMessage message)
        {
            return GetLearnerEmpIdsFromFile(message).Union(GetWorkplaceEmpIdsFromFile(message)).Distinct();
        }

        private IEnumerable<int> GetLearnerEmpIdsFromFile(IMessage message)
        {
            return message?
                    .Learners?
                    .Where(l => l.LearnerEmploymentStatuses != null)
                    .SelectMany(l => l.LearnerEmploymentStatuses)
                    .Where(l => l.EmpIdNullable.HasValue)
                    .Select(l => l.EmpIdNullable.Value)
                ?? new List<int>();
        }

        private IEnumerable<int> GetWorkplaceEmpIdsFromFile(IMessage message)
        {
            return message?
                       .Learners?
                       .Where(l => l.LearningDeliveries != null)
                       .SelectMany(l => l.LearningDeliveries)
                       .Where(ld => ld.LearningDeliveryWorkPlacements != null)
                       .SelectMany(ld => ld.LearningDeliveryWorkPlacements)
                       .Where(wp => wp.WorkPlaceEmpIdNullable.HasValue)
                       .Select(wp => wp.WorkPlaceEmpIdNullable.Value)
                   ?? new List<int>();
        }
    }
}
