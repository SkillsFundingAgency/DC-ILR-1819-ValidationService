using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    /// <summary>
    /// the postcode data service definition
    /// </summary>
    /// <seealso cref="Interface.IExternalDataRetrievalService{IEnumerable{string}}" />
    public interface IPostcodesDataRetrievalService :
        IExternalDataRetrievalService<IEnumerable<string>>
    {
        /// <summary>
        /// Retrieves the ons postcodes asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>a set of ons postcode records</returns>
        Task<IReadOnlyCollection<IONSPostcode>> RetrieveONSPostcodesAsync(CancellationToken cancellationToken);
    }
}
