using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVariantIdColumn : Migration
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

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCollectionItems_Products_ProductId",
                table: "ProductCollectionItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCollectionItems",
                table: "ProductCollectionItems");

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

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductCollectionItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "ProductCollectionItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCollectionItems",
                table: "ProductCollectionItems",
                columns: new[] { "ProductCollectionId", "ProductVariantId" });

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
                value: new DateTime(2025, 12, 24, 1, 33, 11, 218, DateTimeKind.Utc).AddTicks(4261));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 24, 1, 33, 11, 218, DateTimeKind.Utc).AddTicks(4265));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 24, 1, 33, 11, 218, DateTimeKind.Utc).AddTicks(4267));

            migrationBuilder.CreateIndex(
                name: "IX_ProductCollectionItems_ProductVariantId",
                table: "ProductCollectionItems",
                column: "ProductVariantId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCollectionItems_ProductVariants_ProductVariantId",
                table: "ProductCollectionItems",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCollectionItems_Products_ProductId",
                table: "ProductCollectionItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
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

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCollectionItems_ProductVariants_ProductVariantId",
                table: "ProductCollectionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCollectionItems_Products_ProductId",
                table: "ProductCollectionItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCollectionItems",
                table: "ProductCollectionItems");

            migrationBuilder.DropIndex(
                name: "IX_ProductCollectionItems_ProductVariantId",
                table: "ProductCollectionItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductActiveIngredients",
                table: "ProductActiveIngredients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActiveIngredients",
                table: "ActiveIngredients");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "ProductCollectionItems");

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

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductCollectionItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCollectionItems",
                table: "ProductCollectionItems",
                columns: new[] { "ProductCollectionId", "ProductId" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCollectionItems_Products_ProductId",
                table: "ProductCollectionItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
