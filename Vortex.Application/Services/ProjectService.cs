using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain.Constants;
using Vortex.Domain.Entities;
using Vortex.Domain.Repositories;
using Vortex.Infrastructure.CustomException;

namespace Vortex.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IGenericRepository<ProjectEntity> _projectRepository;
    private readonly IGenericRepository<UserProjectRole> _userProjectRoleRepository;
    private readonly IUserService _userService;

    public ProjectService(
        IGenericRepository<ProjectEntity> projectRepository,
        IGenericRepository<UserProjectRole> userProjectRoleRepository,
        IUserService userService)
    {
        _projectRepository = projectRepository;
        _userProjectRoleRepository = userProjectRoleRepository;
        _userService = userService;
    }
    public async Task UpsertProjectAsync(UpsertProjectDto projectModel, CancellationToken cancellation)
    {
        await (projectModel.ProjectId is null
            ? CreateProjectAsync(projectModel, cancellation)
            : UpdateProjectAsync(projectModel, cancellation));
    }

    public async Task DeleteProject(Guid projectId, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProjectCardsDto>> GetProjectsOfUser(Guid userId, CancellationToken cancellation)
    {
        var currentUser = await _userService.GetUserDetailsByIdAsync(cancellation);
        var isAdmin = currentUser.RoleId == Constants.AdminRoleId;
        var isManager = currentUser.RoleId == Constants.ManagerRoleId;
        var projects = await _userProjectRoleRepository.GetByCondition(x => x.UserId == userId && x.Project.IsActive && !x.Project.IsDeleted)
            .Select(upr => new ProjectCardsDto
            {
                ProjectTitle = upr.Project.ProjectName,
                Description = upr.Project.Description,
                IsAcvtive = upr.Project.IsActive,
                NumberOfCompletedTasks = 0,
                NumberOfTotalTasks = 0,
                StartDate = upr.Project.CreatedAt,
                CanDelete = isAdmin,
                CanMark = isAdmin || isManager
            }).ToListAsync(cancellation);
        return projects;
    }

    #region private methods

    private async Task CreateProjectAsync(UpsertProjectDto projectModel, CancellationToken cancellation)
    {
        var existingProject = await _projectRepository.GetByCondition(project => project.ProjectName == projectModel.ProjectName
                && project.ProjectKey == projectModel.ProjectKey)
            .FirstOrDefaultAsync(p => p.IsActive && !p.IsDeleted, cancellation);

        if (existingProject is not null) throw new ConflictException("Project is already existed");
        var currentUserId = _userService.GetCurrentUserId();
        var currentUserDetails = await _userService.GetUserDetailsByIdAsync(cancellation);
        var projectEntity = new ProjectEntity
        {
            Id = Guid.NewGuid(),
            ProjectName = projectModel.ProjectName,
            ProjectKey = projectModel.ProjectKey,
            Description = projectModel.ProjectDescription,
            IsActive = projectModel.IsActive ?? true,
            IsDeleted = false,
            CreatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = currentUserId,
            UpdatedAt = DateTime.UtcNow,
        };
        var projectUserRole = new UserProjectRole
        {
            Id = Guid.NewGuid(),
            UserId = currentUserId,
            RoleId = currentUserDetails.RoleId,
            ProjectId = projectEntity.Id,
        };
        await _projectRepository.AddAsync(projectEntity);
        await _userProjectRoleRepository.AddAsync(projectUserRole);
        await _projectRepository.SaveChangesAsync();
    }

    private async Task UpdateProjectAsync(UpsertProjectDto projectModel, CancellationToken cancellation)
    {
        var existingProject = await _projectRepository.GetByIdAsync(projectModel.ProjectId ?? Guid.Empty);
        if (existingProject is null) throw new BadRequestException("No project found");

        existingProject.ProjectName = projectModel.ProjectName ?? existingProject.ProjectName;
        existingProject.ProjectKey = projectModel.ProjectKey ?? existingProject.ProjectKey;
        existingProject.Description = projectModel.ProjectDescription ?? existingProject.Description;
        existingProject.IsActive = projectModel.IsActive ?? existingProject.IsActive;
        existingProject.IsDeleted = false;
        existingProject.UpdatedAt = DateTime.UtcNow;
        existingProject.UpdatedBy = _userService.GetCurrentUserId();

        await _projectRepository.SaveChangesAsync();
    }

    #endregion
}