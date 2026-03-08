using AutoMapper;
using Vortex.Application.Dtos;
using Vortex.Domain.Entities;

namespace Vortex.Application.Mappings;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<ProjectEntity, ProjectCardsDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ProjectName))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.IsAcvtive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.NumberOfCompletedTasks, opt => opt.Ignore())
            .ForMember(dest => dest.NumberOfTotalTasks, opt => opt.Ignore())
            .ForMember(dest => dest.CanDelete, opt => opt.Ignore())
            .ForMember(dest => dest.CanMark, opt => opt.Ignore());
    }
}
