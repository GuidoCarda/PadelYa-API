using Microsoft.EntityFrameworkCore;
using padelya_api.Models;

namespace padelya_api.Data
{
    public class PadelYaDbContext(DbContextOptions<PadelYaDbContext> options) : DbContext(options)
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<UserStatus> UserStatuses { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<PermissionComponent> PermissionComponents { get; set; }
        public DbSet<SimplePermission> SimplePermissions { get; set; }
        public DbSet<RolComposite> RolComposites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
            

            // SimplePermission - Form (1:1)
            modelBuilder.Entity<SimplePermission>()
                .HasOne(sp => sp.Form)
                .WithMany(f => f.Permissions)
                .HasForeignKey("FormId")
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

            // db form/role/permissions seeding
            // 1. Forms
            modelBuilder.Entity<Form>().HasData(
                new Form { Id = 1, Name = "Booking" },
                new Form { Id = 2, Name = "CourtManagement" },
                new Form { Id = 3, Name = "UserManagement" },
                new Form { Id = 4, Name = "RoleManagement" },
                new Form { Id = 5, Name = "Tournaments" },
                new Form { Id = 6, Name = "Classes" }
            );

            // 2. SimplePermissions
            modelBuilder.Entity<SimplePermission>().HasData(
                new { Id = 1, Name = "Booking_View", FormId = 1, PermissionType = "Simple" },
                new { Id = 2, Name = "Booking_Create", FormId = 1, PermissionType = "Simple" },
                new { Id = 3, Name = "CourtManagement_View", FormId = 2, PermissionType = "Simple" },
                new { Id = 4, Name = "UserManagement_View", FormId = 3, PermissionType = "Simple" },
                new { Id = 5, Name = "RoleManagement_View", FormId = 4, PermissionType = "Simple" },
                new { Id = 6, Name = "Tournaments_View", FormId = 5, PermissionType = "Simple" },
                new { Id = 7, Name = "Classes_View", FormId = 6, PermissionType = "Simple" }
            // Agrega más permisos según lo necesites
            );

            // 3. Roles (RolComposite)
            modelBuilder.Entity<RolComposite>().HasData(
                new { Id = 100, Name = "Admin", PermissionType = "Composite" },
                new { Id = 101, Name = "Teacher", PermissionType = "Composite" },
                new { Id = 102, Name = "Player", PermissionType = "Composite" }
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

                // Teacher: solo algunos permisos
                new { RoleId = 101, PermissionComponentId = 1 },
                new { RoleId = 101, PermissionComponentId = 6 },
                new { RoleId = 101, PermissionComponentId = 7 },

                // Player: solo booking y clases
                new { RoleId = 102, PermissionComponentId = 1 },
                new { RoleId = 102, PermissionComponentId = 2 },
                new { RoleId = 102, PermissionComponentId = 7 }
            );

            modelBuilder.Entity<UserStatus>().HasData(
                new UserStatus { Id = 1, Name = "Active" },
                new UserStatus { Id = 2, Name = "Inactive" }
            );
        }
    }
}
