using Microsoft.EntityFrameworkCore.Migrations;

using System;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Intcomex
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: true),
                    ManufacturerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: true),
                    IntcomexCategoryId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_IntcomexCategoryId",
                        column: x => x.IntcomexCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RedcetusProductId = table.Column<string>(nullable: true),
                    BrandId = table.Column<string>(nullable: true),
                    Stock = table.Column<double>(nullable: false),
                    ProductCode = table.Column<string>(nullable: false),
                    Description = table.Column<string>(maxLength: 150, nullable: true),
                    Mpn = table.Column<string>(nullable: true),
                    CategoryId = table.Column<string>(nullable: true),
                    ProductType = table.Column<int>(nullable: false),
                    ProductLine = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    New = table.Column<bool>(nullable: false),
                    Price_UnitPrice = table.Column<double>(nullable: true),
                    Price_CurrencyId = table.Column<string>(nullable: true),
                    Price_PriceTaxAmount = table.Column<double>(nullable: true),
                    Price_PricePlusTax = table.Column<double>(nullable: true),
                    Price_OriginalPrice = table.Column<double>(nullable: true),
                    Price_OriginalPriceTaxAmount = table.Column<double>(nullable: true),
                    Price_OriginalPricePlusTax = table.Column<double>(nullable: true),
                    Price_BasePrice = table.Column<double>(nullable: true),
                    Price_BasePriceTaxAmount = table.Column<double>(nullable: true),
                    Price_BasePricePlusTax = table.Column<double>(nullable: true),
                    Price_RebateAmount = table.Column<double>(nullable: true),
                    Price_TaxRate = table.Column<double>(nullable: true),
                    OnSale = table.Column<bool>(nullable: false),
                    PrePurchaseStartDate = table.Column<DateTime>(nullable: false),
                    PrePurchaseEndDate = table.Column<DateTime>(nullable: false),
                    PrePurchaseActive = table.Column<bool>(nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IntcomexCategoryId",
                table: "Categories",
                column: "IntcomexCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Mpn",
                table: "Products",
                column: "Mpn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCode",
                table: "Products",
                column: "ProductCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_RedcetusProductId",
                table: "Products",
                column: "RedcetusProductId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
