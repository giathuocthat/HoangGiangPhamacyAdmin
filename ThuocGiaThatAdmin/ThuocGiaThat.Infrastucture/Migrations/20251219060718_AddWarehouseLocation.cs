using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BatchLocationStocks_InventoryBatchId_WarehouseId_LocationCode",
                table: "BatchLocationStocks");

            migrationBuilder.DropColumn(
                name: "BinName",
                table: "BatchLocationStocks");

            migrationBuilder.DropColumn(
                name: "RackName",
                table: "BatchLocationStocks");

            migrationBuilder.DropColumn(
                name: "ShelfName",
                table: "BatchLocationStocks");

            migrationBuilder.DropColumn(
                name: "ZoneName",
                table: "BatchLocationStocks");

            migrationBuilder.AddColumn<int>(
                name: "WarehouseLocationId",
                table: "BatchLocationStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WarehouseLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: true),
                    ZoneName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RackName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ShelfName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BinName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseLocations_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchLocationStocks_InventoryBatchId_WarehouseLocationId",
                table: "BatchLocationStocks",
                columns: new[] { "InventoryBatchId", "WarehouseLocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BatchLocationStocks_WarehouseLocationId",
                table: "BatchLocationStocks",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseLocations_IsActive",
                table: "WarehouseLocations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseLocations_LocationCode",
                table: "WarehouseLocations",
                column: "LocationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseLocations_WarehouseId",
                table: "WarehouseLocations",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_BatchLocationStocks_WarehouseLocations_WarehouseLocationId",
                table: "BatchLocationStocks",
                column: "WarehouseLocationId",
                principalTable: "WarehouseLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BatchLocationStocks_WarehouseLocations_WarehouseLocationId",
                table: "BatchLocationStocks");

            migrationBuilder.DropTable(
                name: "WarehouseLocations");

            migrationBuilder.DropIndex(
                name: "IX_BatchLocationStocks_InventoryBatchId_WarehouseLocationId",
                table: "BatchLocationStocks");

            migrationBuilder.DropIndex(
                name: "IX_BatchLocationStocks_WarehouseLocationId",
                table: "BatchLocationStocks");

            migrationBuilder.DropColumn(
                name: "WarehouseLocationId",
                table: "BatchLocationStocks");

            migrationBuilder.AddColumn<string>(
                name: "BinName",
                table: "BatchLocationStocks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RackName",
                table: "BatchLocationStocks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShelfName",
                table: "BatchLocationStocks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZoneName",
                table: "BatchLocationStocks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BatchLocationStocks_InventoryBatchId_WarehouseId_LocationCode",
                table: "BatchLocationStocks",
                columns: new[] { "InventoryBatchId", "WarehouseId", "LocationCode" },
                unique: true);
        }
    }
}
