using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    /// <summary>
    /// the LARS standard data retrieval service
    /// </summary>
    /// <seealso cref="ILARSStandardDataRetrievalService" />
    public class LARSStandardDataRetrievalService : AbstractDataRetrievalService, ILARSStandardDataRetrievalService
    {
        private readonly ILARS _lars;

        public LARSStandardDataRetrievalService(ILARS lars, ICache<IMessage> messageCache)
            : base(messageCache)
        {
            _lars = lars;
        }

        public async Task<List<LARSStandard>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var uniqueSTDCodes = UniqueSTDCodesFromMessage(_messageCache.Item);

            return await _lars.LARS_Standard
                .Select(s => new LARSStandard
                {
                    StandardCode = s.StandardCode,
                    StandardSectorCode = s.StandardSectorCode,
                    NotionalEndLevel = s.NotionalEndLevel,
                    EffectiveFrom = s.EffectiveFrom,
                    EffectiveTo = s.EffectiveTo,
                    StandardsFunding = s.LARS_StandardFunding.Select(x => new LARSStandardFunding()
                    {
                        CoreGovContributionCap = x.CoreGovContributionCap,
                        EffectiveFrom = x.EffectiveFrom,
                        EffectiveTo = x.EffectiveTo
                    })
                })
                .Where(s => uniqueSTDCodes.Contains(s.StandardCode))
                .ToListAsync(cancellationToken);
        }

        public IEnumerable<int> UniqueSTDCodesFromMessage(IMessage message)
        {
            return message?
                    .Learners?
                    .Where(l => l.LearningDeliveries != null)
                    .SelectMany(l => l.LearningDeliveries)
                    .Where(ld => ld.StdCodeNullable != null)
                    .Select(ld => ld.StdCodeNullable.Value)
                    .Distinct()
                   ?? new List<int>();
        }
    }
}
