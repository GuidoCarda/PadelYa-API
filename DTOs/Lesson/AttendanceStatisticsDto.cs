namespace padelya_api.DTOs.Lesson
{
    public class AttendanceStatisticsDto
    {
        public int TotalClasses { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int JustifiedCount { get; set; }
        public double AttendancePercentage { get; set; }
        public int TotalRegistered { get; set; } // Total de clases donde se registró asistencia
    }
}

