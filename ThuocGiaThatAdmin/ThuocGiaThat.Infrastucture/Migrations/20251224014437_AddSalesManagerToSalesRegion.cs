using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesManagerToSalesRegion : Migration
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

            migrationBuilder.AddColumn<string>(
                name: "SalesManagerId",
                table: "SalesRegions",
                type: "nvarchar(450)",
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
                columns: new[] { "CreatedDate", "SalesManagerId" },
                values: new object[] { new DateTime(2025, 12, 24, 1, 44, 35, 766, DateTimeKind.Utc).AddTicks(3433), null });

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "SalesManagerId" },
                values: new object[] { new DateTime(2025, 12, 24, 1, 44, 35, 766, DateTimeKind.Utc).AddTicks(3441), null });

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "SalesManagerId" },
                values: new object[] { new DateTime(2025, 12, 24, 1, 44, 35, 766, DateTimeKind.Utc).AddTicks(3443), null });

            migrationBuilder.CreateIndex(
                name: "IX_SalesRegions_SalesManagerId",
                table: "SalesRegions",
                column: "SalesManagerId");

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
                name: "FK_SalesRegions_Users_SalesManagerId",
                table: "SalesRegions",
                column: "SalesManagerId",
                principalTable: "Users",
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
                name: "FK_SalesRegions_Users_SalesManagerId",
                table: "SalesRegions");

            migrationBuilder.DropIndex(
                name: "IX_SalesRegions_SalesManagerId",
                table: "SalesRegions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductActiveIngredients",
                table: "ProductActiveIngredients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActiveIngredients",
                table: "ActiveIngredients");

            migrationBuilder.DropColumn(
                name: "SalesManagerId",
                table: "SalesRegions");

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
