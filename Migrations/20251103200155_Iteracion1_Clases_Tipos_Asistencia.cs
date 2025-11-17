using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
  /// <inheritdoc />
  public partial class Iteracion1_Clases_Tipos_Asistencia : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "ClassTypes",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
            Level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ClassTypes", x => x.Id);
          });

      // Check if Person table exists (pre-rename) or Persons table exists (post-rename)
      migrationBuilder.Sql(@"
                DECLARE @tableName NVARCHAR(50) = 'Person';
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Person]') AND type in (N'U'))
                BEGIN
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Persons]') AND type in (N'U'))
                        SET @tableName = 'Persons';
                END

                DECLARE @sql NVARCHAR(MAX) = '
                CREATE TABLE [LessonAttendances] (
                    [Id] int NOT NULL IDENTITY,
                    [LessonId] int NOT NULL,
                    [PersonId] int NOT NULL,
                    [Status] int NOT NULL,
                    [Notes] nvarchar(500) NULL,
                    [RecordedAt] datetime2 NOT NULL,
                    [RecordedByTeacherId] int NULL,
                    CONSTRAINT [PK_LessonAttendances] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_LessonAttendances_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_LessonAttendances_' + @tableName + '_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [' + @tableName + '] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_LessonAttendances_' + @tableName + '_RecordedByTeacherId] FOREIGN KEY ([RecordedByTeacherId]) REFERENCES [' + @tableName + '] ([Id]) ON DELETE NO ACTION
                );';

                EXEC sp_executesql @sql;
            ");

      migrationBuilder.CreateIndex(
          name: "IX_LessonAttendances_LessonId",
          table: "LessonAttendances",
          column: "LessonId");

      migrationBuilder.CreateIndex(
          name: "IX_LessonAttendances_PersonId",
          table: "LessonAttendances",
          column: "PersonId");

      migrationBuilder.CreateIndex(
          name: "IX_LessonAttendances_RecordedByTeacherId",
          table: "LessonAttendances",
          column: "RecordedByTeacherId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "ClassTypes");

      migrationBuilder.DropTable(
          name: "LessonAttendances");
    }
  }
}
