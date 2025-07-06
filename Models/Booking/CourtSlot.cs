using padelya_api.models;
using padelya_api.Models;
using padelya_api.Models.Tournament;

public enum BookingType
{
    Casual,
    Class,
    Tournament
}

public class CourtSlot
{
    public int Id { get; set; }
    public int CourtId { get; set; }
    public Court Court { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    // Relaciones 1:1 (solo una será no nula)
    public Booking Booking { get; set; }
    public Lesson Lesson { get; set; }
    public TournamentMatch TournamentMatch { get; set; }
}