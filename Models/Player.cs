using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Primitives;

namespace padelya_api.Models
{
    public class Player: Person
    {
        public string PreferredPosition { get; set; }
    }

}
