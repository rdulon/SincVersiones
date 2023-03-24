using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Linkstore
{
    public partial class renamecol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SynchronizeToJumpseller",
                table: "Products",
                newName: "SynchronizeToRedcetus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SynchronizeToRedcetus",
                table: "Products",
                newName: "SynchronizeToJumpseller");
        }
    }
}
