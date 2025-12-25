using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class addprops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OverSaleNumber",
                table: "ProductVariants",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RatePrice",
                table: "ProductVariants",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatePriceUnit",
                table: "ProductVariants",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Indication",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Overdose",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 25, 7, 45, 36, 683, DateTimeKind.Utc).AddTicks(3576));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 25, 7, 45, 36, 683, DateTimeKind.Utc).AddTicks(3580));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 25, 7, 45, 36, 683, DateTimeKind.Utc).AddTicks(3584));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverSaleNumber",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "RatePrice",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "RatePriceUnit",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Indication",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Overdose",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 24, 4, 45, 35, 589, DateTimeKind.Utc).AddTicks(580));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 24, 4, 45, 35, 589, DateTimeKind.Utc).AddTicks(582));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 24, 4, 45, 35, 589, DateTimeKind.Utc).AddTicks(584));
        }
    }
}
