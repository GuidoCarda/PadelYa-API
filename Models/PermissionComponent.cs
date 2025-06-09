namespace padelya_api.Models
{
    public abstract class PermissionComponent
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public abstract void Display(int depth);
    }
}
