using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Intcomex
{
    public partial class SaveIntcomexSKU : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IntcomexSku",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_IntcomexSku",
                table: "Products",
                column: "IntcomexSku");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_IntcomexSku",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IntcomexSku",
                table: "Products");
        }
    }
}
