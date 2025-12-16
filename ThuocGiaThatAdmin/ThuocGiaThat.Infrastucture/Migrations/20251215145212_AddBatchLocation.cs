using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddBatchLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationStockMovements_VariantLocationStocks_VariantLocationStockId",
                table: "LocationStockMovements");

            migrationBuilder.DropTable(
                name: "VariantLocationStocks");

            migrationBuilder.RenameColumn(
                name: "VariantLocationStockId",
                table: "LocationStockMovements",
                newName: "BatchLocationStockId");

            migrationBuilder.RenameIndex(
                name: "IX_LocationStockMovements_VariantLocationStockId",
                table: "LocationStockMovements",
                newName: "IX_LocationStockMovements_BatchLocationStockId");

            migrationBuilder.CreateTable(
                name: "BatchLocationStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryBatchId = table.Column<int>(type: "int", nullable: false),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZoneName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RackName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ShelfName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BinName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    QuantityReserved = table.Column<int>(type: "int", nullable: false),
                    IsPrimaryLocation = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchLocationStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchLocationStocks_InventoryBatches_InventoryBatchId",
                        column: x => x.InventoryBatchId,
                        principalTable: "InventoryBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchLocationStocks_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BatchLocationStocks_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchLocationStocks_InventoryBatchId",
                table: "BatchLocationStocks",
                column: "InventoryBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchLocationStocks_InventoryBatchId_WarehouseId_LocationCode",
                table: "BatchLocationStocks",
                columns: new[] { "InventoryBatchId", "WarehouseId", "LocationCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BatchLocationStocks_IsPrimaryLocation",
                table: "BatchLocationStocks",
                column: "IsPrimaryLocation");

            migrationBuilder.CreateIndex(
                name: "IX_BatchLocationStocks_LocationCode",
                table: "BatchLocationStocks",
                column: "LocationCode");

            migrationBuilder.CreateIndex(
                name: "IX_BatchLocationStocks_ProductVariantId",
                table: "BatchLocationStocks",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchLocationStocks_WarehouseId",
                table: "BatchLocationStocks",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationStockMovements_BatchLocationStocks_BatchLocationStockId",
                table: "LocationStockMovements",
                column: "BatchLocationStockId",
                principalTable: "BatchLocationStocks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationStockMovements_BatchLocationStocks_BatchLocationStockId",
                table: "LocationStockMovements");

            migrationBuilder.DropTable(
                name: "BatchLocationStocks");

            migrationBuilder.RenameColumn(
                name: "BatchLocationStockId",
                table: "LocationStockMovements",
                newName: "VariantLocationStockId");

            migrationBuilder.RenameIndex(
                name: "IX_LocationStockMovements_BatchLocationStockId",
                table: "LocationStockMovements",
                newName: "IX_LocationStockMovements_VariantLocationStockId");

            migrationBuilder.CreateTable(
                name: "VariantLocationStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    BinName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsPrimaryLocation = table.Column<bool>(type: "bit", nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    QuantityReserved = table.Column<int>(type: "int", nullable: false),
                    RackName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ShelfName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ZoneName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantLocationStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VariantLocationStocks_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VariantLocationStocks_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VariantLocationStocks_IsPrimaryLocation",
                table: "VariantLocationStocks",
                column: "IsPrimaryLocation");

            migrationBuilder.CreateIndex(
                name: "IX_VariantLocationStocks_LocationCode",
                table: "VariantLocationStocks",
                column: "LocationCode");

            migrationBuilder.CreateIndex(
                name: "IX_VariantLocationStocks_ProductVariantId",
                table: "VariantLocationStocks",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantLocationStocks_ProductVariantId_WarehouseId_LocationCode",
                table: "VariantLocationStocks",
                columns: new[] { "ProductVariantId", "WarehouseId", "LocationCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VariantLocationStocks_WarehouseId",
                table: "VariantLocationStocks",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationStockMovements_VariantLocationStocks_VariantLocationStockId",
                table: "LocationStockMovements",
                column: "VariantLocationStockId",
                principalTable: "VariantLocationStocks",
                principalColumn: "Id");
        }
    }
}
