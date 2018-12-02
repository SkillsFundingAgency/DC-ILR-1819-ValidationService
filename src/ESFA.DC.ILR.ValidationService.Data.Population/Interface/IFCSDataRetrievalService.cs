using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    /// <summary>
    /// the FCS data service definition
    /// </summary>
    /// <seealso cref="Interface.IExternalDataRetrievalService{IReadOnlyCollection{FcsContract}}" />
    public interface IFCSDataRetrievalService :
        IExternalDataRetrievalService<IReadOnlyCollection<FcsContract>>
    {
        /// <summary>
        /// Retrieves the contract allocations asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>a task running the collection builder</returns>
        Task<IReadOnlyCollection<IFcsContractAllocation>> RetrieveContractAllocationsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the eligibility rule employment statuses asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>a task running the collection builder</returns>
        Task<IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus>> RetrieveEligibilityRuleEmploymentStatusesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the eligibility rule sector subject area level.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>a task returning colletion of eligiblity rule sector subject area level.</returns>
        Task<IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel>> RetrieveEligibilityRuleSectorSubjectAreaLevelAsync(CancellationToken cancellationToken);
    }
}
