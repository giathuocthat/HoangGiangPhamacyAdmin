using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerInvoiceInfoId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerInvoiceInfoId",
                table: "Orders",
                column: "CustomerInvoiceInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CustomerInvoiceInfo_CustomerInvoiceInfoId",
                table: "Orders",
                column: "CustomerInvoiceInfoId",
                principalTable: "CustomerInvoiceInfo",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CustomerInvoiceInfo_CustomerInvoiceInfoId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerInvoiceInfoId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerInvoiceInfoId",
                table: "Orders");
        }
    }
}
