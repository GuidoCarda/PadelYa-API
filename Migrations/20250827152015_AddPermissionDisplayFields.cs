using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionDisplayFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 5, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 5, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 19, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 19, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 20, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 21, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 22, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 26, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 34, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 35, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 37, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 38, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 39, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 40, 101 });

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PermissionComponents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "PermissionComponents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 1,
                column: "DisplayName",
                value: "Reservas");

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 2,
                column: "DisplayName",
                value: "Canchas");

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 3,
                column: "DisplayName",
                value: "Usuarios");

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 4,
                column: "DisplayName",
                value: "Roles");

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 5,
                column: "DisplayName",
                value: "Torneos");

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DisplayName", "Name" },
                values: new object[] { "Clases", "lesson" });

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 7,
                column: "DisplayName",
                value: "Rutinas");

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 8,
                column: "DisplayName",
                value: "Comentarios");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite al jugador crear su propia reserva", "Crear reserva (cliente)" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite crear reservas para cualquier usuario", "Crear reserva" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite modificar reservas existentes", "Editar reserva" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite cancelar reservas", "Cancelar reserva" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite ver todas las reservas", "Ver reservas" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite ver las reservas del usuario", "Ver reservas propias" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite asignar un usuario a una reserva", "Asignar usuario a reserva" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite marcar la reserva como pagada", "Marcar reserva como pagada" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite crear nuevos torneos", "Crear torneo" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite editar datos de torneos", "Editar torneo" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite cancelar torneos", "Cancelar torneo" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite ver torneos", "Ver torneos" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite inscribir participantes a un torneo", "Inscribir en torneo" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite asignar usuarios a un torneo", "Asignar usuario a torneo" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite cargar y gestionar resultados de partidos", "Gestionar resultados" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite generar el cuadro del torneo", "Generar cuadro", 5, "tournament:generate_bracket" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite programar los partidos del torneo", "Programar partidos", 5, "tournament:schedule_matches" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite crear nuevas canchas", "Crear cancha", 2, "court:create" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite editar información de canchas", "Editar cancha", 2, "court:edit" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite eliminar canchas", "Eliminar cancha", 2, "court:delete" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite ver todas las canchas", "Ver canchas", 2, "court:view" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Description", "DisplayName", "Name" },
                values: new object[] { "Permite crear nuevas clases", "Crear clase", "lesson:create" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite editar clases", "Editar clase", 6, "lesson:edit" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite cancelar clases", "Cancelar clase", 6, "lesson:cancel" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite ver clases", "Ver clases", 6, "lesson:view" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite inscribirse a clases", "Inscribirse en clase", 6, "lesson:join" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite asignar usuarios a una clase", "Asignar usuario a clase", 6, "lesson:assign_user" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite darse de baja de una clase", "Salir de clase", 6, "lesson:leave" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite crear nuevos usuarios", "Crear usuario", 3, "user:create" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite editar usuarios", "Editar usuario", 3, "user:edit" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite que el usuario edite su propio perfil", "Editar perfil propio", 3, "user:edit_self" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite ver todos los usuarios", "Ver usuarios", 3, "user:view" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite ver la información del propio usuario", "Ver perfil propio", 3, "user:view_own" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite asignar roles a usuarios", "Asignar roles", 3, "user:assign_roles" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite desactivar usuarios", "Desactivar usuario", 3, "user:deactivate" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite crear roles", "Crear rol", 4, "role:create" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite editar roles", "Editar rol", 4, "role:edit" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite eliminar roles", "Eliminar rol", 4, "role:delete" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite agregar o quitar permisos de un rol", "Asignar permisos a rol", 4, "role:permission:assign" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite ver todos los roles", "Ver roles", 4, "role:view" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite crear rutinas", "Crear rutina", 7, "routine:create" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "Description", "DisplayName", "ModuleId", "Name" },
                values: new object[] { "Permite editar rutinas", "Editar rutina", 7, "routine:edit" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 100,
                column: "Name",
                value: "Administrador");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 101,
                column: "Name",
                value: "Profesor");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 102,
                column: "Name",
                value: "Jugador");

            migrationBuilder.InsertData(
                table: "PermissionComponents",
                columns: new[] { "Id", "Description", "DisplayName", "ModuleId", "Name", "PermissionType" },
                values: new object[,]
                {
                    { 43, "Permite eliminar rutinas", "Eliminar rutina", 7, "routine:delete", "Simple" },
                    { 44, "Permite ver rutinas", "Ver rutinas", 7, "routine:view", "Simple" },
                    { 45, "Permite asignar usuarios a una rutina", "Asignar usuario a rutina", 7, "routine:assign_user", "Simple" },
                    { 46, "Permite crear comentarios", "Crear comentario", 8, "feedback:create", "Simple" },
                    { 47, "Permite editar comentarios", "Editar comentario", 8, "feedback:edit", "Simple" },
                    { 48, "Permite eliminar comentarios", "Eliminar comentario", 8, "feedback:delete", "Simple" },
                    { 49, "Permite ver comentarios", "Ver comentarios", 8, "feedback:view", "Simple" }
                });

            migrationBuilder.InsertData(
                table: "RolCompositePermission",
                columns: new[] { "PermissionComponentId", "RoleId" },
                values: new object[,]
                {
                    { 4, 101 },
                    { 4, 102 },
                    { 10, 101 },
                    { 11, 101 },
                    { 12, 101 },
                    { 13, 101 },
                    { 14, 101 },
                    { 15, 101 },
                    { 18, 101 },
                    { 22, 101 },
                    { 23, 101 },
                    { 24, 101 },
                    { 27, 101 },
                    { 28, 101 },
                    { 28, 102 },
                    { 31, 101 },
                    { 31, 102 },
                    { 33, 101 },
                    { 33, 102 },
                    { 41, 101 },
                    { 43, 100 },
                    { 43, 101 },
                    { 44, 100 },
                    { 44, 101 },
                    { 45, 100 },
                    { 45, 101 },
                    { 46, 100 },
                    { 46, 101 },
                    { 47, 100 },
                    { 47, 101 },
                    { 48, 100 },
                    { 48, 101 },
                    { 49, 100 },
                    { 49, 101 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 4, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 4, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 10, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 11, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 12, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 13, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 14, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 15, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 18, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 22, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 23, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 24, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 27, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 28, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 28, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 31, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 31, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 33, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 33, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 41, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 43, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 43, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 44, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 44, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 45, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 45, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 46, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 46, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 47, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 47, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 48, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 48, 101 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 49, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 49, 101 });

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PermissionComponents");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "PermissionComponents");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Modules");

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "class");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 6, "class:create" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 6, "class:edit" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 6, "class:cancel" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 6, "class:view" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 6, "class:join" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 6, "class:assign_user" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 22,
                column: "Name",
                value: "class:leave");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 3, "user:create" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 3, "user:edit" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 3, "user:edit_self" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 3, "user:view" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 3, "user:assign_roles" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 3, "user:deactivate" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 4, "role:create" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 4, "role:edit" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 4, "role:delete" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 4, "role:permission:assign" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 4, "role:view" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 7, "routine:create" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 7, "routine:edit" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 7, "routine:delete" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 7, "routine:view" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 7, "routine:assign_user" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 8, "feedback:create" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 8, "feedback:edit" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 8, "feedback:delete" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "ModuleId", "Name" },
                values: new object[] { 8, "feedback:view" });

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 100,
                column: "Name",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 101,
                column: "Name",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 102,
                column: "Name",
                value: "Player");

            migrationBuilder.InsertData(
                table: "RolCompositePermission",
                columns: new[] { "PermissionComponentId", "RoleId" },
                values: new object[,]
                {
                    { 5, 101 },
                    { 5, 102 },
                    { 19, 101 },
                    { 19, 102 },
                    { 20, 102 },
                    { 21, 101 },
                    { 22, 102 },
                    { 26, 101 },
                    { 34, 101 },
                    { 35, 101 },
                    { 37, 101 },
                    { 38, 101 },
                    { 39, 101 },
                    { 40, 101 }
                });
        }
    }
}
