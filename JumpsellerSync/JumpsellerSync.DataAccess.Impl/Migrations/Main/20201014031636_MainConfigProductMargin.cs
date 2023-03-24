using Microsoft.EntityFrameworkCore.Migrations;

using System;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class MainConfigProductMargin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Margin",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MainConfiguration",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Jumpseller_AuthorizationStarted = table.Column<bool>(nullable: true),
                    Jumpseller_AuthorizationFinished = table.Column<bool>(nullable: true),
                    Jumpseller_ApplicationAuthorized = table.Column<bool>(nullable: true),
                    Jumpseller_AccessToken = table.Column<string>(nullable: true),
                    Jumpseller_RefreshToken = table.Column<string>(nullable: true),
                    Jumpseller_AccessTokenType = table.Column<string>(nullable: true),
                    Jumpseller_TokenExpiresAt = table.Column<DateTime>(nullable: true),
                    Jumpseller_TokenCreatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainConfiguration", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MainConfiguration");

            migrationBuilder.DropColumn(
                name: "Margin",
                table: "Products");
        }
    }
}
