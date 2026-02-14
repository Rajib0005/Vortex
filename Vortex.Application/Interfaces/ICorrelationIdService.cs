namespace Vortex.Application.Interfaces;

public interface ICorrelationIdService
{
    Guid CorrelationId { get; }
}
