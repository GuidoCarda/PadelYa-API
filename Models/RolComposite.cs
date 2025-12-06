using padelya_api.Constants;

namespace padelya_api.Models
{
    public class RolComposite : PermissionComponent
    {
        public ICollection<PermissionComponent> Permissions { get; set; } = new List<PermissionComponent>();

        /// <summary>
        /// Specifies the target entity type for this role.
        /// Used as a safety filter when assigning permissions to roles.
        /// None = Admin role (can only have permissions with RequiredEntity = NULL)
        /// Player = Player role (can have permissions with RequiredEntity = NULL or Player)
        /// Teacher = Teacher role (can have permissions with RequiredEntity = NULL or Teacher)
        /// </summary>
        public RequiredEntityType? TargetType { get; set; }

        public override void Display(int depth)
        {

        }

        public void Add(PermissionComponent c) => Permissions.Add(c);
        public void Remove(PermissionComponent c) => Permissions.Remove(c);
        public bool HasPermission(string permissionName) => Permissions.Any(p => p.Name == permissionName);
    }
}
