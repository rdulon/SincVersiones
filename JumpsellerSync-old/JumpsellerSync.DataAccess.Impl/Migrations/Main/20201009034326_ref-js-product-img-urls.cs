using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class refjsproductimgurls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JumpsellerId",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_JumpsellerId",
                table: "Products",
                column: "JumpsellerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_JumpsellerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "JumpsellerId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "text",
                nullable: true);
        }
    }
}
