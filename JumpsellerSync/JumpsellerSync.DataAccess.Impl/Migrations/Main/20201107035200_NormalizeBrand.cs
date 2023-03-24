using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class NormalizeBrand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Brands_ProviderBrandId", table: "Brands");

            migrationBuilder.DropColumn(name: "ProviderBrandId", table: "Brands");

            migrationBuilder.RenameColumn("Description", "Brands", "Name");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Brands",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_NormalizedName",
                table: "Brands",
                column: "NormalizedName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Brands_NormalizedName", table: "Brands");

            migrationBuilder.DropColumn(name: "NormalizedName", table: "Brands");

            migrationBuilder.RenameColumn("Name", "Brands", "Description");

            migrationBuilder.AddColumn<string>(
                name: "ProviderBrandId",
                table: "Brands",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_ProviderBrandId",
                table: "Brands",
                column: "ProviderBrandId");
        }
    }
}
