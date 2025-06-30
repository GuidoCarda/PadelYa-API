namespace padelya_api.Models
{
    public class SimplePermission : PermissionComponent
    {
        public int ModuleId { get; set; }
        public Module Module { get; set; }

        public override void Display(int depth)
        {
        }
    }
}
