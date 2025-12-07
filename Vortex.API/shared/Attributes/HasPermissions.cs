using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vortex.Domain.Dto;

namespace Vortex.API.shared.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class HasPermissions : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _requiredRole;

    public HasPermissions(string requiredRole)
    {
        _requiredRole = requiredRole;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var projectIdFromRoute = context.RouteData.Values["projectId"]?.ToString();
        
        if (string.IsNullOrEmpty(projectIdFromRoute))
        {
            context.Result = new BadRequestObjectResult("Missing projectId in route");
            return;
        }

        var projectAccessClaim = user.Claims.FirstOrDefault(c => c.Type == "projectAccess")?.Value;
        if (string.IsNullOrEmpty(projectAccessClaim))
        {
            context.Result = new ForbidResult();
            return;
        }

        List<ProjectRoleDto>? accessEntries;
        try
        {
            accessEntries = JsonSerializer.Deserialize<List<ProjectRoleDto>>(projectAccessClaim);
        }
        catch
        {
            context.Result = new ForbidResult();
            return;
        }

        var projectAccess = accessEntries?
            .FirstOrDefault(p => p.ProjectId.ToString().Equals(projectIdFromRoute, StringComparison.OrdinalIgnoreCase));

        if (projectAccess == null || !projectAccess.Roles.Contains(_requiredRole, StringComparer.OrdinalIgnoreCase))
        {
            context.Result = new ForbidResult();
            return;
        }

        await Task.CompletedTask;
    }
}
 