using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class ProductProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProviderId",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Products");
        }
    }
}
