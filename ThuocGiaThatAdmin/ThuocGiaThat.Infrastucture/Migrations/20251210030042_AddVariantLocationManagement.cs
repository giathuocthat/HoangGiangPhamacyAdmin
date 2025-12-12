using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantLocationManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VariantLocationStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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

            migrationBuilder.CreateTable(
                name: "LocationStockMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    FromLocationCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ToLocationCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MovedByUserId = table.Column<int>(type: "int", nullable: true),
                    MovementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VariantLocationStockId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationStockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationStockMovements_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationStockMovements_VariantLocationStocks_VariantLocationStockId",
                        column: x => x.VariantLocationStockId,
                        principalTable: "VariantLocationStocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LocationStockMovements_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationStockMovements_FromLocationCode",
                table: "LocationStockMovements",
                column: "FromLocationCode");

            migrationBuilder.CreateIndex(
                name: "IX_LocationStockMovements_MovementDate",
                table: "LocationStockMovements",
                column: "MovementDate");

            migrationBuilder.CreateIndex(
                name: "IX_LocationStockMovements_ProductVariantId",
                table: "LocationStockMovements",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationStockMovements_ToLocationCode",
                table: "LocationStockMovements",
                column: "ToLocationCode");

            migrationBuilder.CreateIndex(
                name: "IX_LocationStockMovements_VariantLocationStockId",
                table: "LocationStockMovements",
                column: "VariantLocationStockId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationStockMovements_WarehouseId",
                table: "LocationStockMovements",
                column: "WarehouseId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationStockMovements");

            migrationBuilder.DropTable(
                name: "VariantLocationStocks");
        }
    }
}
