using padelya_api.Constants;

namespace padelya_api.DTOs.Tournament
{
    // DTO para enviar al backend al inscribirse
    public class TournamentEnrollmentDto
    {
        // El ID del jugador que el usuario logueado elige como su pareja.
        public int PartnerId { get; set; }
    }

    // DTO para la respuesta de un jugador con sus datos completos
    public class PlayerResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    // DTO para la respuesta de una pareja
    public class CoupleResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PlayerResponseDto> Players { get; set; } = new();
    }

    // DTO para la respuesta de un enrollment
    public class TournamentEnrollmentResponseDto
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public CoupleResponseDto Couple { get; set; } = new();
    }

    // DTO para la respuesta de un torneo completo con enrollments
    public class TournamentResponseDto
    {
        public int Id { get; set; }
        public string CurrentPhase { get; set; } = string.Empty;
        public TournamentStatus TournamentStatus { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quota { get; set; }
        public decimal EnrollmentPrice { get; set; }
        public DateTime EnrollmentStartDate { get; set; }
        public DateTime EnrollmentEndDate { get; set; }
        public DateTime TournamentStartDate { get; set; }
        public DateTime TournamentEndDate { get; set; }
        public List<TournamentEnrollmentResponseDto> Enrollments { get; set; } = new();
        public List<TournamentPhaseDto> TournamentPhases { get; set; } = new();
    }

    // DTO para las fases del torneo
    public class TournamentPhaseDto
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
