using Microsoft.EntityFrameworkCore.Migrations;

using System;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Intcomex
{
    public partial class RemoveUnneccessaryColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Model",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "New",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OnSale",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PrePurchaseActive",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PrePurchaseEndDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PrePurchaseStartDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductLine",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price_BasePrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price_BasePricePlusTax",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price_BasePriceTaxAmount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price_OriginalPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price_OriginalPricePlusTax",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price_OriginalPriceTaxAmount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price_PricePlusTax",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price_PriceTaxAmount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price_RebateAmount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price_TaxRate",
                table: "Products");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCatalogUpdate",
                table: "Configuration",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastCatalogUpdate",
                table: "Configuration");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "New",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OnSale",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PrePurchaseActive",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrePurchaseEndDate",
                table: "Products",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PrePurchaseStartDate",
                table: "Products",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProductLine",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price_BasePrice",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price_BasePricePlusTax",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price_BasePriceTaxAmount",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price_OriginalPrice",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price_OriginalPricePlusTax",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price_OriginalPriceTaxAmount",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price_PricePlusTax",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price_PriceTaxAmount",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price_RebateAmount",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price_TaxRate",
                table: "Products",
                type: "double precision",
                nullable: true);
        }
    }
}
