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
        public DbSet<Module> Modules { get; set; }
        public DbSet<PermissionComponent> PermissionComponents { get; set; }
        public DbSet<SimplePermission> SimplePermissions { get; set; }
        public DbSet<RolComposite> RolComposites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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



            #region ComplexManagement

            modelBuilder.Entity<Complex>()
                .HasMany(c => c.Courts)
                .WithOne(c => c.Complex)
                .HasForeignKey(c => c.ComplexId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Court>()
                .HasMany(c => c.Availability)
                .WithOne(a => a.Court)
                .HasForeignKey(a => a.CourtId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            // db module/role/permissions seeding
            // 1. Modules
            modelBuilder.Entity<Module>().HasData(
                new Module { Id = 1, Name = "booking" },
                new Module { Id = 2, Name = "court" },
                new Module { Id = 3, Name = "user" },
                new Module { Id = 4, Name = "role" },
                new Module { Id = 5, Name = "tournament" },
                new Module { Id = 6, Name = "class" },
                new Module { Id = 7, Name = "routine" },
                new Module { Id = 8, Name = "feedback" }
            );

            // 2. SimplePermissions
            modelBuilder.Entity<SimplePermission>().HasData(
                // Booking permissions
                new { Id = 1, Name = "booking:make", ModuleId = 1, PermissionType = "Simple" },
                new { Id = 2, Name = "booking:create", ModuleId = 1, PermissionType = "Simple" },
                new { Id = 3, Name = "booking:edit", ModuleId = 1, PermissionType = "Simple" },
                new { Id = 4, Name = "booking:cancel", ModuleId = 1, PermissionType = "Simple" },
                new { Id = 5, Name = "booking:view", ModuleId = 1, PermissionType = "Simple" },
                new { Id = 6, Name = "booking:view_own", ModuleId = 1, PermissionType = "Simple" },
                new { Id = 7, Name = "booking:assign_user", ModuleId = 1, PermissionType = "Simple" },
                new { Id = 8, Name = "booking:mark_paid", ModuleId = 1, PermissionType = "Simple" },

                // Tournament permissions
                new { Id = 9, Name = "tournament:create", ModuleId = 5, PermissionType = "Simple" },
                new { Id = 10, Name = "tournament:edit", ModuleId = 5, PermissionType = "Simple" },
                new { Id = 11, Name = "tournament:cancel", ModuleId = 5, PermissionType = "Simple" },
                new { Id = 12, Name = "tournament:view", ModuleId = 5, PermissionType = "Simple" },
                new { Id = 13, Name = "tournament:join", ModuleId = 5, PermissionType = "Simple" },
                new { Id = 14, Name = "tournament:assign_user", ModuleId = 5, PermissionType = "Simple" },
                new { Id = 15, Name = "tournament:manage_scores", ModuleId = 5, PermissionType = "Simple" },

                // Class permissions
                new { Id = 16, Name = "class:create", ModuleId = 6, PermissionType = "Simple" },
                new { Id = 17, Name = "class:edit", ModuleId = 6, PermissionType = "Simple" },
                new { Id = 18, Name = "class:cancel", ModuleId = 6, PermissionType = "Simple" },
                new { Id = 19, Name = "class:view", ModuleId = 6, PermissionType = "Simple" },
                new { Id = 20, Name = "class:join", ModuleId = 6, PermissionType = "Simple" },
                new { Id = 21, Name = "class:assign_user", ModuleId = 6, PermissionType = "Simple" },
                new { Id = 22, Name = "class:leave", ModuleId = 6, PermissionType = "Simple" },

                // User management permissions
                new { Id = 23, Name = "user:create", ModuleId = 3, PermissionType = "Simple" },
                new { Id = 24, Name = "user:edit", ModuleId = 3, PermissionType = "Simple" },
                new { Id = 25, Name = "user:edit_self", ModuleId = 3, PermissionType = "Simple" },
                new { Id = 26, Name = "user:view", ModuleId = 3, PermissionType = "Simple" },
                new { Id = 27, Name = "user:assign_roles", ModuleId = 3, PermissionType = "Simple" },
                new { Id = 28, Name = "user:deactivate", ModuleId = 3, PermissionType = "Simple" },

                // Role management permissions
                new { Id = 29, Name = "role:create", ModuleId = 4, PermissionType = "Simple" },
                new { Id = 30, Name = "role:edit", ModuleId = 4, PermissionType = "Simple" },
                new { Id = 31, Name = "role:delete", ModuleId = 4, PermissionType = "Simple" },
                new { Id = 32, Name = "role:permission:assign", ModuleId = 4, PermissionType = "Simple" },
                new { Id = 33, Name = "role:view", ModuleId = 4, PermissionType = "Simple" },

                // Routine permissions
                new { Id = 34, Name = "routine:create", ModuleId = 7, PermissionType = "Simple" },
                new { Id = 35, Name = "routine:edit", ModuleId = 7, PermissionType = "Simple" },
                new { Id = 36, Name = "routine:delete", ModuleId = 7, PermissionType = "Simple" },
                new { Id = 37, Name = "routine:view", ModuleId = 7, PermissionType = "Simple" },
                new { Id = 38, Name = "routine:assign_user", ModuleId = 7, PermissionType = "Simple" },

                // Feedback permissions
                new { Id = 39, Name = "feedback:create", ModuleId = 8, PermissionType = "Simple" },
                new { Id = 40, Name = "feedback:edit", ModuleId = 8, PermissionType = "Simple" },
                new { Id = 41, Name = "feedback:delete", ModuleId = 8, PermissionType = "Simple" },
                new { Id = 42, Name = "feedback:view", ModuleId = 8, PermissionType = "Simple" }
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

                // Teacher: permisos específicos
                new { RoleId = 101, PermissionComponentId = 1 }, // booking:make
                new { RoleId = 101, PermissionComponentId = 5 }, // booking:view
                new { RoleId = 101, PermissionComponentId = 6 }, // booking:view_own
                new { RoleId = 101, PermissionComponentId = 16 }, // class:create
                new { RoleId = 101, PermissionComponentId = 17 }, // class:edit
                new { RoleId = 101, PermissionComponentId = 19 }, // class:view
                new { RoleId = 101, PermissionComponentId = 21 }, // class:assign_user
                new { RoleId = 101, PermissionComponentId = 34 }, // routine:create
                new { RoleId = 101, PermissionComponentId = 35 }, // routine:edit
                new { RoleId = 101, PermissionComponentId = 37 }, // routine:view
                new { RoleId = 101, PermissionComponentId = 38 }, // routine:assign_user
                new { RoleId = 101, PermissionComponentId = 39 }, // feedback:create
                new { RoleId = 101, PermissionComponentId = 40 }, // feedback:edit
                new { RoleId = 101, PermissionComponentId = 42 }, // feedback:view
                new { RoleId = 101, PermissionComponentId = 25 }, // user:edit_self
                new { RoleId = 101, PermissionComponentId = 26 }, // user:view

                // Player: permisos básicos
                new { RoleId = 102, PermissionComponentId = 1 }, // booking:make
                new { RoleId = 102, PermissionComponentId = 5 }, // booking:view
                new { RoleId = 102, PermissionComponentId = 6 }, // booking:view_own
                new { RoleId = 102, PermissionComponentId = 20 }, // class:join
                new { RoleId = 102, PermissionComponentId = 22 }, // class:leave
                new { RoleId = 102, PermissionComponentId = 19 }, // class:view
                new { RoleId = 102, PermissionComponentId = 37 }, // routine:view
                new { RoleId = 102, PermissionComponentId = 42 }, // feedback:view
                new { RoleId = 102, PermissionComponentId = 25 }, // user:edit_self
                new { RoleId = 102, PermissionComponentId = 26 }  // user:view
            );

            modelBuilder.Entity<UserStatus>().HasData(
                new UserStatus { Id = 1, Name = "Active" },
                new UserStatus { Id = 2, Name = "Inactive" }
            );
        }
    }
}
