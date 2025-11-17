using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
  /// <inheritdoc />
  public partial class AddRankingModuleAndPermissions : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<string>(
          name: "Milestones",
          table: "Stats",
          type: "nvarchar(max)",
          nullable: true);

      migrationBuilder.AddColumn<string>(
          name: "Observations",
          table: "Stats",
          type: "nvarchar(max)",
          nullable: true);

      migrationBuilder.AddColumn<DateTime>(
          name: "RecordedAt",
          table: "Stats",
          type: "datetime2",
          nullable: false,
          defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

      migrationBuilder.AddColumn<int>(
          name: "RecordedByTeacherId",
          table: "Stats",
          type: "int",
          nullable: true);

      migrationBuilder.AddColumn<string>(
          name: "Category",
          table: "Exercises",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "");

      migrationBuilder.AddColumn<string>(
          name: "Description",
          table: "Exercises",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "");

      migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [Modules] WHERE [Id] = 10)
                BEGIN
                    SET IDENTITY_INSERT [Modules] ON;
                    INSERT INTO [Modules] ([Id], [DisplayName], [Name])
                    VALUES (10, N'Ranking', N'ranking');
                    SET IDENTITY_INSERT [Modules] OFF;
                END
            ");

      migrationBuilder.InsertData(
          table: "PermissionComponents",
          columns: new[] { "Id", "Description", "DisplayName", "ModuleId", "Name", "PermissionType" },
          values: new object[,]
          {
                    { 60, "Permite ver el ranking propio y crear desafíos", "Ver ranking propio", 10, "ranking:view_own", "Simple" },
                    { 61, "Permite ver el ranking completo", "Ver ranking", 10, "ranking:view", "Simple" },
                    { 62, "Permite gestionar la tabla anual y validar desafíos", "Gestionar ranking", 10, "ranking:manage", "Simple" }
          });

      migrationBuilder.InsertData(
          table: "RolCompositePermission",
          columns: new[] { "PermissionComponentId", "RoleId" },
          values: new object[,]
          {
                    { 60, 100 },
                    { 60, 102 },
                    { 61, 100 },
                    { 62, 100 }
          });

      migrationBuilder.CreateIndex(
          name: "IX_Stats_RecordedByTeacherId",
          table: "Stats",
          column: "RecordedByTeacherId");

      // Add foreign key to Person or Persons table depending on which exists
      migrationBuilder.Sql(@"
                DECLARE @tableName NVARCHAR(50) = 'Person';
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Person]') AND type in (N'U'))
                BEGIN
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Persons]') AND type in (N'U'))
                        SET @tableName = 'Persons';
                END

                DECLARE @sql NVARCHAR(MAX) = 'ALTER TABLE [Stats] ADD CONSTRAINT [FK_Stats_' + @tableName + '_RecordedByTeacherId] FOREIGN KEY ([RecordedByTeacherId]) REFERENCES [' + @tableName + '] ([Id]) ON DELETE NO ACTION';
                EXEC sp_executesql @sql;
            ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      // Drop the foreign key dynamically based on which name exists
      migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Stats_Person_RecordedByTeacherId')
                    ALTER TABLE [Stats] DROP CONSTRAINT [FK_Stats_Person_RecordedByTeacherId];
                ELSE IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Stats_Persons_RecordedByTeacherId')
                    ALTER TABLE [Stats] DROP CONSTRAINT [FK_Stats_Persons_RecordedByTeacherId];
            ");

      migrationBuilder.DropIndex(
          name: "IX_Stats_RecordedByTeacherId",
          table: "Stats");

      migrationBuilder.DeleteData(
          table: "RolCompositePermission",
          keyColumns: new[] { "PermissionComponentId", "RoleId" },
          keyValues: new object[] { 60, 100 });

      migrationBuilder.DeleteData(
          table: "RolCompositePermission",
          keyColumns: new[] { "PermissionComponentId", "RoleId" },
          keyValues: new object[] { 60, 102 });

      migrationBuilder.DeleteData(
          table: "RolCompositePermission",
          keyColumns: new[] { "PermissionComponentId", "RoleId" },
          keyValues: new object[] { 61, 100 });

      migrationBuilder.DeleteData(
          table: "RolCompositePermission",
          keyColumns: new[] { "PermissionComponentId", "RoleId" },
          keyValues: new object[] { 62, 100 });

      migrationBuilder.DeleteData(
          table: "PermissionComponents",
          keyColumn: "Id",
          keyValue: 60);

      migrationBuilder.DeleteData(
          table: "PermissionComponents",
          keyColumn: "Id",
          keyValue: 61);

      migrationBuilder.DeleteData(
          table: "PermissionComponents",
          keyColumn: "Id",
          keyValue: 62);

      migrationBuilder.DeleteData(
          table: "Modules",
          keyColumn: "Id",
          keyValue: 10);

      migrationBuilder.DropColumn(
          name: "Milestones",
          table: "Stats");

      migrationBuilder.DropColumn(
          name: "Observations",
          table: "Stats");

      migrationBuilder.DropColumn(
          name: "RecordedAt",
          table: "Stats");

      migrationBuilder.DropColumn(
          name: "RecordedByTeacherId",
          table: "Stats");

      migrationBuilder.DropColumn(
          name: "Category",
          table: "Exercises");

      migrationBuilder.DropColumn(
          name: "Description",
          table: "Exercises");
    }
  }
}
