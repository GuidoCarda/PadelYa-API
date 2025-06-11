namespace padelya_api.DTOs
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
    public int PermissionId { get; set; }
  }
}