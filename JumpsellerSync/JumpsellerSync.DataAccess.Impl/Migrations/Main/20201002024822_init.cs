using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

using System;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Main
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ProviderBrandId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provider",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    NextSynchronization = table.Column<DateTime>(nullable: true),
                    SynchronizationType = table.Column<string>(nullable: false),
                    StartTime = table.Column<TimeSpan>(nullable: true),
                    Interval = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Stock = table.Column<double>(nullable: false),
                    SKU = table.Column<string>(nullable: true),
                    BrandId = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    Format = table.Column<int>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    Width = table.Column<double>(nullable: false),
                    Height = table.Column<double>(nullable: false),
                    Length = table.Column<double>(nullable: false),
                    IsDigital = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Hour",
                columns: table => new
                {
                    HourId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Time = table.Column<TimeSpan>(nullable: false),
                    HourlyProviderId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hour", x => x.HourId);
                    table.ForeignKey(
                        name: "FK_Hour_Provider_HourlyProviderId",
                        column: x => x.HourlyProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ProviderCategoryId = table.Column<string>(nullable: false),
                    ProductId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brands_ProviderBrandId",
                table: "Brands",
                column: "ProviderBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ProductId",
                table: "Categories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ProviderCategoryId",
                table: "Categories",
                column: "ProviderCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Hour_HourlyProviderId",
                table: "Hour",
                column: "HourlyProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Provider_Name",
                table: "Provider",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provider_Priority",
                table: "Provider",
                column: "Priority",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provider_Url",
                table: "Provider",
                column: "Url",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Hour");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Provider");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}
