using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SaleUserId",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ManagerId",
                table: "Users",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_SaleUserId",
                table: "Customers",
                column: "SaleUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_SaleUserId",
                table: "Customers",
                column: "SaleUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_ManagerId",
                table: "Users",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_SaleUserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_ManagerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ManagerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Customers_SaleUserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "SaleUserId",
                table: "Customers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
