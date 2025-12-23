using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesRegions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SalesRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesRegions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SalesRegions",
                columns: new[] { "Id", "Code", "CreatedDate", "Description", "IsActive", "Name", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "MB", new DateTime(2025, 12, 23, 3, 31, 22, 377, DateTimeKind.Utc).AddTicks(5351), "Khu vực miền Bắc Việt Nam", true, "Miền Bắc", null },
                    { 2, "MT", new DateTime(2025, 12, 23, 3, 31, 22, 377, DateTimeKind.Utc).AddTicks(5354), "Khu vực miền Trung Việt Nam", true, "Miền Trung", null },
                    { 3, "MN", new DateTime(2025, 12, 23, 3, 31, 22, 377, DateTimeKind.Utc).AddTicks(5359), "Khu vực miền Nam Việt Nam", true, "Miền Nam", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RegionId",
                table: "Users",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_RegionId",
                table: "Customers",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesRegions_Code",
                table: "SalesRegions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesRegions_IsActive",
                table: "SalesRegions",
                column: "IsActive");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_SalesRegions_RegionId",
                table: "Customers",
                column: "RegionId",
                principalTable: "SalesRegions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_SalesRegions_RegionId",
                table: "Users",
                column: "RegionId",
                principalTable: "SalesRegions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_SalesRegions_RegionId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_SalesRegions_RegionId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "SalesRegions");

            migrationBuilder.DropIndex(
                name: "IX_Users_RegionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Customers_RegionId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Customers");
        }
    }
}
