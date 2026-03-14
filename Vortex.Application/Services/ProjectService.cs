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
                CanDelete = upr.RoleId == Constants.AdminRoleId,
                CanMark = upr.RoleId == Constants.AdminRoleId || upr.RoleId == Constants.ManagerRoleId
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

    public async Task<UpsertProjectDto> GetProjectDetailsForUpdateAsync(Guid projectId,
        CancellationToken cancellation)
    {
        var existingProject = await _projectRepository.GetByIdAsync(projectId);
        if (existingProject is null) throw new BadRequestException("No project found");

        var currentUserId = _userService.GetCurrentUserId();
        var currentUserRoleId = _userService.GetCurrentUserRoleId();
        var userProjectRole = await _userProjectRoleRepository.GetByCondition((x)=> x.ProjectId == projectId && x.UserId == currentUserId)
            .FirstOrDefaultAsync(cancellation);
        if(userProjectRole is null) throw new BadRequestException("No project found");
        
        if(userProjectRole.RoleId != Constants.AdminRoleId && userProjectRole.RoleId != Constants.ManagerRoleId) throw new BadRequestException("You are not authorized to update this project");

        var invitedUsers = await _userProjectRoleRepository
            .GetByCondition(x => x.ProjectId == projectId)
            .Include(x => x.User)
            .Select(x => new UserToInviteInProject
            {
                UserId = x.UserId,
                UserEmail = x.User.Email ?? string.Empty
            }).ToListAsync(cancellation);

        var projectModel = new UpsertProjectDto
        {
            ProjectId = existingProject.Id,
            ProjectName = existingProject.ProjectName,
            ProjectKey = existingProject.ProjectKey,
            ProjectDescription = existingProject.Description,
            IsActive = existingProject.IsActive,
            Priority = existingProject.Priority,
            EstimatedDeadline = existingProject.EstimatedDeadline,
            Domain = existingProject.Domain,
            InviteUsers = invitedUsers
        };
        
        return projectModel;
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
        await _userProjectRoleRepository.AddAsync(projectUserRole);
        await using var transaction = await _projectRepository.BeginTransactionAsync();
        try
        {
            await _projectRepository.AddAsync(projectEntity);
            var notificationsToPublish = await InviteUsersToProjectAsync(projectEntity.Id, projectModel.InviteUsers, cancellation);
            await _projectRepository.SaveChangesAsync();
            await transaction.CommitAsync(cancellation);
            
            if (notificationsToPublish.Count != 0)
                await _bus.PublishBatch(notificationsToPublish, cancellation);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellation);
            throw;
        }
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

        await using var transaction = await _projectRepository.BeginTransactionAsync();
        try
        {
            var notificationsToPublish = await InviteUsersToProjectAsync(existingProject.Id, projectModel.InviteUsers, cancellation);
            await _projectRepository.SaveChangesAsync();
            await transaction.CommitAsync(cancellation);

            if (notificationsToPublish.Count != 0)
                await _bus.PublishBatch(notificationsToPublish, cancellation);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellation);
            throw;
        }
    }

    private async Task<List<NotificationContract>> InviteUsersToProjectAsync(Guid projectId, List<UserToInviteInProject> inviteUsers, CancellationToken cancellation)
    {
        var notificationsToPublish = new List<NotificationContract>();
        var currentProjectRoles = await _userProjectRoleRepository
            .GetByCondition(upr => upr.ProjectId == projectId)
            .ToListAsync(cancellation);
        var usersToKeepIds = inviteUsers?.Select(u => u.UserId).ToList() ?? [];
        var rolesToRemove = currentProjectRoles.Where(upr => !usersToKeepIds.Contains(upr.UserId)).ToList();
        
        if (rolesToRemove.Count != 0)
        {
            _userProjectRoleRepository.DeleteRangeAsync(rolesToRemove);
        }

        if (inviteUsers == null || inviteUsers.Count == 0) return notificationsToPublish;
        
        foreach (var user in inviteUsers)
        {
            var existingRole = currentProjectRoles.FirstOrDefault(upr => upr.UserId == user.UserId);

            if (existingRole == null)
            {
                var userProjectRole = new UserProjectRole
                {
                    Id = Guid.NewGuid(),
                    UserId = user.UserId,
                    RoleId = Constants.MemberRoleId,
                    ProjectId = projectId,
                };
                
                try 
                {
                    await _userProjectRoleRepository.AddAsync(userProjectRole);
                    
                    var userDetails = await _userService.GetUserDetailsByIdAsync(user.UserId, cancellation);
                    var resolvedEmail = userDetails.Email ?? string.Empty;
                    var invitationLink = $"{UrlConstants.BaseUrl}/projects";
                    
                    notificationsToPublish.Add(new NotificationContract(
                        NotificationId: Guid.NewGuid(),
                        Destination: resolvedEmail,
                        TemplateId: "InvitationEmail",
                        TemplateData: new Dictionary<string, string>
                        {
                            { "Subject", "You have been invited to Vortex" },
                            { "InvitationLink", invitationLink }
                        },
                        Timestamp: DateTime.UtcNow
                    ));
                }
                catch (DbUpdateException){
                    // Ignore duplicate key or constraint violation for concurrent inserts
                    continue;
                }
            }
        }
        
        return notificationsToPublish;
    }
    #endregion
}