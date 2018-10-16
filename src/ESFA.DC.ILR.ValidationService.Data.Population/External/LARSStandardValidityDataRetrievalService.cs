using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    /// <summary>
    /// the LARS standard validity data retrieval service
    /// </summary>
    /// <seealso cref="ILARSStandardValidityDataRetrievalService" />
    public class LARSStandardValidityDataRetrievalService :
        ILARSStandardValidityDataRetrievalService
    {
        /// <summary>
        /// The lars (EF) object
        /// </summary>
        private readonly ILARS _lars;

        /// <summary>
        /// Initializes a new instance of the <see cref="LARSStandardValidityDataRetrievalService"/> class.
        /// </summary>
        /// <param name="lars">The lars (EF) object.</param>
        public LARSStandardValidityDataRetrievalService(ILARS lars)
        {
            _lars = lars;
        }

        /// <summary>
        /// Retrieves the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>a list of LARS Standard Validity</returns>
        public async Task<List<LARSStandardValidity>> RetrieveAsync(CancellationToken cancellationToken)
        {
            return await _lars.LARS_StandardValidity
                .Select(sv => new LARSStandardValidity
                {
                    StandardCode = sv.StandardCode,
                    ValidityCategory = sv.ValidityCategory,
                    StartDate = sv.StartDate,
                    EndDate = sv.EndDate,
                    LastNewStartDate = sv.LastNewStartDate
                }).ToListAsync(cancellationToken);
        }
    }
}
