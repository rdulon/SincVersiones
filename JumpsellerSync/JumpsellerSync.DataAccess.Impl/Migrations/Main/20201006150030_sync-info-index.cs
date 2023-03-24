using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class syncinfoindex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SynchronizationSessionInfo_ProductId",
                table: "SynchronizationSessionInfo");

            migrationBuilder.DropIndex(
                name: "IX_SynchronizationSessionInfo_ProviderProductId",
                table: "SynchronizationSessionInfo");

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationSessionInfo_ProductId",
                table: "SynchronizationSessionInfo",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationSessionInfo_ProviderProductId",
                table: "SynchronizationSessionInfo",
                column: "ProviderProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SynchronizationSessionInfo_ProductId",
                table: "SynchronizationSessionInfo");

            migrationBuilder.DropIndex(
                name: "IX_SynchronizationSessionInfo_ProviderProductId",
                table: "SynchronizationSessionInfo");

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationSessionInfo_ProductId",
                table: "SynchronizationSessionInfo",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationSessionInfo_ProviderProductId",
                table: "SynchronizationSessionInfo",
                column: "ProviderProductId",
                unique: true);
        }
    }
}
