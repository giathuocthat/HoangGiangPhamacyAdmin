using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class update_supplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPaymentAccounts_Bank_BankId",
                table: "CustomerPaymentAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bank",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Suppliers");

            migrationBuilder.RenameTable(
                name: "Bank",
                newName: "Banks");

            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "Suppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Suppliers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banks",
                table: "Banks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_BankId",
                table: "Suppliers",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPaymentAccounts_Banks_BankId",
                table: "CustomerPaymentAccounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Banks_BankId",
                table: "Suppliers",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPaymentAccounts_Banks_BankId",
                table: "CustomerPaymentAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Banks_BankId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_BankId",
                table: "Suppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Banks",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Suppliers");

            migrationBuilder.RenameTable(
                name: "Banks",
                newName: "Bank");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Suppliers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bank",
                table: "Bank",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPaymentAccounts_Bank_BankId",
                table: "CustomerPaymentAccounts",
                column: "BankId",
                principalTable: "Bank",
                principalColumn: "Id");
        }
    }
}
