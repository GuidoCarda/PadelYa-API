using padelya_api.Models;
using System.Collections.Generic;

namespace padelya_api.Models.Tournament
{
    public class Couple
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<Player> Players { get; set; } = new();
    }
}