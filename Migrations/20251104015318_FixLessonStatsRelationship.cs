using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
  /// <inheritdoc />
  public partial class FixLessonStatsRelationship : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_RankingTraces_AnnualTables_AnnualTableId",
          table: "RankingTraces");

      migrationBuilder.DropForeignKey(
          name: "FK_RankingTraces_RankingEntries_RankingEntryId",
          table: "RankingTraces");

      migrationBuilder.DropForeignKey(
          name: "FK_Stats_Persons_RecordedByTeacherId",
          table: "Stats");

      migrationBuilder.DeleteData(
          table: "RolCompositePermission",
          keyColumns: new[] { "PermissionComponentId", "RoleId" },
          keyValues: new object[] { 50, 102 });

      // Create ClassTypes table only if it doesn't exist
      migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ClassTypes]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [ClassTypes] (
                        [Id] int NOT NULL IDENTITY,
                        [Name] nvarchar(100) NOT NULL,
                        [Description] nvarchar(500) NULL,
                        [Level] nvarchar(50) NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        CONSTRAINT [PK_ClassTypes] PRIMARY KEY ([Id])
                    );
                END
            ");

      // Create LessonAttendances table only if it doesn't exist
      migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LessonAttendances]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [LessonAttendances] (
                        [Id] int NOT NULL IDENTITY,
                        [LessonId] int NOT NULL,
                        [PersonId] int NOT NULL,
                        [Status] int NOT NULL,
                        [Notes] nvarchar(500) NULL,
                        [RecordedAt] datetime2 NOT NULL,
                        [RecordedByTeacherId] int NULL,
                        CONSTRAINT [PK_LessonAttendances] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_LessonAttendances_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_LessonAttendances_Persons_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [Persons] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_LessonAttendances_Persons_RecordedByTeacherId] FOREIGN KEY ([RecordedByTeacherId]) REFERENCES [Persons] ([Id])
                    );
                    
                    CREATE INDEX [IX_LessonAttendances_LessonId] ON [LessonAttendances] ([LessonId]);
                    CREATE INDEX [IX_LessonAttendances_PersonId] ON [LessonAttendances] ([PersonId]);
                    CREATE INDEX [IX_LessonAttendances_RecordedByTeacherId] ON [LessonAttendances] ([RecordedByTeacherId]);
                END
            ");

      migrationBuilder.AddForeignKey(
          name: "FK_RankingTraces_AnnualTables_AnnualTableId",
          table: "RankingTraces",
          column: "AnnualTableId",
          principalTable: "AnnualTables",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_RankingTraces_RankingEntries_RankingEntryId",
          table: "RankingTraces",
          column: "RankingEntryId",
          principalTable: "RankingEntries",
          principalColumn: "Id",
          onDelete: ReferentialAction.NoAction);

      migrationBuilder.AddForeignKey(
          name: "FK_Stats_Persons_RecordedByTeacherId",
          table: "Stats",
          column: "RecordedByTeacherId",
          principalTable: "Persons",
          principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_RankingTraces_AnnualTables_AnnualTableId",
          table: "RankingTraces");

      migrationBuilder.DropForeignKey(
          name: "FK_RankingTraces_RankingEntries_RankingEntryId",
          table: "RankingTraces");

      migrationBuilder.DropForeignKey(
          name: "FK_Stats_Persons_RecordedByTeacherId",
          table: "Stats");

      // Drop tables only if they exist
      migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ClassTypes]') AND type in (N'U'))
                BEGIN
                    DROP TABLE [ClassTypes];
                END
            ");

      migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LessonAttendances]') AND type in (N'U'))
                BEGIN
                    DROP TABLE [LessonAttendances];
                END
            ");

      migrationBuilder.InsertData(
          table: "RolCompositePermission",
          columns: new[] { "PermissionComponentId", "RoleId" },
          values: new object[] { 50, 102 });

      migrationBuilder.AddForeignKey(
          name: "FK_RankingTraces_AnnualTables_AnnualTableId",
          table: "RankingTraces",
          column: "AnnualTableId",
          principalTable: "AnnualTables",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);

      migrationBuilder.AddForeignKey(
          name: "FK_RankingTraces_RankingEntries_RankingEntryId",
          table: "RankingTraces",
          column: "RankingEntryId",
          principalTable: "RankingEntries",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);

      migrationBuilder.AddForeignKey(
          name: "FK_Stats_Persons_RecordedByTeacherId",
          table: "Stats",
          column: "RecordedByTeacherId",
          principalTable: "Persons",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);
    }
  }
}
