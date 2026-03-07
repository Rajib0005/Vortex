using AutoMapper;
using Vortex.Domain.Dto;
using Vortex.Domain.Entities;
using Vortex.Domain.Constants;

namespace Vortex.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserDetailsDto>()
            .ConstructUsing(src => new UserDetailsDto(
                src.Id,
                src.FullName ?? string.Empty,
                src.Email ?? string.Empty,
                src.UserName ?? string.Empty,
                src.IsActive,
                src.EmailConfirmed,
                src.RoleId,
                src.Role != null ? src.Role.Name ?? string.Empty : string.Empty
            ));

        CreateMap<AuthDto, UserEntity>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(_ => Constants.AdminRoleId))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));
    }
}
