using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.ValidationDPActor.Interfaces.Models;
using Microsoft.ServiceFabric.Actors;

namespace ESFA.DC.ILR.ValidationService.ValidationDPActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    [ServiceContract]
    public interface IValidationDPActor : IActor
    {
        [OperationContract]
        Task<string> Validate(ValidationDPActorModel actorModel, CancellationToken cancellationToken);
    }
}
