using padelya_api.Models;
using System.Collections.Generic;

namespace padelya_api.Models.Tournament
{
    public class Couple
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        public List<Player> Players { get; set; } = new(); // Lista de jugadores de la pareja (siempre debe tener 2 jugadores)
    }
}