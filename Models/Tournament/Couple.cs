using padelya_api.Models;

public class Couple
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

   
    public List<Player> Players { get; set; } 
}