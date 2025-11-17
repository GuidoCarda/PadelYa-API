using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
  /// <inheritdoc />
  public partial class AddPersonData : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_Bookings_Person_PersonId",
          table: "Bookings");

      migrationBuilder.DropForeignKey(
          name: "FK_CouplePlayer_Person_PlayersId",
          table: "CouplePlayer");

      // Drop LessonAttendances foreign keys only if the table exists
      migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LessonAttendances]') AND type in (N'U'))
                BEGIN
                    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LessonAttendances_Person_PersonId')
                        ALTER TABLE [LessonAttendances] DROP CONSTRAINT [FK_LessonAttendances_Person_PersonId];
                    
                    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LessonAttendances_Person_RecordedByTeacherId')
                        ALTER TABLE [LessonAttendances] DROP CONSTRAINT [FK_LessonAttendances_Person_RecordedByTeacherId];
                END
            ");

      migrationBuilder.DropForeignKey(
          name: "FK_LessonEnrollments_Person_PersonId",
          table: "LessonEnrollments");

      migrationBuilder.DropForeignKey(
          name: "FK_Lessons_Person_TeacherId",
          table: "Lessons");

      migrationBuilder.DropForeignKey(
          name: "FK_Repairs_Person_PersonId",
          table: "Repairs");

      migrationBuilder.DropForeignKey(
          name: "FK_RoutinePlayer_Person_PlayerId",
          table: "RoutinePlayer");

      migrationBuilder.DropForeignKey(
          name: "FK_Routines_Person_CreatorId",
          table: "Routines");

      migrationBuilder.DropForeignKey(
          name: "FK_Stats_Person_PlayerId",
          table: "Stats");

      // Drop Stats RecordedByTeacherId foreign key only if it exists
      migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Stats_Person_RecordedByTeacherId')
                    ALTER TABLE [Stats] DROP CONSTRAINT [FK_Stats_Person_RecordedByTeacherId];
                ELSE IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Stats_Persons_RecordedByTeacherId')
                    ALTER TABLE [Stats] DROP CONSTRAINT [FK_Stats_Persons_RecordedByTeacherId];
            ");

      migrationBuilder.DropForeignKey(
          name: "FK_Users_Person_PersonId",
          table: "Users");

      migrationBuilder.DropPrimaryKey(
          name: "PK_Person",
          table: "Person");

      migrationBuilder.RenameTable(
          name: "Person",
          newName: "Persons");

      migrationBuilder.AddColumn<string>(
          name: "Email",
          table: "Persons",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "");

      migrationBuilder.AddColumn<string>(
          name: "Name",
          table: "Persons",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "");

      migrationBuilder.AddColumn<string>(
          name: "PhoneNumber",
          table: "Persons",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "");

      migrationBuilder.AddColumn<string>(
          name: "Surname",
          table: "Persons",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "");

      migrationBuilder.AddPrimaryKey(
          name: "PK_Persons",
          table: "Persons",
          column: "Id");

      migrationBuilder.UpdateData(
          table: "Persons",
          keyColumn: "Id",
          keyValue: 1,
          columns: new[] { "Email", "Name", "PhoneNumber", "Surname" },
          values: new object[] { "", "", "", "" });

      migrationBuilder.UpdateData(
          table: "Persons",
          keyColumn: "Id",
          keyValue: 2,
          columns: new[] { "Email", "Name", "PhoneNumber", "Surname" },
          values: new object[] { "", "", "", "" });

      migrationBuilder.UpdateData(
          table: "Persons",
          keyColumn: "Id",
          keyValue: 3,
          columns: new[] { "Email", "Name", "PhoneNumber", "Surname" },
          values: new object[] { "", "", "", "" });

      migrationBuilder.UpdateData(
          table: "Persons",
          keyColumn: "Id",
          keyValue: 4,
          columns: new[] { "Email", "Name", "PhoneNumber", "Surname" },
          values: new object[] { "", "", "", "" });

      migrationBuilder.AddForeignKey(
          name: "FK_Bookings_Persons_PersonId",
          table: "Bookings",
          column: "PersonId",
          principalTable: "Persons",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_CouplePlayer_Persons_PlayersId",
          table: "CouplePlayer",
          column: "PlayersId",
          principalTable: "Persons",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      // Add LessonAttendances foreign keys only if the table exists
      migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LessonAttendances]') AND type in (N'U'))
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LessonAttendances_Persons_PersonId')
                        ALTER TABLE [LessonAttendances] ADD CONSTRAINT [FK_LessonAttendances_Persons_PersonId] 
                            FOREIGN KEY ([PersonId]) REFERENCES [Persons] ([Id]) ON DELETE NO ACTION;
                    
                    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LessonAttendances_Persons_RecordedByTeacherId')
                        ALTER TABLE [LessonAttendances] ADD CONSTRAINT [FK_LessonAttendances_Persons_RecordedByTeacherId] 
                            FOREIGN KEY ([RecordedByTeacherId]) REFERENCES [Persons] ([Id]) ON DELETE NO ACTION;
                END
            ");

      migrationBuilder.AddForeignKey(
          name: "FK_LessonEnrollments_Persons_PersonId",
          table: "LessonEnrollments",
          column: "PersonId",
          principalTable: "Persons",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);

      migrationBuilder.AddForeignKey(
          name: "FK_Lessons_Persons_TeacherId",
          table: "Lessons",
          column: "TeacherId",
          principalTable: "Persons",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_Repairs_Persons_PersonId",
          table: "Repairs",
          column: "PersonId",
          principalTable: "Persons",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);

      migrationBuilder.AddForeignKey(
          name: "FK_RoutinePlayer_Persons_PlayerId",
          table: "RoutinePlayer",
          column: "PlayerId",
          principalTable: "Persons",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);

      migrationBuilder.AddForeignKey(
          name: "FK_Routines_Persons_CreatorId",
          table: "Routines",
          column: "CreatorId",
          principalTable: "Persons",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_Stats_Persons_PlayerId",
          table: "Stats",
          column: "PlayerId",
          principalTable: "Persons",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);

      // Add Stats RecordedByTeacherId foreign key only if the column exists
      migrationBuilder.Sql(@"
                IF COL_LENGTH('Stats', 'RecordedByTeacherId') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Stats_Persons_RecordedByTeacherId')
                        ALTER TABLE [Stats] ADD CONSTRAINT [FK_Stats_Persons_RecordedByTeacherId] 
                            FOREIGN KEY ([RecordedByTeacherId]) REFERENCES [Persons] ([Id]) ON DELETE NO ACTION;
                END
            ");

      migrationBuilder.AddForeignKey(
          name: "FK_Users_Persons_PersonId",
          table: "Users",
          column: "PersonId",
          principalTable: "Persons",
          principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_Bookings_Persons_PersonId",
          table: "Bookings");

      migrationBuilder.DropForeignKey(
          name: "FK_CouplePlayer_Persons_PlayersId",
          table: "CouplePlayer");

      migrationBuilder.DropForeignKey(
          name: "FK_LessonEnrollments_Persons_PersonId",
          table: "LessonEnrollments");

      migrationBuilder.DropForeignKey(
          name: "FK_Lessons_Persons_TeacherId",
          table: "Lessons");

      migrationBuilder.DropForeignKey(
          name: "FK_Repairs_Persons_PersonId",
          table: "Repairs");

      migrationBuilder.DropForeignKey(
          name: "FK_RoutinePlayer_Persons_PlayerId",
          table: "RoutinePlayer");

      migrationBuilder.DropForeignKey(
          name: "FK_Routines_Persons_CreatorId",
          table: "Routines");

      migrationBuilder.DropForeignKey(
          name: "FK_Stats_Persons_PlayerId",
          table: "Stats");

      migrationBuilder.DropForeignKey(
          name: "FK_Users_Persons_PersonId",
          table: "Users");

      migrationBuilder.DropPrimaryKey(
          name: "PK_Persons",
          table: "Persons");

      migrationBuilder.DropColumn(
          name: "Email",
          table: "Persons");

      migrationBuilder.DropColumn(
          name: "Name",
          table: "Persons");

      migrationBuilder.DropColumn(
          name: "PhoneNumber",
          table: "Persons");

      migrationBuilder.DropColumn(
          name: "Surname",
          table: "Persons");

      migrationBuilder.RenameTable(
          name: "Persons",
          newName: "Person");

      migrationBuilder.AddPrimaryKey(
          name: "PK_Person",
          table: "Person",
          column: "Id");

      migrationBuilder.AddForeignKey(
          name: "FK_Bookings_Person_PersonId",
          table: "Bookings",
          column: "PersonId",
          principalTable: "Person",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_CouplePlayer_Person_PlayersId",
          table: "CouplePlayer",
          column: "PlayersId",
          principalTable: "Person",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_LessonEnrollments_Person_PersonId",
          table: "LessonEnrollments",
          column: "PersonId",
          principalTable: "Person",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);

      migrationBuilder.AddForeignKey(
          name: "FK_Lessons_Person_TeacherId",
          table: "Lessons",
          column: "TeacherId",
          principalTable: "Person",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_Repairs_Person_PersonId",
          table: "Repairs",
          column: "PersonId",
          principalTable: "Person",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);

      migrationBuilder.AddForeignKey(
          name: "FK_RoutinePlayer_Person_PlayerId",
          table: "RoutinePlayer",
          column: "PlayerId",
          principalTable: "Person",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);

      migrationBuilder.AddForeignKey(
          name: "FK_Routines_Person_CreatorId",
          table: "Routines",
          column: "CreatorId",
          principalTable: "Person",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_Stats_Person_PlayerId",
          table: "Stats",
          column: "PlayerId",
          principalTable: "Person",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);

      migrationBuilder.AddForeignKey(
          name: "FK_Users_Person_PersonId",
          table: "Users",
          column: "PersonId",
          principalTable: "Person",
          principalColumn: "Id");
    }
  }
}
