using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Linkstore
{
    public partial class alterbrand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Brands");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Brands",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Brands");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Brands",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
