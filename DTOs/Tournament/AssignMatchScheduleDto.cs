using System;
using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Tournament
{
    public class AssignMatchScheduleDto
    {
        [Required(ErrorMessage = "El ID del partido es obligatorio")]
        public int MatchId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime ScheduledDate { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "El ID de la cancha es obligatorio")]
        public int CourtId { get; set; }

        /// <summary>
        /// Duración estimada del partido en minutos (por defecto 90 minutos)
        /// </summary>
        public int DurationMinutes { get; set; } = 90;
    }

    public class MatchScheduleResponseDto
    {
        public int MatchId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int CourtId { get; set; }
        public string CourtName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}

