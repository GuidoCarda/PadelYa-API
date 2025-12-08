namespace padelya_api.DTOs.Lesson
{
    public class LessonReportDto
    {
        public LessonStatisticsDto Statistics { get; set; }
        public List<DailyLessonDto> DailyLessons { get; set; }
        public List<TeacherPerformanceDto> TeacherPerformance { get; set; }
        public List<ClassTypeDistributionDto> ClassTypeDistribution { get; set; }
        public List<AttendanceDistributionDto> AttendanceDistribution { get; set; }
        public List<TopRoutineDto> TopRoutines { get; set; }
        public List<TopExerciseDto> TopExercises { get; set; }
        public List<StudentAttendanceDto> TopStudents { get; set; }
    }

    public class LessonStatisticsDto
    {
        public int TotalLessons { get; set; }
        public int ProgrammedLessons { get; set; }
        public int CompletedLessons { get; set; }
        public int CancelledLessons { get; set; }
        public int TotalEnrollments { get; set; }
        public decimal AverageEnrollmentsPerLesson { get; set; }
        public decimal AttendanceRate { get; set; } // Porcentaje de asistencia
        public int TotalRoutinesAssigned { get; set; }
        public int TotalExercisesUsed { get; set; }
        public int ActiveTeachers { get; set; }
        public int ActiveStudents { get; set; }
        public decimal TotalRevenue { get; set; } // Si las clases tienen precio
    }

    public class DailyLessonDto
    {
        public string Date { get; set; }
        public int LessonCount { get; set; }
        public int EnrollmentCount { get; set; }
        public int AttendedCount { get; set; }
    }

    public class TeacherPerformanceDto
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int LessonCount { get; set; }
        public int TotalStudents { get; set; }
        public decimal AverageAttendance { get; set; }
        public int RoutinesCreated { get; set; }
    }

    public class ClassTypeDistributionDto
    {
        public int ClassTypeId { get; set; }
        public string ClassTypeName { get; set; }
        public int LessonCount { get; set; }
        public int EnrollmentCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class AttendanceDistributionDto
    {
        public string Status { get; set; } // Present, Absent, Justified
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class TopRoutineDto
    {
        public int RoutineId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int AssignmentCount { get; set; }
        public string CreatorName { get; set; }
    }

    public class TopExerciseDto
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int UsageCount { get; set; } // Cuántas rutinas lo incluyen
    }

    public class StudentAttendanceDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int TotalClasses { get; set; }
        public int AttendedClasses { get; set; }
        public decimal AttendanceRate { get; set; }
    }
}

