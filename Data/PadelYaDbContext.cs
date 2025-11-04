using Microsoft.EntityFrameworkCore;
using padelya_api.Constants;
using padelya_api.models;
using padelya_api.Models;
using padelya_api.Models.Class;
using padelya_api.Models.Lesson;
using padelya_api.Models.Annual;
using padelya_api.Models.Challenge;
using padelya_api.Models.Notification;
using padelya_api.Models.Repair;
using padelya_api.Models.Tournament;

namespace padelya_api.Data
{
  public class PadelYaDbContext(DbContextOptions<PadelYaDbContext> options) : DbContext(options)
  {

    public DbSet<User> Users { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<UserStatus> UserStatuses { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<PermissionComponent> PermissionComponents { get; set; }
    public DbSet<SimplePermission> SimplePermissions { get; set; }
    public DbSet<RolComposite> RolComposites { get; set; }

    public DbSet<Complex> Complex { get; set; }
    public DbSet<Court> Courts { get; set; }

    // Represents all ocuppied court slots
    public DbSet<CourtSlot> CourtSlots { get; set; }

    // Bookings (antes CourtBookings)
    public DbSet<Booking> Bookings { get; set; }

    //Tournaments
    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<Couple> Couples { get; set; }
    public DbSet<TournamentEnrollment> TournamentEnrollments { get; set; }
    public DbSet<TournamentMatch> TournamentMatches { get; set; }
    public DbSet<TournamentPhase> TournamentPhases { get; set; }
    public DbSet<TournamentBracket> TournamentBrackets { get; set; }

    //Lessons
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<LessonEnrollment> LessonEnrollments { get; set; }
    public DbSet<padelya_api.Models.Lesson.ClassType> ClassTypes { get; set; }
    public DbSet<padelya_api.Models.Lesson.LessonAttendance> LessonAttendances { get; set; }
    public DbSet<Stats> Stats { get; set; }

    //Routines
    public DbSet<Routine> Routines { get; set; }
    public DbSet<Exercise> Exercises { get; set; }

    //Payments
    public DbSet<Payment> Payments { get; set; }

    // Annual Ranking
    public DbSet<AnnualTable> AnnualTables { get; set; }
    public DbSet<RankingEntry> RankingEntries { get; set; }
    public DbSet<ScoringRule> ScoringRules { get; set; }
    public DbSet<Challenge> Challenges { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<RankingTrace> RankingTraces { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        public DbSet<User> Users { get; set; }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<UserStatus> UserStatuses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<PermissionComponent> PermissionComponents { get; set; }
        public DbSet<SimplePermission> SimplePermissions { get; set; }
        public DbSet<RolComposite> RolComposites { get; set; }

        public DbSet<Complex> Complex { get; set; }
        public DbSet<Court> Courts { get; set; }

        // Represents all ocuppied court slots
        public DbSet<CourtSlot> CourtSlots { get; set; }

        // Bookings (antes CourtBookings)
        public DbSet<Booking> Bookings { get; set; }

        //Tournaments
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Couple> Couples { get; set; }
        public DbSet<TournamentEnrollment> TournamentEnrollments { get; set; }
        public DbSet<TournamentMatch> TournamentMatches { get; set; }
        public DbSet<TournamentPhase> TournamentPhases { get; set; }
        public DbSet<TournamentBracket> TournamentBrackets { get; set; }

        //Lessons
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonEnrollment> LessonEnrollments { get; set; }
        public DbSet<Stats> Stats { get; set; }

        //Routines
        public DbSet<Routine> Routines { get; set; }
        public DbSet<Exercise> Exercises { get; set; }

        //Payments
        public DbSet<Payment> Payments { get; set; }

        //Repairs
        public DbSet<Repair> Repairs { get; set; }
        public DbSet<RepairAudit> RepairAudits { get; set; }
        public DbSet<Racket> Rackets { get; set; }

        //LoginAudit
        public DbSet<LoginAudit> LoginAudits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            #region Security Module
            modelBuilder.Entity<PermissionComponent>()
                .HasKey(p => p.Id);

            // Inheritance config: TPH
            modelBuilder.Entity<PermissionComponent>()
                .HasDiscriminator<string>("PermissionType")
                .HasValue<SimplePermission>("Simple")
                .HasValue<RolComposite>("Composite");

            // User - UserStatus
            modelBuilder.Entity<User>()
                .HasOne(u => u.Status)
                .WithMany()
                .HasForeignKey("StatusId");

            modelBuilder.Entity<User>()
                .Property(u => u.StatusId)
                .HasDefaultValue(1);

            // User - RolComposite
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey("RoleId");


            // SimplePermission - Module (1:1)
            modelBuilder.Entity<SimplePermission>()
                .HasOne(sp => sp.Module)
                .WithMany(m => m.Permissions)
                .HasForeignKey("ModuleId")
                .IsRequired();

            // RolComposite - PermissionComponent (many-to-many self reference)
            modelBuilder.Entity<RolComposite>()
                .HasMany(r => r.Permissions)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "RolCompositePermission",
                    j => j
                        .HasOne<PermissionComponent>()
                        .WithMany()
                        .HasForeignKey("PermissionComponentId"),
                    j => j
                        .HasOne<RolComposite>()
                        .WithMany()
                        .HasForeignKey("RoleId")
                );

            // TPH: Table-Per-Hierarchy
            modelBuilder.Entity<Person>()
                .HasDiscriminator<string>("PersonType")
                .HasValue<Person>("Person")
                .HasValue<Player>("Player")
                .HasValue<Teacher>("Teacher");

            // User - Person (1:1 optional, unidirectional)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Person)
                .WithOne() // without inverse reference
                .HasForeignKey<User>(u => u.PersonId)
                .IsRequired(false);

            #endregion

            #region ComplexManagement

            modelBuilder.Entity<Complex>()
                .HasMany(c => c.Courts)
                .WithOne()
                .HasForeignKey(c => c.ComplexId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region CourtSlots 

            // CourtSlot 1:1 Booking
            modelBuilder.Entity<CourtSlot>()
                .HasOne(cs => cs.Booking)
                .WithOne(b => b.CourtSlot)
                .HasForeignKey<Booking>(b => b.CourtSlotId);

            // CourtSlot 1:1 Lesson
            modelBuilder.Entity<CourtSlot>()
                .HasOne(cs => cs.Lesson)
                .WithOne(l => l.CourtSlot)
                .HasForeignKey<Lesson>(l => l.CourtSlotId);

            // CourtSlot 1:1 TournamentMatch
            modelBuilder.Entity<CourtSlot>()
                .HasOne(cs => cs.TournamentMatch)
                .WithOne(tm => tm.CourtSlot)
                .HasForeignKey<TournamentMatch>(tm => tm.CourtSlotId);

            // Booking 1:N Payment
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Payments)
                .WithOne() // Eliminada navegación bilateral
                .HasForeignKey(p => p.BookingId);

            modelBuilder.Entity<Booking>()
              .Property(b => b.Status)
              .HasConversion<string>();

            modelBuilder.Entity<Payment>()
              .Property(p => p.PaymentType)
              .HasConversion<string>();

            modelBuilder.Entity<Payment>()
              .Property(p => p.PaymentStatus)
              .HasConversion<string>();

            modelBuilder.Entity<CourtSlot>()
              .Property(cs => cs.Status)
              .HasConversion<string>();

            modelBuilder.Entity<Tournament>()
              .Property(t => t.TournamentStatus)
              .HasConversion<string>();

            // Lesson 1:N LessonEnrollment
            modelBuilder.Entity<Lesson>()
                .HasMany(l => l.Enrollments)
                .WithOne(e => e.Lesson)
                .HasForeignKey(e => e.LessonId);

            // LessonEnrollment 1:1 Payment
            modelBuilder.Entity<LessonEnrollment>()
                .HasOne(e => e.Payment)
                .WithOne() // Eliminada navegación bilateral
                .HasForeignKey<Payment>(p => p.LessonEnrollmentId);

            //LessonEnrollment 1:1 Person
            modelBuilder.Entity<LessonEnrollment>()
                .HasOne(e => e.Person)
                .WithMany()
                .HasForeignKey(e => e.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            // TournamentEnrollment 1:1 Payment
            modelBuilder.Entity<TournamentEnrollment>()
                .HasOne(tr => tr.Payment)
                .WithOne() // Eliminada navegación bilateral
                .HasForeignKey<Payment>(p => p.TournamentEnrollmentId);


            #endregion


            #region Tournaments
            // Couple-Player (n:m)
            modelBuilder.Entity<Couple>()
                .HasMany(c => c.Players)
                .WithMany();

            // TournamentMatch: CoupleOne and CoupleTwo
            modelBuilder.Entity<TournamentMatch>()
                .HasOne(m => m.CoupleOne)
                .WithMany()
                .HasForeignKey(m => m.CoupleOneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TournamentMatch>()
                .HasOne(m => m.CoupleTwo)
                .WithMany()
                .HasForeignKey(m => m.CoupleTwoId)
                .OnDelete(DeleteBehavior.Restrict);

            // TournamentPhase - Tournament (n:1)
            modelBuilder.Entity<TournamentPhase>()
                .HasOne(p => p.Tournament)
                .WithMany(t => t.TournamentPhases)
                .HasForeignKey(p => p.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Bracket - TournamentPhase (n:1)
            modelBuilder.Entity<TournamentBracket>()
                .HasOne(b => b.Phase)
                .WithMany(p => p.Brackets)
                .HasForeignKey(b => b.PhaseId)
                .OnDelete(DeleteBehavior.NoAction);

            // TournamentEnrollment - Couple (n:1)
            modelBuilder.Entity<TournamentEnrollment>()
                .HasOne(e => e.Couple)
                .WithMany()
                .HasForeignKey(e => e.CoupleId)
                .OnDelete(DeleteBehavior.Restrict);

            // TournamentEnrollment - Tournament (n:1)
            modelBuilder.Entity<TournamentEnrollment>()
                .HasOne(e => e.Tournament)
                .WithMany(t => t.Enrollments)
                .HasForeignKey(e => e.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Bracket - TournamentMatch (n:1)
            modelBuilder.Entity<TournamentMatch>()
                .HasOne(m => m.Bracket)
                .WithMany(b => b.Matches)
                .HasForeignKey(m => m.BracketId)
                .OnDelete(DeleteBehavior.NoAction);

            // Tournament Configuration
            modelBuilder.Entity<Tournament>()
                .Property(t => t.TournamentStatus)
                .HasConversion<string>();

            #endregion

            #region Lessons
            // LESSON - TEACHER (Many-to-One)
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Teacher)
                .WithMany() // Eliminada navegación bilateral
                .HasForeignKey(l => l.TeacherId);

            // LESSON - STATS (One-to-Many)
            modelBuilder.Entity<Lesson>()
                .HasMany(l => l.Reports)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // STATS - PLAYER (Many-to-One)
            modelBuilder.Entity<Stats>()
                .HasOne(s => s.Player)
                .WithMany()
                .HasForeignKey(s => s.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ROUTINE - TEACHER (Many-to-One)
            modelBuilder.Entity<Routine>()
                .HasOne(r => r.Creator)
                .WithMany()
                .HasForeignKey(r => r.CreatorId);

            // ROUTINE - PLAYER (Many-to-Many)
            modelBuilder.Entity<Routine>()
                .HasMany(r => r.Players)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                "RoutinePlayer",
                j => j
                    .HasOne<Player>()
                    .WithMany()
                    .HasForeignKey("PlayerId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<Routine>()
                    .WithMany()
                    .HasForeignKey("RoutineId")
                    .OnDelete(DeleteBehavior.Cascade)
            );

            // ROUTINE - EXERCISE (Many-to-Many)
            modelBuilder.Entity<Routine>()
                .HasMany(r => r.Exercises)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "RoutineExercise",
                    j => j
                        .HasOne<Exercise>()
                        .WithMany()
                        .HasForeignKey("ExerciseId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Routine>()
                        .WithMany()
                        .HasForeignKey("RoutineId")
                        .OnDelete(DeleteBehavior.Cascade)
                );


            #endregion


            #region Repairs

            // Repair - Person (Many-to-One)
            modelBuilder.Entity<Repair>()
                .HasOne(r => r.Person)
                .WithMany()
                .HasForeignKey(r => r.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            // Repair - Racket (Many-to-One)
            modelBuilder.Entity<Repair>()
                .HasOne(r => r.Racket)
                .WithMany()
                .HasForeignKey(r => r.RacketId)
                .OnDelete(DeleteBehavior.Restrict);

            // Repair - Payment (1:1 optional)
            modelBuilder.Entity<Repair>()
                .HasOne(r => r.Payment)
                .WithOne()
                .HasForeignKey<Repair>(r => r.PaymentId)
                .IsRequired(false);

            // Store RepairStatus enum as string
            modelBuilder.Entity<Repair>()
                .Property(r => r.Status)
                .HasConversion<string>();

            // Set default for CreatedAt
            modelBuilder.Entity<Repair>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // RepairAudit - Repair (Many-to-One)
            modelBuilder.Entity<RepairAudit>()
                .HasOne(ra => ra.Repair)
                .WithMany()
                .HasForeignKey(ra => ra.RepairId)
                .OnDelete(DeleteBehavior.Restrict);

            // RepairAudit - User (Many-to-One)
            modelBuilder.Entity<RepairAudit>()
                .HasOne(ra => ra.User)
                .WithMany()
                .HasForeignKey(ra => ra.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Store RepairAuditAction enum as string
            modelBuilder.Entity<RepairAudit>()
                .Property(ra => ra.Action)
                .HasConversion<string>();

            #endregion

            #region LoginAudit

            // LoginAudit - User (Many-to-One)
            modelBuilder.Entity<LoginAudit>()
                .HasOne(la => la.User)
                .WithMany()
                .HasForeignKey(la => la.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Store LoginAuditAction enum as string
            modelBuilder.Entity<LoginAudit>()
                .Property(la => la.Action)
                .HasConversion<string>();

            // Set default for Timestamp
            modelBuilder.Entity<LoginAudit>()
                .Property(la => la.Timestamp)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region Seeding security module
            // db module/role/permissions seeding
            // 1. Modules
            modelBuilder.Entity<Module>().HasData(
                new Module { Id = 1, Name = "booking", DisplayName = "Reservas" },
                new Module { Id = 2, Name = "court", DisplayName = "Canchas" },
                new Module { Id = 3, Name = "user", DisplayName = "Usuarios" },
                new Module { Id = 4, Name = "role", DisplayName = "Roles" },
                new Module { Id = 5, Name = "tournament", DisplayName = "Torneos" },
                new Module { Id = 6, Name = "lesson", DisplayName = "Clases" },
                new Module { Id = 7, Name = "routine", DisplayName = "Rutinas" },
                new Module { Id = 8, Name = "feedback", DisplayName = "Comentarios" },
                new Module { Id = 9, Name = "repair", DisplayName = "Reparaciones" }
            );

            // 2. SimplePermissions
            modelBuilder.Entity<SimplePermission>().HasData(
                // Booking permissions
                new { Id = 1, Name = "booking:make", ModuleId = 1, PermissionType = "Simple", DisplayName = "Hacer reserva", Description = "Permite a un cliente reservar un turno en su propia cuenta" },
                new { Id = 2, Name = "booking:create", ModuleId = 1, PermissionType = "Simple", DisplayName = "Crear reserva", Description = "Permite crear reservas para cualquier usuario" },
                new { Id = 3, Name = "booking:edit", ModuleId = 1, PermissionType = "Simple", DisplayName = "Editar reserva", Description = "Permite modificar reservas existentes" },
                new { Id = 4, Name = "booking:cancel", ModuleId = 1, PermissionType = "Simple", DisplayName = "Cancelar reserva", Description = "Permite cancelar reservas" },
                new { Id = 5, Name = "booking:view", ModuleId = 1, PermissionType = "Simple", DisplayName = "Ver reservas", Description = "Permite ver todas las reservas" },
                new { Id = 6, Name = "booking:view_own", ModuleId = 1, PermissionType = "Simple", DisplayName = "Ver reservas propias", Description = "Permite ver las reservas del usuario" },
                new { Id = 7, Name = "booking:assign_user", ModuleId = 1, PermissionType = "Simple", DisplayName = "Asignar usuario a reserva", Description = "Permite asignar un usuario a una reserva" },
                new { Id = 8, Name = "booking:mark_paid", ModuleId = 1, PermissionType = "Simple", DisplayName = "Marcar reserva como pagada", Description = "Permite marcar la reserva como pagada" },

                // Tournament permissions
                new { Id = 9, Name = "tournament:create", ModuleId = 5, PermissionType = "Simple", DisplayName = "Crear torneo", Description = "Permite crear nuevos torneos" },
                new { Id = 10, Name = "tournament:edit", ModuleId = 5, PermissionType = "Simple", DisplayName = "Editar torneo", Description = "Permite editar datos de torneos" },
                new { Id = 50, Name = "tournament:delete", ModuleId = 5, PermissionType = "Simple", DisplayName = "Eliminar torneo", Description = "Permite eliminar torneos sin inscripciones" },
                new { Id = 11, Name = "tournament:cancel", ModuleId = 5, PermissionType = "Simple", DisplayName = "Cancelar torneo", Description = "Permite cancelar torneos" },
                new { Id = 12, Name = "tournament:view", ModuleId = 5, PermissionType = "Simple", DisplayName = "Ver torneos", Description = "Permite ver torneos" },
                new { Id = 13, Name = "tournament:join", ModuleId = 5, PermissionType = "Simple", DisplayName = "Inscribir en torneo", Description = "Permite inscribir participantes a un torneo" },
                new { Id = 14, Name = "tournament:assign_user", ModuleId = 5, PermissionType = "Simple", DisplayName = "Asignar usuario a torneo", Description = "Permite asignar usuarios a un torneo" },
                new { Id = 15, Name = "tournament:manage_scores", ModuleId = 5, PermissionType = "Simple", DisplayName = "Gestionar resultados", Description = "Permite cargar y gestionar resultados de partidos" },
                new { Id = 16, Name = "tournament:generate_bracket", ModuleId = 5, PermissionType = "Simple", DisplayName = "Generar cuadro", Description = "Permite generar el cuadro del torneo" },
                new { Id = 17, Name = "tournament:schedule_matches", ModuleId = 5, PermissionType = "Simple", DisplayName = "Programar partidos", Description = "Permite programar los partidos del torneo" },


                // Court permissions
                new { Id = 18, Name = "court:create", ModuleId = 2, PermissionType = "Simple", DisplayName = "Crear cancha", Description = "Permite crear nuevas canchas" },
                new { Id = 19, Name = "court:edit", ModuleId = 2, PermissionType = "Simple", DisplayName = "Editar cancha", Description = "Permite editar información de canchas" },
                new { Id = 20, Name = "court:delete", ModuleId = 2, PermissionType = "Simple", DisplayName = "Eliminar cancha", Description = "Permite eliminar canchas" },
                new { Id = 21, Name = "court:view", ModuleId = 2, PermissionType = "Simple", DisplayName = "Ver canchas", Description = "Permite ver todas las canchas" },

                // lesson permissions
                new { Id = 22, Name = "lesson:create", ModuleId = 6, PermissionType = "Simple", DisplayName = "Crear clase", Description = "Permite crear nuevas clases" },
                new { Id = 23, Name = "lesson:edit", ModuleId = 6, PermissionType = "Simple", DisplayName = "Editar clase", Description = "Permite editar clases" },
                new { Id = 24, Name = "lesson:cancel", ModuleId = 6, PermissionType = "Simple", DisplayName = "Cancelar clase", Description = "Permite cancelar clases" },
                new { Id = 25, Name = "lesson:view", ModuleId = 6, PermissionType = "Simple", DisplayName = "Ver clases", Description = "Permite ver clases" },
                new { Id = 26, Name = "lesson:join", ModuleId = 6, PermissionType = "Simple", DisplayName = "Inscribirse en clase", Description = "Permite inscribirse a clases" },
                new { Id = 27, Name = "lesson:assign_user", ModuleId = 6, PermissionType = "Simple", DisplayName = "Asignar usuario a clase", Description = "Permite asignar usuarios a una clase" },
                new { Id = 28, Name = "lesson:leave", ModuleId = 6, PermissionType = "Simple", DisplayName = "Salir de clase", Description = "Permite darse de baja de una clase" },

                // User management permissions
                new { Id = 29, Name = "user:create", ModuleId = 3, PermissionType = "Simple", DisplayName = "Crear usuario", Description = "Permite crear nuevos usuarios" },
                new { Id = 30, Name = "user:edit", ModuleId = 3, PermissionType = "Simple", DisplayName = "Editar usuario", Description = "Permite editar usuarios" },
                new { Id = 31, Name = "user:edit_self", ModuleId = 3, PermissionType = "Simple", DisplayName = "Editar perfil propio", Description = "Permite que el usuario edite su propio perfil" },
                new { Id = 32, Name = "user:view", ModuleId = 3, PermissionType = "Simple", DisplayName = "Ver usuarios", Description = "Permite ver todos los usuarios" },
                new { Id = 33, Name = "user:view_own", ModuleId = 3, PermissionType = "Simple", DisplayName = "Ver perfil propio", Description = "Permite ver la información del propio usuario" },
                new { Id = 34, Name = "user:assign_roles", ModuleId = 3, PermissionType = "Simple", DisplayName = "Asignar roles", Description = "Permite asignar roles a usuarios" },
                new { Id = 35, Name = "user:deactivate", ModuleId = 3, PermissionType = "Simple", DisplayName = "Desactivar usuario", Description = "Permite desactivar usuarios" },

                // Role management permissions
                new { Id = 36, Name = "role:create", ModuleId = 4, PermissionType = "Simple", DisplayName = "Crear rol", Description = "Permite crear roles" },
                new { Id = 37, Name = "role:edit", ModuleId = 4, PermissionType = "Simple", DisplayName = "Editar rol", Description = "Permite editar roles" },
                new { Id = 38, Name = "role:delete", ModuleId = 4, PermissionType = "Simple", DisplayName = "Eliminar rol", Description = "Permite eliminar roles" },
                new { Id = 39, Name = "role:permission:assign", ModuleId = 4, PermissionType = "Simple", DisplayName = "Asignar permisos a rol", Description = "Permite agregar o quitar permisos de un rol" },
                new { Id = 40, Name = "role:view", ModuleId = 4, PermissionType = "Simple", DisplayName = "Ver roles", Description = "Permite ver todos los roles" },

                // Routine permissions
                new { Id = 41, Name = "routine:create", ModuleId = 7, PermissionType = "Simple", DisplayName = "Crear rutina", Description = "Permite crear rutinas" },
                new { Id = 42, Name = "routine:edit", ModuleId = 7, PermissionType = "Simple", DisplayName = "Editar rutina", Description = "Permite editar rutinas" },
                new { Id = 43, Name = "routine:delete", ModuleId = 7, PermissionType = "Simple", DisplayName = "Eliminar rutina", Description = "Permite eliminar rutinas" },
                new { Id = 44, Name = "routine:view", ModuleId = 7, PermissionType = "Simple", DisplayName = "Ver rutinas", Description = "Permite ver rutinas" },
                new { Id = 45, Name = "routine:assign_user", ModuleId = 7, PermissionType = "Simple", DisplayName = "Asignar usuario a rutina", Description = "Permite asignar usuarios a una rutina" },

                // Feedback permissions
                new { Id = 46, Name = "feedback:create", ModuleId = 8, PermissionType = "Simple", DisplayName = "Crear comentario", Description = "Permite crear comentarios" },
                new { Id = 47, Name = "feedback:edit", ModuleId = 8, PermissionType = "Simple", DisplayName = "Editar comentario", Description = "Permite editar comentarios" },
                new { Id = 48, Name = "feedback:delete", ModuleId = 8, PermissionType = "Simple", DisplayName = "Eliminar comentario", Description = "Permite eliminar comentarios" },
                new { Id = 49, Name = "feedback:view", ModuleId = 8, PermissionType = "Simple", DisplayName = "Ver comentarios", Description = "Permite ver comentarios" },


              // Repair permissions
              new { Id = 55, Name = "repair:create", ModuleId = 9, PermissionType = "Simple", DisplayName = "Crear reparación", Description = "Permite crear nuevas reparaciones" },
              new { Id = 56, Name = "repair:edit", ModuleId = 9, PermissionType = "Simple", DisplayName = "Editar reparación", Description = "Permite editar reparaciones" },
              new { Id = 57, Name = "repair:cancel", ModuleId = 9, PermissionType = "Simple", DisplayName = "Cancelar reparación", Description = "Permite cancelar reparaciones" },
              new { Id = 58, Name = "repair:view", ModuleId = 9, PermissionType = "Simple", DisplayName = "Ver reparaciones", Description = "Permite ver reparaciones" },
              new { Id = 59, Name = "repair:view_own", ModuleId = 9, PermissionType = "Simple", DisplayName = "Ver reparación propia", Description = "Permite ver la reparación del usuario" }
            );

            // 3. Roles (RolComposite)
            modelBuilder.Entity<RolComposite>().HasData(
                new { Id = 100, Name = "Administrador", PermissionType = "Composite" },
                new { Id = 101, Name = "Profesor", PermissionType = "Composite" },
                new { Id = 102, Name = "Jugador", PermissionType = "Composite" }
            );

            // 4. Asignar permisos a roles (tabla intermedia)
            modelBuilder.Entity("RolCompositePermission").HasData(
                // Admin: todos los permisos
                new { RoleId = 100, PermissionComponentId = 1 },
                new { RoleId = 100, PermissionComponentId = 2 },
                new { RoleId = 100, PermissionComponentId = 3 },
                new { RoleId = 100, PermissionComponentId = 4 },
                new { RoleId = 100, PermissionComponentId = 5 },
                new { RoleId = 100, PermissionComponentId = 6 },
                new { RoleId = 100, PermissionComponentId = 7 },
                new { RoleId = 100, PermissionComponentId = 8 },
                new { RoleId = 100, PermissionComponentId = 9 },
                new { RoleId = 100, PermissionComponentId = 10 },
                new { RoleId = 100, PermissionComponentId = 11 },
                new { RoleId = 100, PermissionComponentId = 12 },
                new { RoleId = 100, PermissionComponentId = 13 },
                new { RoleId = 100, PermissionComponentId = 14 },
                new { RoleId = 100, PermissionComponentId = 15 },
                new { RoleId = 100, PermissionComponentId = 16 },
                new { RoleId = 100, PermissionComponentId = 17 },
                new { RoleId = 100, PermissionComponentId = 18 },
                new { RoleId = 100, PermissionComponentId = 19 },
                new { RoleId = 100, PermissionComponentId = 20 },
                new { RoleId = 100, PermissionComponentId = 21 },
                new { RoleId = 100, PermissionComponentId = 22 },
                new { RoleId = 100, PermissionComponentId = 23 },
                new { RoleId = 100, PermissionComponentId = 24 },
                new { RoleId = 100, PermissionComponentId = 25 },
                new { RoleId = 100, PermissionComponentId = 26 },
                new { RoleId = 100, PermissionComponentId = 27 },
                new { RoleId = 100, PermissionComponentId = 28 },
                new { RoleId = 100, PermissionComponentId = 29 },
                new { RoleId = 100, PermissionComponentId = 30 },
                new { RoleId = 100, PermissionComponentId = 31 },
                new { RoleId = 100, PermissionComponentId = 32 },
                new { RoleId = 100, PermissionComponentId = 33 },
                new { RoleId = 100, PermissionComponentId = 34 },
                new { RoleId = 100, PermissionComponentId = 35 },
                new { RoleId = 100, PermissionComponentId = 36 },
                new { RoleId = 100, PermissionComponentId = 37 },
                new { RoleId = 100, PermissionComponentId = 38 },
                new { RoleId = 100, PermissionComponentId = 39 },
                new { RoleId = 100, PermissionComponentId = 40 },
                new { RoleId = 100, PermissionComponentId = 41 },
                new { RoleId = 100, PermissionComponentId = 42 },
                new { RoleId = 100, PermissionComponentId = 43 },
                new { RoleId = 100, PermissionComponentId = 44 },
                new { RoleId = 100, PermissionComponentId = 45 },
                new { RoleId = 100, PermissionComponentId = 46 },
                new { RoleId = 100, PermissionComponentId = 47 },
                new { RoleId = 100, PermissionComponentId = 48 },
                new { RoleId = 100, PermissionComponentId = 49 },
                new { RoleId = 100, PermissionComponentId = 50 },
                new { RoleId = 100, PermissionComponentId = 55 },
                new { RoleId = 100, PermissionComponentId = 56 },
                new { RoleId = 100, PermissionComponentId = 57 },
                new { RoleId = 100, PermissionComponentId = 58 },
                new { RoleId = 100, PermissionComponentId = 59 },

                // Teacher: permisos específicos
                new { RoleId = 101, PermissionComponentId = 1 }, // booking:make
                new { RoleId = 101, PermissionComponentId = 4 }, // booking:cancel
                new { RoleId = 101, PermissionComponentId = 6 }, // booking:view_own

                new { RoleId = 101, PermissionComponentId = 22 }, // lesson:create
                new { RoleId = 101, PermissionComponentId = 23 }, // lesson:edit
                new { RoleId = 101, PermissionComponentId = 24 }, // lesson:cancel
                new { RoleId = 101, PermissionComponentId = 25 }, // lesson:view
                new { RoleId = 101, PermissionComponentId = 27 }, // lesson:assign_user
                new { RoleId = 101, PermissionComponentId = 28 }, // lesson:leave

                new { RoleId = 101, PermissionComponentId = 41 }, // routine:create
                new { RoleId = 101, PermissionComponentId = 42 }, // routine:edit
                new { RoleId = 101, PermissionComponentId = 43 }, // routine:delete
                new { RoleId = 101, PermissionComponentId = 44 }, // routine:view
                new { RoleId = 101, PermissionComponentId = 45 }, // routine:assign_user

                new { RoleId = 101, PermissionComponentId = 46 }, // feedback:create
                new { RoleId = 101, PermissionComponentId = 47 }, // feedback:edit
                new { RoleId = 101, PermissionComponentId = 48 }, // feedback:delete
                new { RoleId = 101, PermissionComponentId = 49 }, // feedback:view

                new { RoleId = 101, PermissionComponentId = 31 }, // user:edit_self
                new { RoleId = 101, PermissionComponentId = 33 }, // user:view_own

                //tournament
                new { RoleId = 101, PermissionComponentId = 10 }, // tournament:create
                new { RoleId = 101, PermissionComponentId = 11 }, // tournament:edit
                new { RoleId = 101, PermissionComponentId = 12 }, // tournament:cancel
                new { RoleId = 101, PermissionComponentId = 13 }, // tournament:view
                new { RoleId = 101, PermissionComponentId = 14 }, // tournament:join
                new { RoleId = 101, PermissionComponentId = 15 }, // tournament:assign_user
                new { RoleId = 101, PermissionComponentId = 16 }, // tournament:manage_scores
                new { RoleId = 101, PermissionComponentId = 17 }, // tournament:generate_bracket
                new { RoleId = 101, PermissionComponentId = 18 }, // tournament:schedule_matches


                // Player: permisos básicos
                new { RoleId = 102, PermissionComponentId = 1 }, // booking:make
                new { RoleId = 102, PermissionComponentId = 4 }, // booking:cancel
                new { RoleId = 102, PermissionComponentId = 6 }, // booking:view_own
                new { RoleId = 102, PermissionComponentId = 25 }, // lesson:view
                new { RoleId = 102, PermissionComponentId = 26 }, // lesson:join  
                new { RoleId = 102, PermissionComponentId = 28 }, // lesson:leave
                new { RoleId = 102, PermissionComponentId = 31 }, // user:edit_self
                new { RoleId = 102, PermissionComponentId = 33 }, // user:view_own
                new { RoleId = 102, PermissionComponentId = 37 }, // routine:view
                new { RoleId = 102, PermissionComponentId = 42 }  // feedback:view
            );

            modelBuilder.Entity<UserStatus>().HasData(
                new UserStatus { Id = 1, Name = "Active" },
                new UserStatus { Id = 2, Name = "Inactive" }
            );

            #endregion

            #region Complex Seeding
            // Complex seeding
            modelBuilder.Entity<Complex>().HasData(
                new Complex
                {
                    Id = 1,
                    Name = "PadelYa Sports Complex",
                    Address = "123 Sports Avenue, Downtown District, City Center",
                    OpeningTime = new DateTime(2024, 1, 1, 7, 30, 0), // 7:30 AM
                    ClosingTime = new DateTime(2024, 1, 2, 0, 0, 0)  // 12:00 AM
                }
            );

            // Courts seeding
            modelBuilder.Entity<Court>().HasData(
                new Court
                {
                    Id = 1,
                    Name = "Cancha 1 - Premium",
                    CourtStatus = CourtStatus.Available,
                    BookingPrice = 20,
                    OpeningTime = new TimeOnly(7, 30), // 7:30 AM
                    ClosingTime = new TimeOnly(0, 0, 0), // 12:00 AM
                    ComplexId = 1,
                    Type = "Cristal"
                },
                new Court
                {
                    Id = 2,
                    Name = "Cancha 2 - Standard",
                    CourtStatus = CourtStatus.Available,
                    BookingPrice = 10,
                    OpeningTime = new TimeOnly(7, 30), // 7:30 AM
                    ClosingTime = new TimeOnly(0, 0, 0), // 12:00 AM
                    ComplexId = 1,
                    Type = "Césped"
                }
            );

            #endregion

            // Seeding de Player y User para pruebas de Bookings
            modelBuilder.Entity<Player>().HasData(
            new Player
            {
                Id = 1,
                Name = "Juan",
                Surname = "Pérez",
                Email = "player1@padelya.com",
                PhoneNumber = "+598 91 234 567",
                Birthdate = new DateTime(1990, 1, 1),
                Category = "Primera",
                PreferredPosition = "Derecha"
            },
            new Player
            {
                Id = 2,
                Name = "Ana",
                Surname = "García",
                Email = "player2@padelya.com",
                PhoneNumber = "+598 92 345 678",
                Birthdate = new DateTime(1992, 2, 2),
                Category = "Segunda",
                PreferredPosition = "Revés"
            },
            new Player
            {
                Id = 3,
                Name = "Luis",
                Surname = "Martínez",
                Email = "player3@padelya.com",
                PhoneNumber = "+598 93 456 789",
                Birthdate = new DateTime(1994, 3, 3),
                Category = "Tercera",
                PreferredPosition = "Derecha"
            }
          );

            modelBuilder.Entity<Teacher>().HasData(
                new Teacher
                {
                    Id = 4,
                    Name = "María",
                    Surname = "González",
                    Email = "teacher@padelya.com",
                    PhoneNumber = "+598 94 567 890",
                    Birthdate = new DateTime(1985, 5, 15),
                    Category = "Profesional",
                    Institution = "PadelYa Academy",
                    Title = "Profesor Certificado"
                }
            );

            modelBuilder.Entity<User>().HasData(
                // Admin user without person
                new User
                {
                    Id = 1,
                    PersonId = null,
                    Name = "Admin",
                    Surname = "System",
                    Email = "admin@padelya.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEG5lfoUTu6r2+ZvrS33ePXDMdIIbp6s1lxbAH8I4hJv9JKy4nB7LEP9X8/e9ypyvsQ==", // test1234
                    StatusId = 1,
                    RoleId = 100
                },
                // Teacher user
                new User
                {
                    Id = 2,
                    PersonId = 4,
                    Name = "María",
                    Surname = "González",
                    Email = "teacher@padelya.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEG5lfoUTu6r2+ZvrS33ePXDMdIIbp6s1lxbAH8I4hJv9JKy4nB7LEP9X8/e9ypyvsQ==", // test1234
                    StatusId = 1,
                    RoleId = 101
                },
                // Player users
                new User
                {
                    Id = 3,
                    PersonId = 1,
                    Name = "Juan",
                    Surname = "Pérez",
                    Email = "player1@padelya.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEG5lfoUTu6r2+ZvrS33ePXDMdIIbp6s1lxbAH8I4hJv9JKy4nB7LEP9X8/e9ypyvsQ==", // test1234
                    StatusId = 1,
                    RoleId = 102
                },
                new User
                {
                    Id = 4,
                    PersonId = 2,
                    Name = "Ana",
                    Surname = "García",
                    Email = "player2@padelya.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEG5lfoUTu6r2+ZvrS33ePXDMdIIbp6s1lxbAH8I4hJv9JKy4nB7LEP9X8/e9ypyvsQ==", // test1234
                    StatusId = 1,
                    RoleId = 102
                },
                new User
                {
                    Id = 5,
                    PersonId = 3,
                    Name = "Luis",
                    Surname = "Martínez",
                    Email = "player3@padelya.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEG5lfoUTu6r2+ZvrS33ePXDMdIIbp6s1lxbAH8I4hJv9JKy4nB7LEP9X8/e9ypyvsQ==", // test1234
                    StatusId = 1,
                    RoleId = 102
                }
            );

            // Seeding de CourtSlots y Bookings para pruebas de disponibilidad
            modelBuilder.Entity<CourtSlot>().HasData(
                new CourtSlot { Id = 1, CourtId = 1, Date = new DateTime(2025, 7, 6), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 30) },
                new CourtSlot { Id = 2, CourtId = 1, Date = new DateTime(2025, 7, 6), StartTime = new TimeOnly(13, 30), EndTime = new TimeOnly(15, 0) },
                new CourtSlot { Id = 3, CourtId = 2, Date = new DateTime(2025, 7, 6), StartTime = new TimeOnly(10, 30), EndTime = new TimeOnly(12, 0) },
                new CourtSlot { Id = 4, CourtId = 2, Date = new DateTime(2025, 7, 6), StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(19, 30) }
            );
            modelBuilder.Entity<Booking>().HasData(
                new Booking { Id = 1, CourtSlotId = 1, PersonId = 1, Status = BookingStatus.ReservedPaid },
                new Booking { Id = 2, CourtSlotId = 2, PersonId = 2, Status = BookingStatus.ReservedDeposit },
                new Booking { Id = 3, CourtSlotId = 3, PersonId = 1, Status = BookingStatus.ReservedPaid },
                new Booking { Id = 4, CourtSlotId = 4, PersonId = 3, Status = BookingStatus.ReservedPaid }
            );
        }
    }
}
