using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Tournament
{
    public class RegisterMatchResultDto
    {
        [Required(ErrorMessage = "El ID del partido es obligatorio")]
        public int MatchId { get; set; }

        [Required(ErrorMessage = "El ID de la pareja ganadora es obligatorio")]
        public int WinnerCoupleId { get; set; }

        [Required(ErrorMessage = "El resultado es obligatorio")]
        [RegularExpression(@"^\d{1,2}-\d{1,2}(,\s*\d{1,2}-\d{1,2}){1,2}$", 
            ErrorMessage = "El formato del resultado debe ser: '6-4, 6-3' o '6-4, 4-6, 7-5'")]
        public string Result { get; set; } = string.Empty;
    }

    public class MatchResultResponseDto
    {
        public int MatchId { get; set; }
        public int WinnerCoupleId { get; set; }
        public string WinnerCoupleName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool AdvancedToNextRound { get; set; }
    }
}

