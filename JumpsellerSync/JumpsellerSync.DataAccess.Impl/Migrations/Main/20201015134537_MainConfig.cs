using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class MainConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Jumpseller_AuthorizationFinished",
                table: "MainConfiguration");

            migrationBuilder.DropColumn(
                name: "Jumpseller_AuthorizationStarted",
                table: "MainConfiguration");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Jumpseller_AuthorizationFinished",
                table: "MainConfiguration",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Jumpseller_AuthorizationStarted",
                table: "MainConfiguration",
                type: "boolean",
                nullable: true);
        }
    }
}
