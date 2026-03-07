using AutoMapper;
using Vortex.Application.Dtos;
using Vortex.Domain.Entities;

namespace Vortex.Application.Mappings;

public class AuditLogProfile : Profile
{
    public AuditLogProfile()
    {
        CreateMap<AuditLog, AuditLogDto>()
            .ForMember(dest => dest.UserName, opt => opt.Ignore()); // Will be populated manually if needed, or by join
    }
}
