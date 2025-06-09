using System.ComponentModel.DataAnnotations.Schema;

namespace padelya_api.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Institution { get; set; }
        public string Category { get; set; }
    }
}
