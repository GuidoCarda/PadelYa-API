namespace padelya_api.DTOs.Role
{
    public class CreateRoleDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateRoleDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class AddPermissionDto
    {
        public List<int> permissionsIds { get; set; }
    }

    public class AssignRoleDto
    {
        public int RoleId { get; set; }
    }
}