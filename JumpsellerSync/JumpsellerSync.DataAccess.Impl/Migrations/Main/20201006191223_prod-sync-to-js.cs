using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class prodsynctojs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SynchronizedToJumpseller",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SynchronizedToJumpseller",
                table: "Products",
                column: "SynchronizedToJumpseller");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_SynchronizedToJumpseller",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SynchronizedToJumpseller",
                table: "Products");
        }
    }
}
