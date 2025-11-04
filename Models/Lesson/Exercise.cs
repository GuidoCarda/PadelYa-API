using System.Collections.Generic;
using padelya_api.Models;

namespace padelya_api.Models.Class
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}