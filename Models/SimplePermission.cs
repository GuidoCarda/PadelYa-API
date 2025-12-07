using padelya_api.Constants;

namespace padelya_api.Models
{
    public class SimplePermission : PermissionComponent
    {
        public int ModuleId { get; set; }
        public Module Module { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        
        /// <summary>
        /// Specifies the required entity type for this permission.
        /// NULL = Admin/System permission (no domain profile required)
        /// Player = Requires user to have a Player profile
        /// Teacher = Requires user to have a Teacher profile
        /// </summary>
        public RequiredEntityType? RequiredEntity { get; set; }

        public override void Display(int depth)
        {
        }
    }
}
