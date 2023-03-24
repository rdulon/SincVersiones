using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Linkstore
{
    public partial class redcetusprodid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SynchronizeToRedcetus",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "RedcetusProductId",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Stock",
                table: "Products",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_RedcetusProductId",
                table: "Products",
                column: "RedcetusProductId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_RedcetusProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RedcetusProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SynchronizeToRedcetus",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
