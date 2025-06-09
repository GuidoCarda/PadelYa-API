namespace padelya_api.Models
{
    public class SimplePermission : PermissionComponent
    {
        public int FormId { get; set; }
        public Form Form { get; set; }

        public override void Display(int depth)
        {
        }
    }
}
