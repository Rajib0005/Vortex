namespace Vortex.Domain.Constants;

public static class Permissions
{
    public const string Project_Create = "Project.Create";
    public const string Project_View = "Project.View";
    public const string Project_Update = "Project.Update";
    public const string Project_Delete = "Project.Delete";

    // User management within a project
    public const string User_Invite = "User.Invite";
    public const string User_Remove = "User.Remove";
    public const string User_View = "User.View";
}

public static class RolePermissionMap
{
    public static readonly Dictionary<Guid, string[]> RolePermissions = new()
    {
        [Constants.AdminRoleId] =
        [
            Permissions.Project_Create,
            Permissions.Project_View,
            Permissions.Project_Update,
            Permissions.Project_Delete,
            Permissions.User_Invite,
            Permissions.User_Remove,
            Permissions.User_View
        ],
        [Constants.ManagerRoleId] =[
            Permissions.Project_View,
            Permissions.Project_Update,
            Permissions.User_Invite,
            Permissions.User_View,
        ],
        [Constants.MemberRoleId] = [Permissions.Project_View]
    };
} 