using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class upadtesaleregion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalesManagerId",
                table: "SalesRegions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesRegions_SalesManagerId",
                table: "SalesRegions",
                column: "SalesManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesRegions_Users_SalesManagerId",
                table: "SalesRegions",
                column: "SalesManagerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesRegions_Users_SalesManagerId",
                table: "SalesRegions");

            migrationBuilder.DropIndex(
                name: "IX_SalesRegions_SalesManagerId",
                table: "SalesRegions");

            migrationBuilder.DropColumn(
                name: "SalesManagerId",
                table: "SalesRegions");
        }
    }
}
