using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddProductBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                table: "LocationStockMovements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostPrice",
                table: "InventoryBatches",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateTable(
                name: "ProductBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PurchaseOrderNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Supplier = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    QRCodePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBatches_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationStockMovements_BatchNumber",
                table: "LocationStockMovements",
                column: "BatchNumber");

            migrationBuilder.CreateIndex(
                name: "IX_LocationStockMovements_BatchNumber_WarehouseId_ToLocationCode",
                table: "LocationStockMovements",
                columns: new[] { "BatchNumber", "WarehouseId", "ToLocationCode" },
                unique: true,
                filter: "[ToLocationCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_BatchNumber",
                table: "ProductBatches",
                column: "BatchNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_ExpiryDate",
                table: "ProductBatches",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_IsActive",
                table: "ProductBatches",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_ProductVariantId",
                table: "ProductBatches",
                column: "ProductVariantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductBatches");

            migrationBuilder.DropIndex(
                name: "IX_LocationStockMovements_BatchNumber",
                table: "LocationStockMovements");

            migrationBuilder.DropIndex(
                name: "IX_LocationStockMovements_BatchNumber_WarehouseId_ToLocationCode",
                table: "LocationStockMovements");

            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "LocationStockMovements");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostPrice",
                table: "InventoryBatches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
