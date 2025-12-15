using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class PendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 68,
                column: "Description",
                value: "Permite ver el catálogo de productos");

            // Insert PermissionComponents only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [PermissionComponents] WHERE [Id] = 73)
                BEGIN
                    SET IDENTITY_INSERT [PermissionComponents] ON;
                    INSERT INTO [PermissionComponents] ([Id], [Description], [DisplayName], [ModuleId], [Name], [PermissionType], [RequiredEntity])
                    VALUES (73, N'Permite ver todos los pedidos de clientes', N'Ver todos los pedidos', 12, N'order:view_all', N'Simple', NULL);
                    SET IDENTITY_INSERT [PermissionComponents] OFF;
                END

                IF NOT EXISTS (SELECT 1 FROM [PermissionComponents] WHERE [Id] = 74)
                BEGIN
                    SET IDENTITY_INSERT [PermissionComponents] ON;
                    INSERT INTO [PermissionComponents] ([Id], [Description], [DisplayName], [ModuleId], [Name], [PermissionType], [RequiredEntity])
                    VALUES (74, N'Permite cambiar el estado de un pedido', N'Actualizar estado de pedido', 12, N'order:update_status', N'Simple', NULL);
                    SET IDENTITY_INSERT [PermissionComponents] OFF;
                END

                IF NOT EXISTS (SELECT 1 FROM [PermissionComponents] WHERE [Id] = 75)
                BEGIN
                    SET IDENTITY_INSERT [PermissionComponents] ON;
                    INSERT INTO [PermissionComponents] ([Id], [Description], [DisplayName], [ModuleId], [Name], [PermissionType], [RequiredEntity])
                    VALUES (75, N'Permite ver el historial de pedidos propios', N'Ver pedidos propios', 12, N'order:view_own', N'Simple', N'Player');
                    SET IDENTITY_INSERT [PermissionComponents] OFF;
                END

                IF NOT EXISTS (SELECT 1 FROM [PermissionComponents] WHERE [Id] = 76)
                BEGIN
                    SET IDENTITY_INSERT [PermissionComponents] ON;
                    INSERT INTO [PermissionComponents] ([Id], [Description], [DisplayName], [ModuleId], [Name], [PermissionType], [RequiredEntity])
                    VALUES (76, N'Permite realizar una compra (checkout)', N'Realizar compra', 12, N'order:create', N'Simple', N'Player');
                    SET IDENTITY_INSERT [PermissionComponents] OFF;
                END

                IF NOT EXISTS (SELECT 1 FROM [PermissionComponents] WHERE [Id] = 77)
                BEGIN
                    SET IDENTITY_INSERT [PermissionComponents] ON;
                    INSERT INTO [PermissionComponents] ([Id], [Description], [DisplayName], [ModuleId], [Name], [PermissionType], [RequiredEntity])
                    VALUES (77, N'Permite agregar productos al carrito', N'Agregar al carrito', 12, N'cart:add', N'Simple', N'Player');
                    SET IDENTITY_INSERT [PermissionComponents] OFF;
                END

                IF NOT EXISTS (SELECT 1 FROM [PermissionComponents] WHERE [Id] = 78)
                BEGIN
                    SET IDENTITY_INSERT [PermissionComponents] ON;
                    INSERT INTO [PermissionComponents] ([Id], [Description], [DisplayName], [ModuleId], [Name], [PermissionType], [RequiredEntity])
                    VALUES (78, N'Permite quitar productos del carrito', N'Eliminar del carrito', 12, N'cart:remove', N'Simple', N'Player');
                    SET IDENTITY_INSERT [PermissionComponents] OFF;
                END

                IF NOT EXISTS (SELECT 1 FROM [PermissionComponents] WHERE [Id] = 79)
                BEGIN
                    SET IDENTITY_INSERT [PermissionComponents] ON;
                    INSERT INTO [PermissionComponents] ([Id], [Description], [DisplayName], [ModuleId], [Name], [PermissionType], [RequiredEntity])
                    VALUES (79, N'Permite cambiar cantidades en el carrito', N'Modificar carrito', 12, N'cart:update', N'Simple', N'Player');
                    SET IDENTITY_INSERT [PermissionComponents] OFF;
                END

                IF NOT EXISTS (SELECT 1 FROM [PermissionComponents] WHERE [Id] = 80)
                BEGIN
                    SET IDENTITY_INSERT [PermissionComponents] ON;
                    INSERT INTO [PermissionComponents] ([Id], [Description], [DisplayName], [ModuleId], [Name], [PermissionType], [RequiredEntity])
                    VALUES (80, N'Permite ver el carrito de compras', N'Ver carrito', 12, N'cart:view', N'Simple', N'Player');
                    SET IDENTITY_INSERT [PermissionComponents] OFF;
                END
            ");

            // Insert RolCompositePermission only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 68 AND [RoleId] = 102)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (68, 102);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 72 AND [RoleId] = 102)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (72, 102);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 73 AND [RoleId] = 100)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (73, 100);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 74 AND [RoleId] = 100)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (74, 100);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 75 AND [RoleId] = 100)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (75, 100);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 75 AND [RoleId] = 102)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (75, 102);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 76 AND [RoleId] = 100)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (76, 100);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 76 AND [RoleId] = 102)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (76, 102);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 77 AND [RoleId] = 100)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (77, 100);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 77 AND [RoleId] = 102)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (77, 102);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 78 AND [RoleId] = 100)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (78, 100);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 78 AND [RoleId] = 102)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (78, 102);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 79 AND [RoleId] = 100)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (79, 100);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 79 AND [RoleId] = 102)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (79, 102);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 80 AND [RoleId] = 100)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (80, 100);

                IF NOT EXISTS (SELECT 1 FROM [RolCompositePermission] WHERE [PermissionComponentId] = 80 AND [RoleId] = 102)
                    INSERT INTO [RolCompositePermission] ([PermissionComponentId], [RoleId]) VALUES (80, 102);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 68, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 72, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 73, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 74, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 75, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 75, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 76, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 76, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 77, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 77, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 78, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 78, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 79, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 79, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 80, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 80, 102 });

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 68,
                column: "Description",
                value: "Permite ver todos los productos");
        }
    }
}
