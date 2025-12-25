using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class addPropsForProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductActiveIngredient_ActiveIngredient_ActiveIngredientId",
                table: "ProductActiveIngredient");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductActiveIngredient_Products_ProductId",
                table: "ProductActiveIngredient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductActiveIngredient",
                table: "ProductActiveIngredient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActiveIngredient",
                table: "ActiveIngredient");

            migrationBuilder.RenameTable(
                name: "ProductActiveIngredient",
                newName: "ProductActiveIngredients");

            migrationBuilder.RenameTable(
                name: "ActiveIngredient",
                newName: "ActiveIngredients");

            migrationBuilder.RenameIndex(
                name: "IX_ProductActiveIngredient_ProductId",
                table: "ProductActiveIngredients",
                newName: "IX_ProductActiveIngredients_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductActiveIngredient_ActiveIngredientId",
                table: "ProductActiveIngredients",
                newName: "IX_ProductActiveIngredients_ActiveIngredientId");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductActiveIngredients",
                table: "ProductActiveIngredients",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActiveIngredients",
                table: "ActiveIngredients",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 25, 2, 46, 37, 519, DateTimeKind.Utc).AddTicks(474));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 25, 2, 46, 37, 519, DateTimeKind.Utc).AddTicks(478));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 25, 2, 46, 37, 519, DateTimeKind.Utc).AddTicks(480));

            migrationBuilder.AddForeignKey(
                name: "FK_ProductActiveIngredients_ActiveIngredients_ActiveIngredientId",
                table: "ProductActiveIngredients",
                column: "ActiveIngredientId",
                principalTable: "ActiveIngredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductActiveIngredients_Products_ProductId",
                table: "ProductActiveIngredients",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductActiveIngredients_ActiveIngredients_ActiveIngredientId",
                table: "ProductActiveIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductActiveIngredients_Products_ProductId",
                table: "ProductActiveIngredients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductActiveIngredients",
                table: "ProductActiveIngredients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActiveIngredients",
                table: "ActiveIngredients");

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

            migrationBuilder.RenameTable(
                name: "ProductActiveIngredients",
                newName: "ProductActiveIngredient");

            migrationBuilder.RenameTable(
                name: "ActiveIngredients",
                newName: "ActiveIngredient");

            migrationBuilder.RenameIndex(
                name: "IX_ProductActiveIngredients_ProductId",
                table: "ProductActiveIngredient",
                newName: "IX_ProductActiveIngredient_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductActiveIngredients_ActiveIngredientId",
                table: "ProductActiveIngredient",
                newName: "IX_ProductActiveIngredient_ActiveIngredientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductActiveIngredient",
                table: "ProductActiveIngredient",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActiveIngredient",
                table: "ActiveIngredient",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 23, 3, 31, 22, 377, DateTimeKind.Utc).AddTicks(5351));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 23, 3, 31, 22, 377, DateTimeKind.Utc).AddTicks(5354));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 23, 3, 31, 22, 377, DateTimeKind.Utc).AddTicks(5359));

            migrationBuilder.AddForeignKey(
                name: "FK_ProductActiveIngredient_ActiveIngredient_ActiveIngredientId",
                table: "ProductActiveIngredient",
                column: "ActiveIngredientId",
                principalTable: "ActiveIngredient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductActiveIngredient_Products_ProductId",
                table: "ProductActiveIngredient",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
