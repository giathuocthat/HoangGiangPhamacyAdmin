using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThuocGiaThat.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddColumn_DrugEfficient_DosageInstrction_to_product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DosageInstructions",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrugEfficacy",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DosageInstructions",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DrugEfficacy",
                table: "Products");
        }
    }
}
