using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class LocalProductDeleteCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalProducts_Products_ProductId",
                table: "LocalProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalProducts_Products_ProductId",
                table: "LocalProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalProducts_Products_ProductId",
                table: "LocalProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalProducts_Products_ProductId",
                table: "LocalProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
