namespace padelya_api.Services
{
    public interface IPermissionService
    {
        // User permission checking (for authorization)
        Task<bool> HasPermissionAsync(int userId, string permissionName);
        Task<bool> HasModulePermissionAsync(int userId, string module, string action);
        Task<bool> HasModuleAccessAsync(int userId, string module);
        Task<List<string>> GetUserPermissionsAsync(int userId);
        Task<List<string>> GetUserModulePermissionsAsync(int userId, string module);

        // Utility methods
        Task<bool> HasAnyPermissionAsync(int userId, params string[] permissionNames);
        Task<bool> HasAllPermissionsAsync(int userId, params string[] permissionNames);
    }
}