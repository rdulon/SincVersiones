using Microsoft.EntityFrameworkCore.Migrations;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class reconcile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SyncPrice",
                table: "SynchronizationSessions");

            migrationBuilder.DropColumn(
                name: "SyncStock",
                table: "SynchronizationSessions");

            migrationBuilder.AlterColumn<double>(
                name: "Stock",
                table: "SynchronizationSessionInfo",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "SynchronizationSessionInfo",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderProductId",
                table: "SynchronizationSessionInfo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderId",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderProductId",
                table: "Products",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Providers_ProviderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_SynchronizationSessionInfo_ProductId",
                table: "SynchronizationSessionInfo");

            migrationBuilder.DropIndex(
                name: "IX_SynchronizationSessionInfo_ProviderProductId",
                table: "SynchronizationSessionInfo");

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

            migrationBuilder.AddColumn<bool>(
                name: "SyncPrice",
                table: "SynchronizationSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SyncStock",
                table: "SynchronizationSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<double>(
                name: "Stock",
                table: "SynchronizationSessionInfo",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "SynchronizationSessionInfo",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double));
        }
    }
}
