using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Intcomex
{
    public partial class DirtyProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Dirty",
                table: "Products",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dirty",
                table: "Products");
        }
    }
}
