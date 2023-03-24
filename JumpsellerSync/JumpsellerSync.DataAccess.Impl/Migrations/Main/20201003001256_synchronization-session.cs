using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

using System;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class synchronizationsession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hour_Provider_HourlyProviderId",
                table: "Hour");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Provider",
                table: "Provider");

            migrationBuilder.RenameTable(
                name: "Provider",
                newName: "Providers");

            migrationBuilder.RenameIndex(
                name: "IX_Provider_Url",
                table: "Providers",
                newName: "IX_Providers_Url");

            migrationBuilder.RenameIndex(
                name: "IX_Provider_Priority",
                table: "Providers",
                newName: "IX_Providers_Priority");

            migrationBuilder.RenameIndex(
                name: "IX_Provider_Name",
                table: "Providers",
                newName: "IX_Providers_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Providers",
                table: "Providers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SynchronizationSessions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ProviderId = table.Column<string>(nullable: false),
                    Running = table.Column<bool>(nullable: false),
                    SyncPrice = table.Column<bool>(nullable: false),
                    SyncStock = table.Column<bool>(nullable: false),
                    ProcessedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynchronizationSessions", x => new { x.Id, x.ProviderId });
                    table.ForeignKey(
                        name: "FK_SynchronizationSessions_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SynchronizationSessionInfo",
                columns: table => new
                {
                    SyncSessionInfoId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<string>(nullable: true),
                    Stock = table.Column<double>(nullable: true),
                    Price = table.Column<double>(nullable: true),
                    ProviderId = table.Column<string>(nullable: false),
                    SynchronizationSessionId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynchronizationSessionInfo", x => x.SyncSessionInfoId);
                    table.ForeignKey(
                        name: "FK_SynchronizationSessionInfo_SynchronizationSessions_Synchron~",
                        columns: x => new { x.SynchronizationSessionId, x.ProviderId },
                        principalTable: "SynchronizationSessions",
                        principalColumns: new[] { "Id", "ProviderId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationSessionInfo_SynchronizationSessionId_Provide~",
                table: "SynchronizationSessionInfo",
                columns: new[] { "SynchronizationSessionId", "ProviderId" });

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationSessions_ProviderId",
                table: "SynchronizationSessions",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hour_Providers_HourlyProviderId",
                table: "Hour",
                column: "HourlyProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hour_Providers_HourlyProviderId",
                table: "Hour");

            migrationBuilder.DropTable(
                name: "SynchronizationSessionInfo");

            migrationBuilder.DropTable(
                name: "SynchronizationSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Providers",
                table: "Providers");

            migrationBuilder.RenameTable(
                name: "Providers",
                newName: "Provider");

            migrationBuilder.RenameIndex(
                name: "IX_Providers_Url",
                table: "Provider",
                newName: "IX_Provider_Url");

            migrationBuilder.RenameIndex(
                name: "IX_Providers_Priority",
                table: "Provider",
                newName: "IX_Provider_Priority");

            migrationBuilder.RenameIndex(
                name: "IX_Providers_Name",
                table: "Provider",
                newName: "IX_Provider_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Provider",
                table: "Provider",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hour_Provider_HourlyProviderId",
                table: "Hour",
                column: "HourlyProviderId",
                principalTable: "Provider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
