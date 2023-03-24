using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class provider_ref_redcetus_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Providers_ProviderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_SynchronizationSessionInfo_ProviderProductId",
                table: "SynchronizationSessionInfo");

            migrationBuilder.DropIndex(
                name: "IX_Products_JumpsellerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProviderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProviderProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProviderProductId",
                table: "SynchronizationSessionInfo");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProviderProductId",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "JumpsellerId",
                table: "Products",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Products_JumpsellerId",
                table: "Products",
                column: "JumpsellerId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_JumpsellerId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ProviderProductId",
                table: "SynchronizationSessionInfo",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "JumpsellerId",
                table: "Products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderId",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderProductId",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationSessionInfo_ProviderProductId",
                table: "SynchronizationSessionInfo",
                column: "ProviderProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_JumpsellerId",
                table: "Products",
                column: "JumpsellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProviderId",
                table: "Products",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProviderProductId",
                table: "Products",
                column: "ProviderProductId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Providers_ProviderId",
                table: "Products",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
