using Vortex.Application.Interfaces;

namespace Vortex.Infrastructure.Services;

public class CorrelationIdService : ICorrelationIdService
{
    public Guid CorrelationId { get; } = Guid.NewGuid();
}
