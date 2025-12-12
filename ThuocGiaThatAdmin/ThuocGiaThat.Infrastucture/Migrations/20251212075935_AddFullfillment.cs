using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddFullfillment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantityFulfilled",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OrderItemFulfillments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    InventoryBatchId = table.Column<int>(type: "int", nullable: false),
                    QuantityFulfilled = table.Column<int>(type: "int", nullable: false),
                    FulfilledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FulfilledByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemFulfillments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemFulfillments_InventoryBatches_InventoryBatchId",
                        column: x => x.InventoryBatchId,
                        principalTable: "InventoryBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItemFulfillments_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemFulfillments_FulfilledDate",
                table: "OrderItemFulfillments",
                column: "FulfilledDate");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemFulfillments_InventoryBatchId",
                table: "OrderItemFulfillments",
                column: "InventoryBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemFulfillments_OrderItemId",
                table: "OrderItemFulfillments",
                column: "OrderItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemFulfillments");

            migrationBuilder.DropColumn(
                name: "QuantityFulfilled",
                table: "OrderItems");
        }
    }
}
