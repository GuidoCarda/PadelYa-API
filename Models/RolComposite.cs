namespace padelya_api.Models
{
    public class RolComposite : PermissionComponent
    {
        public ICollection<PermissionComponent> Permissions { get; set; } = new List<PermissionComponent>();


        public override void Display(int depth)
        {

        }

        public void Add(PermissionComponent c) => Permissions.Add(c);
        public void Remove(PermissionComponent c) => Permissions.Remove(c);
        public bool HasPermission(string permissionName) => Permissions.Any(p => p.Name == permissionName);
    }
}
