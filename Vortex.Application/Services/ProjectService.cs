using Microsoft.EntityFrameworkCore;
using Vortex.Application.Dtos;
using Vortex.Application.Interfaces;
using Vortex.Domain;
using Vortex.Domain.Constants;
using Vortex.Domain.Entities;
using Vortex.Domain.Repositories;
using Vortex.Domain.Exceptions;
using MassTransit;
using MassTransit.Initializers;
using Vortex.Contracts.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Vortex.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IGenericRepository<ProjectEntity> _projectRepository;
    private readonly IGenericRepository<UserProjectRole> _userProjectRoleRepository;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    private readonly IBus _bus;

    public ProjectService(
        IGenericRepository<ProjectEntity> projectRepository,
        IGenericRepository<UserProjectRole> userProjectRoleRepository,
        IBus bus,
        IUserService userService,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _userProjectRoleRepository = userProjectRoleRepository;
        _userService = userService;
        _bus = bus;
        _mapper = mapper;
    }
    public async Task UpsertProjectAsync(UpsertProjectDto projectModel, CancellationToken cancellation, Guid? createdBy = null)
    {
        await (projectModel.ProjectId is null
            ? CreateProjectAsync(projectModel, cancellation, createdBy)
            : UpdateProjectAsync(projectModel, cancellation));
    }

    public async Task DeleteProject(Guid projectId, CancellationToken cancellation)
    {
        var existingProject = await _projectRepository.GetByIdAsync(projectId);
        var projectUserRoleMapper = await _userProjectRoleRepository.GetByCondition((x)=> x.ProjectId == projectId)
            .FirstOrDefaultAsync(cancellation);
        if(existingProject is null || projectUserRoleMapper is null) throw new BadRequestException("No project found");
        
        existingProject.IsDeleted = true;
        _userProjectRoleRepository.UpdateAsync(projectUserRoleMapper);
        await _projectRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProjectCardsDto>> GetProjectsOfUser(Guid userId, CancellationToken cancellation)
    {
        var currentUser = await _userService.GetUserDetailsByIdAsync(cancellation);
        var isAdmin = currentUser.RoleId == Constants.AdminRoleId;
        var isManager = currentUser.RoleId == Constants.ManagerRoleId;
        var projects = await _userProjectRoleRepository
            .GetByCondition(x => x.UserId == userId && x.Project.IsActive && !x.Project.IsDeleted)
            .OrderByDescending((x)=> x.Project.CreatedAt)
            .Select(upr => new ProjectCardsDto
            {
                ProjectId = upr.Project.Id,
                Title = upr.Project.ProjectName,
                Description = upr.Project.Description,
                ProjectKey =  upr.Project.ProjectKey,
                IsAcvtive = upr.Project.IsActive,
                NumberOfCompletedTasks = 0,
                NumberOfTotalTasks = 0,
                StartDate = upr.Project.CreatedAt,
                Priority = upr.Project.Priority,
                EstimatedDeadline = upr.Project.EstimatedDeadline,
                Domain = upr.Project.Domain,
                CanDelete = isAdmin,
                CanMark = isAdmin || isManager
            }).ToListAsync(cancellation);
        return projects;
    }

    public async Task<ProjectCardsDto> GetProjectDetailsById(Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if(project is null) throw new BadRequestException("Project not found");
        
        var currentUser = await _userService.GetUserDetailsByIdAsync();
        var isAdmin = currentUser.RoleId == Constants.AdminRoleId;
        var isManager = currentUser.RoleId == Constants.ManagerRoleId;

        var projectDetails = _mapper.Map<ProjectCardsDto>(project);
        projectDetails.NumberOfCompletedTasks = 0;
        projectDetails.NumberOfTotalTasks = 0;
        projectDetails.CanDelete = isAdmin;
        projectDetails.CanMark = isAdmin || isManager;

        return projectDetails;
    }

    #region private methods

    private async Task CreateProjectAsync(UpsertProjectDto projectModel, CancellationToken cancellation, Guid? createdBy = null)
    {
        var existingProject = await _projectRepository.GetByCondition(project => project.ProjectName == projectModel.ProjectName
                && project.ProjectKey == projectModel.ProjectKey)
            .FirstOrDefaultAsync(p => p.IsActive && !p.IsDeleted, cancellation);

        if (existingProject is not null) throw new ConflictException("Project is already existed");
        var currentUserId = createdBy ?? _userService.GetCurrentUserId();
        var currentUserDetails = await _userService.GetUserDetailsByIdAsync(currentUserId, cancellation);
        var projectEntity = new ProjectEntity
        {
            Id = projectModel.ProjectId ?? Guid.NewGuid(),
            ProjectName = projectModel.ProjectName ?? string.Empty,
            ProjectKey = projectModel.ProjectKey ?? string.Empty,
            Description = projectModel.ProjectDescription,
            IsActive = projectModel.IsActive ?? true,
            Priority = projectModel.Priority ?? ProjectPriority.Medium,
            EstimatedDeadline = projectModel.EstimatedDeadline,
            Domain = projectModel.Domain,
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
        await InviteUsersToProjectAsync(projectEntity.Id, projectModel.InviteUsers, cancellation);
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
        existingProject.Priority = projectModel.Priority ?? existingProject.Priority;
        existingProject.EstimatedDeadline = projectModel.EstimatedDeadline ?? existingProject.EstimatedDeadline;
        existingProject.Domain = projectModel.Domain ?? existingProject.Domain;
        existingProject.IsDeleted = false;
        existingProject.UpdatedAt = DateTime.UtcNow;
        existingProject.UpdatedBy = _userService.GetCurrentUserId();

        await InviteUsersToProjectAsync(existingProject.Id, projectModel.InviteUsers, cancellation);
        await _projectRepository.SaveChangesAsync();
    }

    private async Task InviteUsersToProjectAsync(Guid projectId, List<UserToInviteInProject> inviteUsers, CancellationToken cancellation)
    {
        if (inviteUsers == null || inviteUsers.Count == 0) return;
        
        foreach (var user in inviteUsers)
        {
            var isAlreadyInvited = await _userProjectRoleRepository
                .GetByCondition(upr => upr.ProjectId == projectId && upr.UserId == user.UserId)
                .AnyAsync(cancellation);

            if (!isAlreadyInvited)
            {
                var userProjectRole = new UserProjectRole
                {
                    Id = Guid.NewGuid(),
                    UserId = user.UserId,
                    RoleId = Constants.MemberRoleId,
                    ProjectId = projectId,
                };
                await _userProjectRoleRepository.AddAsync(userProjectRole);
                var invitationLink = $"{UrlConstants.BaseUrl}/projects";
                var notificationsToSend = new List<NotificationContract>
                {
                    new NotificationContract(
                        NotificationId: Guid.NewGuid(),
                        Destination: user.UserEmail ?? string.Empty,
                        TemplateId: "InvitationEmail", // Matches the HTML file name without extension
                        TemplateData: new Dictionary<string, string>
                        {
                            { "Subject", "You have been invited to Vortex" },
                            { "InvitationLink", invitationLink }
                        },
                        Timestamp: DateTime.UtcNow
                    )
                };
                
                if (notificationsToSend.Count != 0)
                    await _bus.PublishBatch(notificationsToSend, cancellation);
            }
        }
    }
    #endregion
}