using Vortex.Domain.Dto;
using Vortex.Domain.Entities;

namespace Vortex.Infrastructure.Interfaces;

public interface ITokenService
{
    string GenerateTokenAsync(AuthDto userModel);
}