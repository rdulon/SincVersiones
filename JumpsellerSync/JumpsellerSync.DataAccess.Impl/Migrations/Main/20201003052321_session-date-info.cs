using Microsoft.EntityFrameworkCore.Migrations;

using System;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class sessiondateinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SynchronizationSessions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SynchronizationSessions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SynchronizationSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SynchronizationSessions");
        }
    }
}
