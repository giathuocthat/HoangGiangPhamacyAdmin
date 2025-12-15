using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class addTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpCodes_Phone_Code_IsUsed",
                table: "OtpCodes");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "OtpCodes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OtpCodes_Phone_Code_IsUsed_Type",
                table: "OtpCodes",
                columns: new[] { "Phone", "Code", "IsUsed", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpCodes_Phone_Code_IsUsed_Type",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "OtpCodes");

            migrationBuilder.CreateIndex(
                name: "IX_OtpCodes_Phone_Code_IsUsed",
                table: "OtpCodes",
                columns: new[] { "Phone", "Code", "IsUsed" });
        }
    }
}
