using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Primitives;

namespace padelya_api.Models
{
    public class Player
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime BirthDate { get; set; }
        public string PreferredPosition { get; set; }
    }

}
