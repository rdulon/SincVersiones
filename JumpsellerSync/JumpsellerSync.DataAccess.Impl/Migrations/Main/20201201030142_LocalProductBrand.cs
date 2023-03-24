using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class LocalProductBrand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrandId",
                table: "LocalProducts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalProducts_BrandId",
                table: "LocalProducts",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalProducts_Brands_BrandId",
                table: "LocalProducts",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalProducts_Brands_BrandId",
                table: "LocalProducts");

            migrationBuilder.DropIndex(
                name: "IX_LocalProducts_BrandId",
                table: "LocalProducts");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "LocalProducts");
        }
    }
}
