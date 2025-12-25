using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class addPropsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 25, 7, 26, 50, 433, DateTimeKind.Utc).AddTicks(9929));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 25, 7, 26, 50, 433, DateTimeKind.Utc).AddTicks(9932));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 25, 7, 26, 50, 433, DateTimeKind.Utc).AddTicks(9934));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 24, 4, 45, 35, 589, DateTimeKind.Utc).AddTicks(580));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 24, 4, 45, 35, 589, DateTimeKind.Utc).AddTicks(582));

            migrationBuilder.UpdateData(
                table: "SalesRegions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 24, 4, 45, 35, 589, DateTimeKind.Utc).AddTicks(584));
        }
    }
}
