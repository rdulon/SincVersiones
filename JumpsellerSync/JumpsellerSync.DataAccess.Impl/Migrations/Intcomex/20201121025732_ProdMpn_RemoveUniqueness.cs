using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Intcomex
{
    public partial class ProdMpn_RemoveUniqueness : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Mpn",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Mpn",
                table: "Products",
                column: "Mpn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Mpn",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Mpn",
                table: "Products",
                column: "Mpn",
                unique: true);
        }
    }
}
